using Mandara.Business.Calculators;
using Mandara.Business.Contracts;
using Mandara.Business.Model;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Extensions.Collections;
using Mandara.Extensions.Guids;
using Mandara.Extensions.Option;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.Services
{
    public class TradeSplitService : ITradeSplitService
    {
        private class NegativeIdGenerator
        {
            private const int BaseId = -1;
            private int _currentId = BaseId;
            private readonly IEnumerator<int> _idGenerator;

            public NegativeIdGenerator()
            {
                _idGenerator = GetIdGenerator();
            }

            private IEnumerator<int> GetIdGenerator()
            {
                while (true)
                {
                    if (_currentId == int.MinValue)
                    {
                        _currentId = BaseId;
                    }

                    yield return _currentId--;
                }
            }

            public int Next()
            {
                _idGenerator.MoveNext();
                return _idGenerator.Current;
            }
        }

        private class StripData
        {
            public DateTime StripDateTime { get; }
            public ProductDateType StripProductDateType { get; }

            public StripData(Tuple<DateTime, ProductDateType> parsedStrip)
            {
                StripDateTime = parsedStrip.Item1;
                StripProductDateType = parsedStrip.Item2;
            }
        }

        private readonly IProductsStorage _productsStorage;
        private readonly ISecurityDefinitionsStorage _securityDefinitionsStorage;
        private readonly IPrecalcPositionsCalculator _precalcPositionsCalculator;
        private readonly NegativeIdGenerator _securityDefIdGenerator = new NegativeIdGenerator();
        private readonly NegativeIdGenerator _tradeIdGenerator = new NegativeIdGenerator();
        private const string DefaultTradeOrderStatus = "Filled";
        private const string DefaultTradeSide = "Buy";
        private const decimal DefaultTradePrice = 10M;
        private const decimal DefaultTradeQuantity = 1M;
        // TODO: Move to TradeParser?
        public const string WeeklyDiffStripNameFormat = "ddMMMyyyy";
        public const string StripNameYearComponent = "year";
        public const string StripNameMonthComponent = "month";
        public const string StripNameDayComponent = "day";
        public static string stripNameAsDatePattern =
            "(?<"
                + StripNameDayComponent
                + ">[0-9]{2})(?<"
                + StripNameMonthComponent
                + ">[A-Za-z]{3})(?<"
                + StripNameYearComponent
                + ">20[0-9]{2})";

        public TradeSplitService(
            IProductsStorage productsStorage,
            ISecurityDefinitionsStorage securityDefStorage,
            IPrecalcPositionsCalculator precalcPositionsCalculator)
        {
            _productsStorage = productsStorage;
            _securityDefinitionsStorage = securityDefStorage;
            _precalcPositionsCalculator = precalcPositionsCalculator;
        }

        public List<SdQuantityModel> SplitQCalTimeSpreadIntoPerMonth(List<SdQuantityModel> sdQuantities)
        {
            List<SdQuantityModel> splitModels = new List<SdQuantityModel>();

            foreach (SdQuantityModel model in sdQuantities)
            {
                TryGetResult<SecurityDefinition> secDef =
                    _securityDefinitionsStorage.TryGetSecurityDefinition(model.SecurityDefinitionId);

                if (!secDef.HasValue)
                {
                    continue;
                }

                List<SdQuantityModel> tempModels = new List<SdQuantityModel>();
                string[] strips = secDef.Value.StripName.Split('/');
                DateTime today = SystemTime.Today();

                foreach (string stripName in strips)
                {
                    StripData parsedStrip = new StripData(StripHelper.ParseStripDate(stripName, today));
                    int numberOfStrips = strips.Length;

                    if (IsStandardMultiMonthOrComplexTrade(parsedStrip.StripProductDateType, numberOfStrips))
                    {
                        tempModels.AddRange(SplitStandardMultiMonthOrComplexTrade(model, secDef.Value, parsedStrip));
                    }
                    else
                    {
                        tempModels.Clear();
                        tempModels.Add(model);
                        break;
                    }
                }

                splitModels.AddRange(tempModels);
            }

            return splitModels;
        }

        private List<SdQuantityModel> SplitStandardMultiMonthOrComplexTrade(
            SdQuantityModel model,
            SecurityDefinition secDef,
            StripData parsedStrip)
        {
            List<SdQuantityModel> splitModels = new List<SdQuantityModel>();
            int numMonths = GetNumberOfMonths(parsedStrip.StripProductDateType);
            TradeCapture mappedTradeCapture = MapSdQuantityToTradeCapture(model, secDef);

            for (int monthShift = 0; monthShift < numMonths; monthShift++)
            {
                DateTime newTradeDateTime = parsedStrip.StripDateTime.AddMonths(monthShift);
                TradeCapture monthTrade = BuildTradeCaptureWithSecurityDefinitionForMonth(
                    mappedTradeCapture,
                    secDef,
                    newTradeDateTime);
                SecurityDefinition monthSecDef = monthTrade.SecurityDefinition;
                SdQuantityModel monthModel = new SdQuantityModel
                {
                    SecurityDefinitionId = monthSecDef.SecurityDefinitionId,
                    PortfolioId = model.PortfolioId,
                    TradesFxExposure = model.TradesFxExposure,
                    TradesQuantity = model.TradesQuantity
                };

                _securityDefinitionsStorage.TryAdd(monthSecDef);
                splitModels.Add(monthModel);
            }

            return splitModels;
        }

        private TradeCapture MapSdQuantityToTradeCapture(SdQuantityModel sdQuantity, SecurityDefinition secDef)
        {
            TradeCapture tradeCapture = new TradeCapture()
            {
                SecurityDefinitionId = sdQuantity.SecurityDefinitionId,
                SecurityDefinition = secDef,
                PortfolioId = sdQuantity.PortfolioId,
                Quantity = sdQuantity.TradesQuantity,
                Price = DefaultTradePrice,
                Side = DefaultTradeSide,
                OrdStatus = DefaultTradeOrderStatus,
            };

            return tradeCapture;
        }

        private bool IsStandardMultiMonthOrComplexTrade(ProductDateType dateType, int numberOfStrips)
        {
            return IsStandardMultiMonthTrade(dateType)
                || (dateType == ProductDateType.MonthYear && numberOfStrips == 2);
        }

        private bool IsStandardMultiMonthTrade(ProductDateType dateType)
        {
            return dateType == ProductDateType.Quarter || dateType == ProductDateType.Year;
        }

        private int GetNumberOfMonths(ProductDateType dateType)
        {
            int numMonths = 1;

            switch (dateType)
            {
                case ProductDateType.Quarter:
                numMonths = 3;
                break;
                case ProductDateType.Year:
                numMonths = 12;
                break;
            }

            return numMonths;
        }

        public List<TradeCapture> SplitQCalTimeSpreadIntoPerMonth(List<TradeCapture> tradeCaptures)
        {
            List<TradeCapture> splitTrades = new List<TradeCapture>();
            DateTime today = SystemTime.Today();

            foreach (TradeCapture trade in tradeCaptures)
            {
                bool isTradeSplit = false;
                SecurityDefinition secDef = trade.SecurityDefinition;

                if (IsNotCustomPeriodTrade(trade))
                {
                    StripData parsedStrip = new StripData(StripHelper.ParseStripDate(secDef.StripName, today));

                    if (IsStandardMultiMonthTrade(parsedStrip.StripProductDateType))
                    {
                        isTradeSplit = true;

                        List<TradeCapture> split = SplitStandardMultiMonthTrade(trade, secDef, parsedStrip);
                        splitTrades.AddRange(split);
                    }
                }

                if (!isTradeSplit)
                {
                    splitTrades.Add(trade);
                }
            }

            return splitTrades;
        }

        private bool IsNotCustomPeriodTrade(TradeCapture trade)
        {
            Product product = trade.SecurityDefinition.Product;

            return !IsCustomMonthlyTrade(trade, trade.SecurityDefinition)
                && (!product.IsProductDaily);
        }

        private List<TradeCapture> SplitStandardMultiMonthTrade(
            TradeCapture trade,
            SecurityDefinition secDef,
            StripData parsedStrip)
        {
            List<TradeCapture> splitTrades = new List<TradeCapture>();
            int numMonths = GetNumberOfMonths(parsedStrip.StripProductDateType);

            for (int monthShift = 0; monthShift < numMonths; monthShift++)
            {
                DateTime newTradeDateTime = parsedStrip.StripDateTime.AddMonths(monthShift);
                TradeCapture monthTrade = BuildTradeCaptureWithSecurityDefinitionForMonth(
                    trade,
                    secDef,
                    newTradeDateTime);
                SecurityDefinition monthSecDef = monthTrade.SecurityDefinition;

                SetTradeProperties(monthTrade, trade);

                _securityDefinitionsStorage.TryAdd(monthSecDef);
                splitTrades.Add(monthTrade);
            }

            return splitTrades;
        }

        private static void SetTradeProperties(
            TradeCapture newTrade,
            TradeCapture originalTrade)
        {
            SecurityDefinition newSd = newTrade.SecurityDefinition;

            newTrade.SecurityDefinitionId = newSd.SecurityDefinitionId;
            newTrade.TradeEndDate = originalTrade.TradeEndDate;
            newTrade.TradeStartDate = originalTrade.TradeStartDate;
            newTrade.PortfolioId = originalTrade.PortfolioId;

            newTrade.PrecalcDetails = newSd.PrecalcDetails.Select(x => new PrecalcTcDetail
            {
                Month = x.Month,
                DaysSerialized = x.DaysSerialized,
                MaxDay = x.MaxDay,
                MinDay = x.MinDay,
                ProductId = x.ProductId
            }).ToList();

            newTrade.IsParentTrade = true;
            newTrade.Price = originalTrade.Price;
            newTrade.Quantity = originalTrade.Quantity;
            newTrade.TradeId = newTrade.TradeId;
        }

        public List<TradeModel> SplitHistoricCustomPeriodTrades(List<TradeModel> trades)
        {
            return SplitCustomPeriodTrades(trades, CalculateNonLiveTradeQuantitySplitFactor);
        }

        public List<TradeModel> SplitLiveCustomPeriodTrades(List<TradeModel> trades)
        {
            return SplitCustomPeriodTrades(trades, CalculateLiveTradeQuantitySplitFactor);
        }

        private List<TradeModel> SplitCustomPeriodTrades(
            List<TradeModel> trades,
            Func<List<DateTime>, int> dailyTradesSplitFactor)
        {
            List<TradeModel> splitTrades = new List<TradeModel>();

            foreach (TradeModel trade in trades)
            {
                SecurityDefinition securityDefinition = GetSecurityDefinitionById(trade.SecurityDefinitionId);
                TradeCapture mappedTradeCapture = MapTradeModelToTradeCapture(trade, securityDefinition);

                if (IsCustomMonthlyTrade(mappedTradeCapture, securityDefinition))
                {
                    IEnumerable<TradeModel> splitTradeModels =
                        SplitCustomMonthtlyTrade(mappedTradeCapture, securityDefinition)
                            .Select(
                                tradeCap =>
                                    NewTradeModelFromSplitSecurityDef(
                                        tradeCap,
                                        tradeCap.SecurityDefinition,
                                        GetPrecalcDetails(tradeCap.SecurityDefinition),
                                        tradeCap.TradeId));

                    splitTrades.AddRange(splitTradeModels);
                }
                else if (securityDefinition.Product.IsWeeklyDiffOrDailySwapProduct())
                {
                    IEnumerable<TradeModel> splitTradeModels =
                        SplitFullWeekDailyTrade(
                            mappedTradeCapture,
                            securityDefinition,
                            dailyTradesSplitFactor)
                            .Select(
                                tradeCap =>
                                    NewTradeModelFromSplitSecurityDef(
                                        tradeCap,
                                        tradeCap.SecurityDefinition,
                                        GetPrecalcDetails(tradeCap),
                                        tradeCap.TradeId));

                    splitTrades.AddRange(splitTradeModels);
                }
                else
                {
                    splitTrades.Add(trade);
                }
            }

            return splitTrades;
        }


        private SecurityDefinition GetSecurityDefinitionById(int securityDefinitionId)
        {
            SecurityDefinition securityDefinition =
                _securityDefinitionsStorage.TryGetSecurityDefinition(securityDefinitionId).Value;

            if (null == securityDefinition.Product)
            {
                securityDefinition.Product = _productsStorage.TryGetProduct(securityDefinition.product_id.Value).Value;
            }

            return securityDefinition;
        }

        private TradeCapture MapTradeModelToTradeCapture(TradeModel trade, SecurityDefinition secDef)
        {
            TradeCapture tradeCapture = new TradeCapture()
            {
                SecurityDefinitionId = trade.SecurityDefinitionId,
                SecurityDefinition = secDef,
                PortfolioId = trade.PortfolioId,
                Quantity = trade.Quantity,
                Price = trade.Price,
                Side = DefaultTradeSide,
                OrdStatus = DefaultTradeOrderStatus,
                TradeStartDate = trade.TradeStartDate,
                TradeEndDate = trade.TradeEndDate,
                IsParentTrade = trade.IsParentTrade,
                PrecalcDetails = GetTradeCapturePrecalcDetails(trade.PrecalcDetails),
            };

            return tradeCapture;
        }

        private bool IsCustomMonthlyTrade(TradeCapture trade, SecurityDefinition secDef)
        {
            return StripHelper.StripNameCustomVariants.Contains(secDef.StripName)
                   && trade.TradeStartDate.HasValue
                   && trade.TradeEndDate.HasValue;
        }

        private List<TradeCapture> SplitCustomMonthtlyTrade(
            TradeCapture trade,
            SecurityDefinition baseSecurityDefinition)
        {
            List<TradeCapture> splitTrades = new List<TradeCapture>();
            DateTime startMonth = trade.TradeStartDate.Value.Date;
            DateTime endMonth = trade.TradeEndDate.Value.AddMonths(1);
            endMonth = new DateTime(endMonth.Year, endMonth.Month, 1);

            for (DateTime month = startMonth; month < endMonth; month = month.AddMonths(1))
            {
                TradeCapture monthTrade = BuildTradeCaptureWithSecurityDefinitionForMonth(
                    trade,
                    baseSecurityDefinition,
                    month);
                SecurityDefinition monthSecDef = monthTrade.SecurityDefinition;

                _securityDefinitionsStorage.TryAdd(monthSecDef);
                splitTrades.Add(monthTrade);
            }

            return splitTrades;
        }

        private List<PrecalcTcDetail> GetTradeCapturePrecalcDetails(List<PrecalcDetailModel> precalcDetails)
        {
            if (null == precalcDetails)
            {
                return null;
            }

            return
                precalcDetails.Select(
                    detail =>
                        new PrecalcTcDetail()
                        {
                            DaysSerialized = detail.DaysSerialized,
                            MinDay = detail.MinDay,
                            MaxDay = detail.MaxDay,
                            Month = detail.Month,
                            ProductId = detail.ProductId
                        }).ToList();
        }

        public List<TradeCapture> SplitCustomPeriodTrades(List<TradeCapture> trades)
        {
            List<TradeCapture> splitTrades = new List<TradeCapture>();

            foreach (TradeCapture trade in trades)
            {
                SecurityDefinition securityDefinition = GetSecurityDefinitionById(trade.SecurityDefinitionId);

                if (IsCustomMonthlyTrade(trade, securityDefinition))
                {
                    IEnumerable<TradeCapture> splitTradeCaptures =
                        SplitCustomMonthtlyTrade(trade, securityDefinition).Select(
                            tradeCap =>
                            {
                                SetTradeProperties(tradeCap, trade);
                                return tradeCap;
                            });

                    splitTrades.AddRange(splitTradeCaptures);
                }
                else if (securityDefinition.Product.IsWeeklyDiffOrDailySwapProduct())
                {
                    List<TradeCapture> splitTradeCaptures = SplitFullWeekDailyTrade(
                        trade,
                        securityDefinition,
                        CalculateLiveTradeQuantitySplitFactor);

                    splitTrades.AddRange(splitTradeCaptures);
                }
                else
                {
                    splitTrades.Add(trade);
                }
            }

            return splitTrades;
        }

        /// <summary>
        /// Trades that are not live will have an overnight PnL only which is calculated from the position, ie.e the
        /// precalc details.  There is therefore no need to split out the trade quantity.  And, in fact, doing so will
        /// result in incorect overnight PnL (the reason for this has not been investigated yet).
        /// </summary>
        /// <param name="tradeDays"></param>
        /// <returns></returns>
        private int CalculateNonLiveTradeQuantitySplitFactor(List<DateTime> tradeDays)
        {
            return 1;
        }

        /// <summary>
        /// Live trades have PnL calculated using only the trade quantity.  This means that when splitting them the
        /// trade quantity needs to be separated per split trade rather than the precalc details quantity.
        /// </summary>
        /// <param name="tradeDays"></param>
        /// <returns></returns>
        private int CalculateLiveTradeQuantitySplitFactor(List<DateTime> tradeDays)
        {
            return tradeDays.Count;
        }

        private TradeModel NewTradeModelFromSplitSecurityDef(
            TradeCapture baseTrade,
            SecurityDefinition splitSecDef,
            List<PrecalcDetailModel> precalcDetails,
            int newTradeId)
        {
            TradeModel monthModel = new TradeModel()
            {
                SecurityDefinitionId = splitSecDef.SecurityDefinitionId,
                SecurityDefinition = splitSecDef,
                TradeEndDate = baseTrade.TradeEndDate,
                TradeStartDate = baseTrade.TradeStartDate,
                PortfolioId = baseTrade.PortfolioId,
                PrecalcDetails = precalcDetails,
                IsParentTrade = baseTrade.IsParentTrade,
                Price = baseTrade.Price,
                Quantity = baseTrade.Quantity,
                TradeId = newTradeId
            };

            return monthModel;
        }

        private List<PrecalcDetailModel> GetPrecalcDetails(SecurityDefinition secDef)
        {
            return secDef.PrecalcDetails.Select(
                        x =>
                            new PrecalcDetailModel
                            {
                                Month = x.Month,
                                DaysSerialized = x.DaysSerialized,
                                MaxDay = x.MaxDay,
                                MinDay = x.MinDay,
                                ProductId = x.ProductId
                            }).ToList();
        }

        private List<PrecalcDetailModel> GetPrecalcDetails(TradeCapture trade)
        {
            if (null != trade.PrecalcDetails)
            {
                return trade.PrecalcDetails.Select(
                        x =>
                            new PrecalcDetailModel
                            {
                                Month = x.Month,
                                DaysSerialized = x.DaysSerialized,
                                MaxDay = x.MaxDay,
                                MinDay = x.MinDay,
                                ProductId = x.ProductId
                            }).ToList();
            }
            else
            {
                return new List<PrecalcDetailModel>();
            }
        }

        private TradeCapture BuildTradeCaptureWithSecurityDefinitionForMonth(
            TradeCapture originalTrade,
            SecurityDefinition secDef,
            DateTime month)
        {
            DateTime endOfMonth = new DateTime(
                month.Year,
                month.Month,
                DateTime.DaysInMonth(month.Year, month.Month));
            SecurityDefinition securityDefinition = BuildNewSecurityDefinition(secDef, month, endOfMonth);
            TradeCapture tradeCapture = BuildTradeCapture(
                originalTrade,
                securityDefinition,
                month,
                endOfMonth,
                ProductDateType.MonthYear);

            Tuple<ICollection<PrecalcTcDetail>, ICollection<PrecalcSdDetail>> precalcDetails =
                _precalcPositionsCalculator.CalculatePrecalcPositions(tradeCapture, securityDefinition);

            securityDefinition.PrecalcDetails = precalcDetails.Item2;
            return tradeCapture;
        }

        private SecurityDefinition BuildNewSecurityDefinition(
            SecurityDefinition baseSecDef,
            DateTime startDate,
            DateTime endOfMonth)
        {
            SecurityDefinition securityDefinition = new SecurityDefinition();

            securityDefinition.SecurityDefinitionId = _securityDefIdGenerator.Next();
            securityDefinition.Product = GetNewSecurityDefinitionProduct(baseSecDef);
            securityDefinition.StripName = startDate.ToString("MMMyy");

            securityDefinition.ProductDescription = baseSecDef.ProductDescription;
            securityDefinition.UnderlyingSymbol = GenerateNewGuid();
            securityDefinition.Exchange = baseSecDef.Exchange;
            securityDefinition.UnderlyingSecurityDesc = baseSecDef.UnderlyingSecurityDesc;
            securityDefinition.StartDate = startDate.ToShortDateString();
            securityDefinition.StartDateAsDate = startDate;

            securityDefinition.EndDate = endOfMonth.ToShortDateString();
            securityDefinition.EndDateAsDate = endOfMonth;
            securityDefinition.UnderlyingMaturityDate = startDate.ToShortDateString();
            securityDefinition.Exchange = baseSecDef.Exchange;
            securityDefinition.HubAlias = baseSecDef.HubAlias;
            return securityDefinition;
        }

        private Product GetNewSecurityDefinitionProduct(SecurityDefinition baseSecDef)
        {
            if (null == baseSecDef.Product || baseSecDef.product_id != baseSecDef.Product.ProductId)
            {
                return _productsStorage.TryGetProduct(baseSecDef.product_id.Value).Value;
            }

            return baseSecDef.Product;
        }

        private TradeCapture BuildTradeCapture(
            TradeCapture originalTrade,
            SecurityDefinition securityDefinition,
            DateTime startDate,
            DateTime endDate,
            ProductDateType productDateType)
        {
            TradeCapture tradeCapture = new TradeCapture();

            tradeCapture.TradeId = _tradeIdGenerator.Next();
            tradeCapture.SecurityDefinition = securityDefinition;
            tradeCapture.PortfolioId = null != originalTrade ? originalTrade.PortfolioId : null;
            tradeCapture.Price = null != originalTrade ? originalTrade.Price : DefaultTradePrice;
            tradeCapture.Quantity = null != originalTrade ? originalTrade.Quantity : DefaultTradeQuantity;
            tradeCapture.Side = null != originalTrade ? originalTrade.Side : DefaultTradeSide;
            tradeCapture.TradeStartDate = startDate;
            tradeCapture.TradeEndDate = endDate;

            tradeCapture.Exchange = securityDefinition.Exchange;
            tradeCapture.ExecutingFirm = String.Empty;
            tradeCapture.OrdStatus = null != originalTrade ? originalTrade.OrdStatus : DefaultTradeOrderStatus;
            tradeCapture.ExecID = GenerateNewGuid();

            tradeCapture.NumOfLots = 1;
            tradeCapture.CreatedBy = tradeCapture.OriginationTrader = "DuplicateLegSd";
            tradeCapture.IsParentTrade = true;
            tradeCapture.Pending = false;

            tradeCapture.TradeDate = startDate;
            tradeCapture.TransactTime = tradeCapture.TimeStamp = startDate;

            tradeCapture.SecurityDefinition.Strip1Date = startDate;
            tradeCapture.SecurityDefinition.Strip1DateType = productDateType;
            return tradeCapture;
        }

        private string GenerateNewGuid()
        {
            return GuidExtensions.NumericGuid(GuidExtensions.HalfGuidLength);
        }

        private List<TradeCapture> SplitFullWeekDailyTrade(
            TradeCapture trade,
            SecurityDefinition securityDefinition,
            Func<List<DateTime>, int> calculateTradeQuantitySplitFactor)
        {
            List<TradeCapture> splitWeekly = new List<TradeCapture>();
            List<DateTime> tradeDays = GetDailyTradeDates(trade.PrecalcDetails);
            PrecalcTcDetail firstLegPrecalc = trade.PrecalcDetails.First();
            decimal? firstLegDailyQuantity = firstLegPrecalc.DaysPositions.First().Value;
            decimal? quantity = firstLegDailyQuantity.HasValue
                ? Math.Abs(firstLegDailyQuantity.Value)
                : default(decimal?);
            int tradeQuantitySplitFactor = calculateTradeQuantitySplitFactor(tradeDays);

            if (!quantity.HasValue)
            {
                splitWeekly.Add(trade);
                return splitWeekly;
            }

            foreach (DateTime tradeDay in tradeDays)
            {
                SecurityDefinition daySecDef = BuildNewSecurityDefinition(securityDefinition, tradeDay, tradeDay);

                daySecDef.StripName = tradeDay.ToString(WeeklyDiffStripNameFormat);

                TradeCapture dayTrade = BuildLiveTradeCaptureWithSecurityDefinitionForDay(
                    trade,
                    daySecDef,
                    tradeDay,
                    quantity.Value,
                    tradeQuantitySplitFactor);

                _securityDefinitionsStorage.TryAdd(daySecDef);
                splitWeekly.Add(dayTrade);
            }

            return splitWeekly;
        }

        private List<DateTime> GetDailyTradeDates(ICollection<PrecalcTcDetail> tradePrecalcDetails)
        {
            HashSet<DateTime> dates = new HashSet<DateTime>();

            tradePrecalcDetails.ForEach(
                precalcDetail =>
                    precalcDetail.DaysPositions.ForEach(dayPosition => dates.Add(dayPosition.Key)));
            return dates.OrderBy(date => date).ToList();
        }

        private TradeCapture BuildTradeCaptureWithSecurityDefinitionForDay(
            TradeCapture baseTrade,
            SecurityDefinition baseSecDef,
            DateTime day,
            decimal precalcDetailDailyQuantity)
        {
            TradeCapture tradeCapture = BuildTradeCapture(
                baseTrade,
                baseSecDef,
                day,
                day,
                ProductDateType.Day);
            SecurityDefinition securityDefinition = tradeCapture.SecurityDefinition;

            Tuple<ICollection<PrecalcTcDetail>, ICollection<PrecalcSdDetail>> precalcDetails =
                _precalcPositionsCalculator.CalculatePrecalcPositions(tradeCapture, securityDefinition);
            List<PrecalcTcDetail> splitTradeCaptPrecalcDetails;

            // The precalc details will have the base product single contract quantity rather than the quantity for a
            // single day as given in the original trade's precalc details.
            if (precalcDetails.Item1.Any())
            {
                splitTradeCaptPrecalcDetails = ApplyDailyPrecalcQuantityForSplitDayTrade(
                    precalcDetails.Item1.ToList(),
                    precalcDetailDailyQuantity).ToList();
            }
            else
            {
                splitTradeCaptPrecalcDetails = new List<PrecalcTcDetail>();
            }

            tradeCapture.PrecalcDetails = splitTradeCaptPrecalcDetails;
            return tradeCapture;
        }

        private IEnumerable<PrecalcTcDetail> ApplyDailyPrecalcQuantityForSplitDayTrade(
            List<PrecalcTcDetail> precalcDetails,
            decimal precalcDetailDailyQuantity)
        {
            foreach (PrecalcTcDetail detail in precalcDetails)
            {
                PrecalcTcDetail splitDetail = new PrecalcTcDetail()
                {
                    MaxDay = detail.MaxDay,
                    MinDay = detail.MinDay,
                    Month = detail.Month,
                    DaysPositions = UpdateDayPositionQuantity(detail.DaysPositions, precalcDetailDailyQuantity),
                    ProductId = detail.ProductId,
                    TradeCapture = detail.TradeCapture,
                };

                yield return splitDetail;
            }
        }

        private TradeCapture BuildLiveTradeCaptureWithSecurityDefinitionForDay(
            TradeCapture baseTrade,
            SecurityDefinition baseSecDef,
            DateTime day,
            decimal precalcDetailDailyQuantity,
            int tradeQuantitySplitFactor)
        {
            TradeCapture newTradeCapture = BuildTradeCaptureWithSecurityDefinitionForDay(
                baseTrade,
                baseSecDef,
                day,
                precalcDetailDailyQuantity);

            newTradeCapture.Quantity = newTradeCapture.Quantity / tradeQuantitySplitFactor;
            return newTradeCapture;
        }

        private Dictionary<DateTime, decimal?> UpdateDayPositionQuantity(
            Dictionary<DateTime, decimal?> dayPositions,
            decimal quantity)
        {
            Dictionary<DateTime, decimal?> updatedDayPositions = new Dictionary<DateTime, decimal?>();

            dayPositions.Keys.ForEach(
                day =>
                {
                    decimal? initialQuantity = dayPositions[day];

                    if (initialQuantity.HasValue && initialQuantity.Value != 0M)
                    {
                        updatedDayPositions[day] =
                            quantity * initialQuantity.Value / Math.Abs(initialQuantity.Value);
                    }
                    else
                    {
                        updatedDayPositions[day] = 0M;
                    }
                });
            return updatedDayPositions;
        }

        public List<TradeCapture> SplitTradeCaptures(List<TradeCapture> trades)
        {
            List<TradeCapture> splitTradeCaptures = new List<TradeCapture>();
            List<TradeCapture> splitQAndCalTrades = SplitQCalTimeSpreadIntoPerMonth(trades);

            splitTradeCaptures = SplitCustomPeriodTrades(splitQAndCalTrades);
            return splitTradeCaptures;
        }
    }
}
