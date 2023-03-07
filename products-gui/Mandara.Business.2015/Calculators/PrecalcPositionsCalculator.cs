using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Mandara.Business.Contracts;
using Mandara.Business.Managers;
using Mandara.Business.OldCode;
using Mandara.Date;
using Mandara.Entities;
using Mandara.Entities.Calculation;
using Mandara.Entities.EntityPieces;
using Mandara.Entities.Services;
using MoreLinq;

namespace Mandara.Business.Calculators
{
    public class PrecalcPositionsCalculator : IPrecalcPositionsCalculator
    {
        private readonly CalculationCache _calculationCache;
        private bool _calcCacheInitialized;
        private readonly List<SecurityDefinition> _processedSecurityDefinitions;
        private readonly IProductsStorage _productStorage;
        private readonly PricingPrePositionsManager _pricingPositionsManager;

        private static Func<MandaraEntities> _productsDb;
        public static Func<MandaraEntities> ProductsDb
        {
            get => _productsDb ?? (_productsDb = DefaultProductsDatabase);
            set
            {
                _productsDb = value ?? DefaultProductsDatabase;
                CalculationCache.ProductsDb = _productsDb;
            }
        }

        public PrecalcPositionsCalculator(IProductsStorage storage, PricingPrePositionsManager pricingPositionsManager)
        {
            ExistingTradePrecalcs = new ConcurrentDictionary<int, List<PrecalcTcDetail>>();
            ExistingSecurityPrecalcs = new ConcurrentDictionary<int, List<PrecalcSdDetail>>();
            ClearerPrecalcs = new List<PrecalcSourcedetailsDetail>();
            ClearerSecurityPrecalcs = new List<PrecalcSdDetail>();
            _calculationCache = new CalculationCache();
            _processedSecurityDefinitions = new List<SecurityDefinition>();
            _productStorage = storage;
            _pricingPositionsManager = pricingPositionsManager;
        }

        private static MandaraEntities DefaultProductsDatabase()
        {
            return new MandaraEntities(MandaraEntities.DefaultConnStrName, nameof(PrecalcPositionsCalculator));
        }

        public ConcurrentDictionary<int, List<PrecalcTcDetail>> ExistingTradePrecalcs { get; }
        public ConcurrentDictionary<int, List<PrecalcSdDetail>> ExistingSecurityPrecalcs { get; }
        public List<PrecalcSourcedetailsDetail> ClearerPrecalcs { get; }
        public List<PrecalcSdDetail> ClearerSecurityPrecalcs { get; }

        public Tuple<ICollection<PrecalcTcDetail>, ICollection<PrecalcSdDetail>> ForceCalculatePrecalcPositions(
            TradeCapture trade,
            SecurityDefinition securityDefinition)
        {
            trade.PrecalcPositions?.Clear();
            securityDefinition.PrecalcDetails?.Clear();

            return CalculatePrecalcPositions(trade, securityDefinition);
        }

        public Tuple<ICollection<PrecalcTcDetail>, ICollection<PrecalcSdDetail>> CalculatePrecalcPositions(
            TradeCapture trade,
            SecurityDefinition securityDefinition)
        {
            return CalculatePrecalcPositions(trade, securityDefinition, securityDefinition.Product);
        }

        public Tuple<ICollection<PrecalcTcDetail>, ICollection<PrecalcSdDetail>> CalculatePrecalcPositions(
            TradeCapture trade,
            SecurityDefinition securityDefinition,
            Product product)
        {
            if (!PrecalcDetailsAlreadyExist(trade, securityDefinition))
            {
                InitialiseCalculationCache();

                TradePieces tradePieces = new TradePieces(trade, securityDefinition, product);
                List<PricingCalculationDetail> pricingDetails = GetPricingCalculationDetails(tradePieces);

                if (pricingDetails != null && pricingDetails.Count > 0)
                {
                    SetPrecalcDetailsOnTrade(tradePieces, pricingDetails);
                }
            }

            return new Tuple<ICollection<PrecalcTcDetail>, ICollection<PrecalcSdDetail>>(
                trade.PrecalcDetails ?? new List<PrecalcTcDetail>(),
                securityDefinition.PrecalcDetails ?? new List<PrecalcSdDetail>());
        }

        private bool PrecalcDetailsAlreadyExist(TradeCapture trade, SecurityDefinition secDef)
        {
            return trade.HasPrecalcDetailsOnTrade() || SecurityDefinitionHasPrecalcDetails(secDef);
        }

        private bool SecurityDefinitionHasPrecalcDetails(SecurityDefinition secDef)
        {
            if (null == secDef)
            {
                return false;
            }

            if (!secDef.HasPrecalcDetails()
                && ExistingSecurityPrecalcs.TryGetValue(
                    secDef.SecurityDefinitionId,
                    out List<PrecalcSdDetail> securityPrecalcs))
            {
                secDef.PrecalcDetails = securityPrecalcs;
            }

            return secDef.HasPrecalcDetails();
        }

        private void InitialiseCalculationCache()
        {
            if (!_calcCacheInitialized)
            {
                _calculationCache.Initialize(_productStorage);
                _calcCacheInitialized = true;
            }
        }

        private void SetPrecalcDetailsOnTrade(TradePieces trade, List<PricingCalculationDetail> positions)
        {
            trade.Trade.PrecalcPositions = PositionsToPrecalcPositions(positions);

            if (IsNonMonthlyStrip(trade))
            {
                SetPrecalcDetailsOnTradeCapture(trade.Trade);
                AddTradeCapturePrecalcDetailsToLocalStore(trade.Trade);
            }
            else
            {
                SetPrecalcDetailsOnSecurityDef(trade.Security.SecurityDef, trade.Trade.PrecalcPositions);
                AddSecurityDefPrecalcDetailsToLocalStore(trade.Security.SecurityDef);
            }
        }

        private static List<PrecalcDetail> PositionsToPrecalcPositions(
            List<PricingCalculationDetail> positions)
        {
            return positions
                   .DistinctBy(pos => new { pos.CalculationDate, pos.ProductId, pos.SourceProductId, pos.Amount })
                   .GroupBy(pos => new { pos.Period, pos.ProductId })
                   .Select(byProductDate => BuildPrecalcDetail(byProductDate.ToList()))
                   .ToList();
        }

        private static PrecalcDetail BuildPrecalcDetail(List<PricingCalculationDetail> positions)
        {
            DateRange posDateRange = GetPositionDateRange(positions);
            Dictionary<DateTime, decimal?> daysPositions = GetDailyPositions(positions, posDateRange);
            PricingCalculationDetail calculationDetail = positions.First();

            return new PrecalcDetail
            {
                Month = calculationDetail.Period,
                Product = calculationDetail.ProductReference,
                DaysPositions = daysPositions,
                MinDay = posDateRange.Start,
                MaxDay = posDateRange.End
            };
        }

        private static DateRange GetPositionDateRange(List<PricingCalculationDetail> positions)
        {
            (DateTime min, DateTime max) = positions.Aggregate(
                (min: DateTime.MaxValue, max: DateTime.MinValue),
                (currentRange, pos) => (GetMinDate(currentRange.min, pos), GetMaxDate(currentRange.max, pos)));

            return new DateRange(min, max);

            DateTime GetMinDate(DateTime currentMin, PricingCalculationDetail pos)
            {
                return currentMin.CompareTo(pos.CalculationDate) < 0 ? currentMin : pos.CalculationDate;
            }

            DateTime GetMaxDate(DateTime currentMax, PricingCalculationDetail pos)
            {
                return currentMax.CompareTo(pos.CalculationDate) > 0 ? currentMax : pos.CalculationDate;
            }
        }

        private static Dictionary<DateTime, decimal?> GetDailyPositions(
            List<PricingCalculationDetail> positions,
            DateRange posDateRange)
        {
            List<DayPosition> positionPerDay =
                positions.Select(pos => new DayPosition { Day = pos.CalculationDate.Date, Position = pos.Amount })
                              .ToList();

            EnsureWeekendPositionsPresent(positionPerDay);

            // The OrderBy shouldn't be necessary here.
            return positionPerDay.OrderBy(dayPos => dayPos.Day)
                                 .ToDictionary(dayPos => dayPos.Day, dayPos => (decimal?)dayPos.Position);
        }

        private static void EnsureWeekendPositionsPresent(List<DayPosition> positionPerDay)
        {
            List<DateTime> filledDays = positionPerDay.Select(posForDay => posForDay.Day).OrderBy(day => day).ToList();
            DateTime prevDay = filledDays.First();

            filledDays.Skip(1)
                      .ForEach(
                          dayWithPos =>
                          {
                              Enumerable.Range(1, (int)dayWithPos.Subtract(prevDay).TotalDays - 1)
                                        .Select(dayOffset => prevDay.AddDays(dayOffset))
                                        .Where(day => day.IsWeekendDay())
                                        .ForEach(day => positionPerDay.Add(new DayPosition(day, 0M)));
                              prevDay = dayWithPos;
                          });
        }

        private static bool IsNonMonthlyStrip(TradePieces trade)
        {
            SecurityDefinition securityDefinition = trade.Security.SecurityDef;
            List<Product> products = trade.Trade.PrecalcPositions.Select(precalc => precalc.Product).ToList();

            products.Add(trade.Security.Product);

            return TradeCapture.IsNonMonthlyStrip(products, securityDefinition, trade.Strip.Part1.DateType);
        }

        private static void SetPrecalcDetailsOnTradeCapture(TradeCapture trade)
        {
            trade.PrecalcDetails = GetTradeCapturePrecalcDetails(trade);
        }

        private static void SetPrecalcDetailsOnSecurityDef(SecurityDefinition secDef, List<PrecalcDetail> precalc)
        {
            secDef.PrecalcDetails = GetSecDefPrecalcDetails(secDef.SecurityDefinitionId, precalc);
        }

        private void AddSecurityDefPrecalcDetailsToLocalStore(SecurityDefinition secDef)
        {
            if (!secDef.IsNew())
            {
                ExistingSecurityPrecalcs.TryAdd(
                    secDef.SecurityDefinitionId,
                    secDef.PrecalcDetails.ToList());
            }
        }

        private void AddTradeCapturePrecalcDetailsToLocalStore(TradeCapture trade)
        {
            if (!trade.IsNew())
            {
                ExistingTradePrecalcs.TryAdd(trade.TradeId, trade.PrecalcDetails.ToList());
            }
        }

        private static List<PrecalcTcDetail> GetTradeCapturePrecalcDetails(TradeCapture trade)
        {
            List<PrecalcTcDetail> precalcTcDetails =
                trade.PrecalcPositions.Select(
                    precalc =>
                        new PrecalcTcDetail
                        {
                            Month = precalc.Month,
                            DaysPositions = precalc.DaysPositions,
                            ProductId = precalc.Product.ProductId,
                            TradeCaptureId = trade.TradeId,
                            MinDay = precalc.MinDay,
                            MaxDay = precalc.MaxDay,
                        }).ToList();
            return precalcTcDetails;
        }

        private static List<PrecalcSdDetail> GetSecDefPrecalcDetails(int secDefId, List<PrecalcDetail> precalcs)
        {
            List<PrecalcSdDetail> precalcSdDetails =
                precalcs.Select(
                    precalc =>
                        new PrecalcSdDetail
                        {
                            Month = precalc.Month,
                            DaysPositions = precalc.DaysPositions,
                            ProductId = precalc.Product.ProductId,
                            SecurityDefinitionId = secDefId,
                            MinDay = precalc.MinDay,
                            MaxDay = precalc.MaxDay,
                        }).ToList();
            return precalcSdDetails;
        }

        public void CalculatePrecalcPositions(SourceDetail sourceDetail, SecurityDefinition securityDefinition)
        {
            if (!_calcCacheInitialized)
            {
                _calculationCache.Initialize();
                _calcCacheInitialized = true;
            }

            List<PricingCalculationDetail> pricingDetails = GetPricingCalculationDetails(sourceDetail);

            if (pricingDetails != null && pricingDetails.Count > 0)
            {
                List<PrecalcDetail> precalcPositions = ConvertPricingDetailsToPrecalDetails(pricingDetails);

                // balmo and daily goes to the trade, custom strip - also to the trade everything else - to the security
                // definition
                List<Product> products = precalcPositions.Select(x => x.Product).ToList();
                products.Add(sourceDetail.Product);

                bool isNonMonthlyStrip = TradeCapture.IsNonMonthlyStrip(
                    products,
                    securityDefinition,
                    sourceDetail.DateType);

                if (isNonMonthlyStrip)
                {
                    var precalcSourcedetailsDetails = precalcPositions.Select(
                        x =>
                        {
                            var precalc = new PrecalcSourcedetailsDetail
                            {
                                Month = x.Month,
                                DaysPositions = x.DaysPositions,
                                ProductId = x.Product.ProductId,
                                MinDay = x.MinDay,
                                MaxDay = x.MaxDay,
                            };

                            if (sourceDetail.SourceDataId == 0)
                                precalc.SourceDetail = sourceDetail;
                            else
                                precalc.SourceDetailId = sourceDetail.SourceDetailId;

                            return precalc;
                        }).ToList();

                    ClearerPrecalcs.AddRange(precalcSourcedetailsDetails);
                }
                else
                {
                    bool processed = _processedSecurityDefinitions.Any(x => x == securityDefinition);

                    if (securityDefinition.SecurityDefinitionId != 0)
                    {
                        processed =
                            _processedSecurityDefinitions.Any(
                                x => x.SecurityDefinitionId == securityDefinition.SecurityDefinitionId);
                    }

                    if (processed)
                        return;

                    _processedSecurityDefinitions.Add(securityDefinition);

                    var precalcSdDetails = precalcPositions.Select(
                        x =>
                        {
                            var precalc = new PrecalcSdDetail
                            {
                                Month = x.Month,
                                DaysPositions = x.DaysPositions,
                                ProductId = x.Product.ProductId,
                                MinDay = x.MinDay,
                                MaxDay = x.MaxDay,
                            };

                            if (securityDefinition.SecurityDefinitionId == 0)
                                precalc.SecurityDefinition = securityDefinition;
                            else
                                precalc.SecurityDefinitionId = securityDefinition.SecurityDefinitionId;

                            return precalc;
                        }).ToList();

                    ClearerSecurityPrecalcs.AddRange(precalcSdDetails);
                }
            }
        }

        public static List<PrecalcDetail> ConvertPricingDetailsToPrecalDetails(
            List<PricingCalculationDetail> pricingDetails)
        {
            return
                pricingDetails.GroupBy(x => new { x.Period, x.ProductId })
                              .Select(grp => BuildPrecalcDetail(grp.ToList()))
                              .ToList();
        }

        private List<PricingCalculationDetail> GetPricingCalculationDetails(TradePieces trade)
        {
            SourceDetail sourceDetail = ConvertTradeCaptureToSourceDetail(trade);

            if (sourceDetail == null)
            {
                throw new NoSourceDetailException($"Could not create source detail data from ${trade}");
            }

            return GetPricingCalculationDetails(sourceDetail);
        }

        private List<PricingCalculationDetail> GetPricingCalculationDetails(SourceDetail sourceDetail)
        {
            List<PricingCalculationDetail> pricingDetails = null;
            decimal backupQuantity = sourceDetail.Quantity.Value;
            (DateTime start, DateTime end) = GetStartAndEndDate(sourceDetail);

            sourceDetail.Quantity = 1M;

            pricingDetails = sourceDetail.Product.IsProductDaily
                ? GetDailyPricingDetails(sourceDetail)
                : CalculatePricingPositions(sourceDetail, start, end);
            sourceDetail.Quantity = backupQuantity;
            return pricingDetails;
        }

        private List<PricingCalculationDetail> CalculatePricingPositions(
            SourceDetail sourceDetail,
            DateTime startDate,
            DateTime endDate)
        {
            List<SourceDetail> sourceDetails = new List<SourceDetail>() { sourceDetail };

            return _pricingPositionsManager.CalculateTasReport(startDate, endDate, sourceDetails, _calculationCache);
        }

        private List<PricingCalculationDetail> GetDailyPricingDetails(SourceDetail sourceDetail)
        {
            // TODO: Confirm that the methods called by CalculatePositions do not return null.
            return CalculatePositions(sourceDetail)?
                   .Where(position => null != position.DailyDetails)
                   .SelectMany(BuildPositionForDay)
                   .ToList() ?? new List<PricingCalculationDetail>();

            IEnumerable<PricingCalculationDetail> BuildPositionForDay(CalculationDetail position)
            {
                return position.DailyDetails.Select(
                    dayPos => new PricingCalculationDetail
                    {
                        Amount = dayPos.Amount,
                        CalculationDate = dayPos.CalculationDate.Date,
                        Product = position.Product,
                        ProductReference = position.ProductReference,
                        ProductId = position.ProductReference.ProductId,
                        Period = position.CalculationDate,
                    });
            }
        }

        private List<CalculationDetail> CalculatePositions(SourceDetail sourceDetail)
        {
            using (var productsDb = ProductsDb())
            {
                ClassicPositionsCalculator calculator = new ClassicPositionsCalculator(productsDb, _calculationCache);
                List<SourceDetail> sourceDetails = new List<SourceDetail>() { sourceDetail };

                return calculator.Calculate(sourceDetails, sourceDetail.ProductDate, false);
            }
        }

        public static SourceDetail ConvertTradeCaptureToSourceDetail(TradePieces trade)
        {
            if (trade.Strip.IsDefault())
            {
                return null;
            }

            TradeCapture capture = trade.Trade;
            SecurityDefinition secDef = trade.Security.SecurityDef;
            Product product = trade.Security.Product;

            SourceDetail sourceDetail = new SourceDetail
            {
                SourceDetailId = capture.TradeId,
                StripName = secDef.StripName,
                IsTimeSpread = capture.Strip.IsTimeSpread,
                InstrumentDescription = secDef.UnderlyingSecurityDesc,
                Product = product,
                Quantity = capture.Quantity.Value,
                TradePrice = capture.Price.Value,
                TransactTime = capture.TransactTime,
                MaturityDate = secDef.UnderlyingMaturityDateAsDate,
                TradeCapture = capture,
                PortfolioId = capture.Portfolio?.PortfolioId,
                TradeCaptureId = capture.TradeId,
                ProductId = secDef.product_id.Value,
                UseExpiryCalendar = product.UseExpiryCalendar.HasValue && product.UseExpiryCalendar.Value,
            };

            sourceDetail.ProductDate = sourceDetail.ProductDate1 = trade.Strip.Part1.StartDate;
            sourceDetail.DateType = sourceDetail.DateType1 = trade.Strip.Part1.DateType;

            if (trade.Strip.IsTimeSpread)
            {
                sourceDetail.ProductDate2 = trade.Strip.Part2.StartDate;
                sourceDetail.DateType2 = trade.Strip.Part2.DateType;
            }

            if (capture.TradeEndDate.HasValue)
            {
                sourceDetail.TradeEndDate = capture.TradeEndDate.Value;
            }

            return sourceDetail;
        }

        private (DateTime start, DateTime end) GetStartAndEndDate(SourceDetail sourceDetail)
        {
            (DateTime? tradeStartDate, DateTime? tradeEndDate) = TradeDateRange();
            ProductDateType productDateType = TradeDateType();

            switch (productDateType)
            {
                case ProductDateType.Quarter:
                {
                    return (sourceDetail.ProductDate, TradeEndDate().AddMonths(3));
                }

                case ProductDateType.Year:
                {
                    return (sourceDetail.ProductDate, TradeEndDate().AddMonths(12));
                }

                case ProductDateType.Custom:
                {
                    return (tradeStartDate.HasValue && tradeEndDate.HasValue)
                        ? (tradeStartDate.Value, tradeEndDate.Value)
                        : (sourceDetail.ProductDate, TradeEndDate());
                }

                case ProductDateType.Daily:
                {
                    return (tradeStartDate.HasValue && tradeEndDate.HasValue)
                        ? (tradeStartDate.Value.FirstDayOfMonth(), tradeEndDate.Value)
                        : (sourceDetail.ProductDate, TradeEndDate());
                }

                default:
                {
                    return (sourceDetail.ProductDate, TradeEndDate().AddMonths(1));
                }
            }

            ProductDateType TradeDateType()
            {
                return sourceDetail.IsTimeSpread
                    ? sourceDetail.DateType2
                    : sourceDetail.DateType1;
            }

            (DateTime? TradeStartDate, DateTime? TradeEndDate) TradeDateRange()
            {
                return (sourceDetail.TradeCapture != null)
                    ? (sourceDetail.TradeCapture.TradeStartDate, sourceDetail.TradeCapture.TradeEndDate)
                    : (sourceDetail.ProductDate, sourceDetail.ExpiryDate);
            }

            DateTime TradeEndDate()
            {
                return sourceDetail.IsTimeSpread ? sourceDetail.ProductDate2 : sourceDetail.ProductDate;
            }
        }

        public DateTime GetExpiryDate(Product product, int productYear, int productMonth)
        {
            DateTime expirationMonth = new DateTime(productYear, productMonth, 1);

            // trying to retrieve expiryDate from the CalculationCache
            Dictionary<DateTime, CalendarExpiryDate> calendarExpiryDates;
            _calculationCache.ExpiryDatesMap.TryGetValue(product.ExpiryCalendar.CalendarId, out calendarExpiryDates);

            CalendarExpiryDate calendarExpiryDate;
            calendarExpiryDates.TryGetValue(expirationMonth, out calendarExpiryDate);

            DateTime expiryDate = calendarExpiryDate.ExpiryDate;

            List<DateTime> holidays = new List<DateTime>();

            SortedSet<CalendarHoliday> holidaysSet;
            if (_calculationCache.CalendarHolidaysMap.TryGetValue(product.HolidaysCalendar.CalendarId, out holidaysSet))
            {
                holidays = holidaysSet.Select(it => it.HolidayDate).ToList();
            }

            int rollingSign = 1;
            bool isCorrectDay = false;

            do
            {
                isCorrectDay = true;

                if (expiryDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    expiryDate = expiryDate.AddDays(rollingSign);
                    isCorrectDay = false;
                }

                if (expiryDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    expiryDate = expiryDate.AddDays(rollingSign);
                    isCorrectDay = false;
                }

                if (holidays.Contains(expiryDate))
                {
                    expiryDate = expiryDate.AddDays(rollingSign);
                    isCorrectDay = false;
                }
            }
            while (isCorrectDay == false);

            return expiryDate;
        }
    }

    // TODO: Move to Mandara.Dates
    public static class DateRangeExtensions
    {
        public static void Deconstruct(this DateRange toDeconstruct, out DateTime start, out DateTime end)
        {
            start = toDeconstruct.Start;
            end = toDeconstruct.End;
        }
    }
}