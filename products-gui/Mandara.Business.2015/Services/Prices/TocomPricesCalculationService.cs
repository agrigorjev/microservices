using Mandara.Business.Contracts;
using Mandara.Business.DataInterface;
using Mandara.Business.Fx;
using Mandara.Business.Model;
using Mandara.Date;
using Mandara.Entities;
using Mandara.Entities.Entities;
using Mandara.Entities.Enums;
using Mandara.Extensions;
using Mandara.Extensions.Option;
using Mandara.IRM.Server.Services;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.Services.Prices
{
    public class TocomPricesCalculationService : IPricesCalculationService
    {
        private readonly ILogger _log;
        private readonly IProductsStorage _productsStorage;
        private readonly ISettlementPricesStorage _settlementPricesStorage;
        private readonly ILivePricesProvider _originalLivePricesProvider;
        private MonthlyBusinessDayCalculator _businessDaysCalculator;
        private readonly IFxOfficialProductPnLMapDataProvider _fxOfficialProdPnLMapDataProvider;
        private readonly CurrencyIdToFXOfficialProductMapper _currencyIdOfficialProductMapper;
        private static ICurrencyProvider _currencyProvider;
        public const string UsdCurrencyIsoName = CurrencyCodes.USD;
        public const string ForceLivePriceWithSettlementOverride = "SettlementOverride";
        private bool _useSettlementForLive = false;
        private bool _thisInstanceHasInitialised = false;
        private Currency _usdCurrency;

        private enum GetPriceSelection
        {
            Live = 1,
            Fx = 2,
            Settlement = 4,
        }

        private class FunctionGetPrice
        {
            public Func<int, int, PriceIdentificationDates, IPricesProvider, Money> GetPrice { get; private set; }

            public FunctionGetPrice(Func<int, int, PriceIdentificationDates, IPricesProvider, Money> getPrice)
            {
                GetPrice = getPrice ?? throw new ArgumentNullException("The GetPrice function cannot be null.");
            }
        }

        private Dictionary<int, FunctionGetPrice> _priceProviderFuncsByPriceOptions =
            new Dictionary<int, FunctionGetPrice>();

        private class PriceIdentificationDates
        {
            public DateTime RiskDay { get; set; }
            public DateTime StartDate { get; set; }

            public PriceIdentificationDates(DateTime riskDay, DateTime start)
            {
                RiskDay = riskDay;
                StartDate = start;
            }
        }

        public TocomPricesCalculationService(
            ILogger log,
            IProductsStorage productsStorage,
            ISettlementPricesStorage settlementPricesStorage,
            ILivePricesProvider livePricesProvider,
            IFxOfficialProductPnLMapDataProvider fxOfficialProdPnlMapDataProvider,
            CurrencyIdToFXOfficialProductMapper currencyIdOfficialProdMapper,
            ICurrencyProvider currencyProvider)
        {
            _log = log;
            _productsStorage = productsStorage;
            _settlementPricesStorage = settlementPricesStorage;
            _originalLivePricesProvider = livePricesProvider;
            _fxOfficialProdPnLMapDataProvider = fxOfficialProdPnlMapDataProvider;
            _currencyIdOfficialProductMapper = currencyIdOfficialProdMapper;
            _currencyProvider = currencyProvider;
            InitialisePriceProviderFunctions();
        }

        private void InitialisePriceProviderFunctions()
        {
            _priceProviderFuncsByPriceOptions.Add(
                GetPriceSelectionFlag(GetPriceSelection.Live),
                new FunctionGetPrice(GetLivePrice));
            _priceProviderFuncsByPriceOptions.Add(
                GetPriceSelectionFlag(GetPriceSelection.Settlement),
                new FunctionGetPrice(DummyGetSettlementPriceForLivePrice));
            _priceProviderFuncsByPriceOptions.Add(
                GetPriceSelectionFlag(GetPriceSelection.Live, GetPriceSelection.Fx),
                new FunctionGetPrice(GetFxLivePrice));
        }

        private int GetPriceSelectionFlag(params GetPriceSelection[] priceSelection)
        {
            if (null == priceSelection)
            {
                return (int)GetPriceSelection.Live;
            }

            return priceSelection.Aggregate(
                0,
                (priceSelectionFlag, selectionOption) => priceSelectionFlag | (int)selectionOption);
        }

        public bool TryCalculateLivePrice(
            TradeCapture trade,
            DateTime riskDate,
            IPricesProvider livePricesProvider,
            out LivePrice livePrice,
            out TradePrice tradePrice)
        {
            Initialise();

            if (livePricesProvider == null)
            {
                livePricesProvider = _originalLivePricesProvider;
            }

            _businessDaysCalculator = new MonthlyBusinessDayCalculator(_productsStorage);
            SetSettlementForLiveOption(trade);

            SecurityDefinition securityDefinition = trade.SecurityDefinition;
            Product product = securityDefinition.Product;
            OfficialProduct officialProduct = product.OfficialProduct;
            TryGetResult<string> isoName = GetCurrencyName(officialProduct);

            if (!isoName.HasValue)
            {
                tradePrice = new TradePrice();
                livePrice = new LivePrice();
                return false;
            }

            tradePrice = new TradePrice()
            {
                TradedPrice = new Money(trade.Price.Value, isoName.Value)
            };

            livePrice = new LivePrice();

            if (!trade.TradeStartDate.HasValue)
            {
                _log.Error(
                    "Live price could not be calculated for trade, no start day present. Trade ID: {0}",
                    trade.TradeId);
                return false;
            }

            int calendarId = product.holidays_calendar_id.Value;
            FunctionGetPrice defaultGetPriceFuncs = GetPriceProvider(GetPriceSelectionFlag(GetPriceSelection.Live));
            PriceIdentificationDates defaultPriceDates = new PriceIdentificationDates(
                riskDate,
                trade.Strip.Part1.StartDate);

            if (trade.TradeStartDate > riskDate)
            {
                defaultPriceDates.RiskDay = trade.TradeStartDate.Value;
                livePrice.TradeLivePrice = defaultGetPriceFuncs.GetPrice(
                    calendarId,
                    officialProduct.OfficialProductId,
                    defaultPriceDates,
                    livePricesProvider);
                return true;
            }

            List<int> officialProductIds = new List<int>();
            int settlementOfficialProductId =
                officialProduct.SettlementProductId ?? officialProduct.OfficialProductId;

            officialProductIds.Add(settlementOfficialProductId);

            FxOfficialProductPnLMap currencyToOfficialProdIdMap =
                _currencyIdOfficialProductMapper.GetOfficialProductAndHolidayCalendarIDsForCurrency(
                    officialProduct.CurrencyId);

            officialProductIds.Add(currencyToOfficialProdIdMap.OfficialProductId);

            TryGetResult<Dictionary<int, decimal>> totalSettles = GetTotalSettlementsForMonth(
                calendarId,
                officialProductIds,
                defaultPriceDates);

            if (!totalSettles.HasValue)
            {
                livePrice.TradeLivePrice = defaultGetPriceFuncs.GetPrice(
                    calendarId,
                    officialProduct.OfficialProductId,
                    new PriceIdentificationDates(trade.TradeStartDate.Value, defaultPriceDates.StartDate),
                    livePricesProvider);
                return true;
            }

            try
            {
                decimal partialProductPrice = GetPartialFullmonth(
                    calendarId,
                    settlementOfficialProductId,
                    defaultPriceDates,
                    livePricesProvider,
                    GetPriceProvider(GetPriceSelectionFlag(GetPriceSelection.Live)));

                decimal partialFxPrice = GetPartialFullmonth(
                    currencyToOfficialProdIdMap.HolidayCalendarId,
                    currencyToOfficialProdIdMap.OfficialProductId,
                    defaultPriceDates,
                    livePricesProvider,
                    GetPriceProvider(GetPriceSelectionFlag(GetPriceSelection.Live, GetPriceSelection.Fx)));

                decimal positionFactor = product.PositionFactor.HasValue ? product.PositionFactor.Value : 1;

                livePrice.TradeLivePrice = new Money(
                    partialProductPrice * partialFxPrice * positionFactor,
                    isoName.Value);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error calculating live price for trade {0}", trade.TradeId);
                return false;
            }

            return true;
        }

        public PriceCalcResult TryCalculateLivePrice(
            TradeCapture trade,
            DateTime riskDate,
            IPricesProvider livePricesProvider)
        {
            Initialise();

            if (livePricesProvider == null)
            {
                livePricesProvider = _originalLivePricesProvider;
            }

            _businessDaysCalculator = new MonthlyBusinessDayCalculator(_productsStorage);
            SetSettlementForLiveOption(trade);

            SecurityDefinition securityDefinition = trade.SecurityDefinition;
            Product product = securityDefinition.Product;
            OfficialProduct officialProduct = product.OfficialProduct;
            TryGetResult<string> isoName = GetCurrencyName(officialProduct);
            PriceCalcResult price = PriceCalcResult.Default;

            if (!isoName.HasValue)
            {
                return price;
            }

            price.Trade = new TradePrice()
            {
                TradedPrice = new Money(trade.Price.Value, isoName.Value)
            };

            price.Live = new LivePrice();
            price.TradeId = trade.TradeId;

            if (!trade.TradeStartDate.HasValue)
            {
                _log.Error(
                    "Live price could not be calculated for trade, no start day present. Trade ID: {0}",
                    trade.TradeId);
                return price;
            }

            int calendarId = product.holidays_calendar_id.Value;
            FunctionGetPrice defaultGetPriceFuncs = GetPriceProvider(GetPriceSelectionFlag(GetPriceSelection.Live));
            PriceIdentificationDates defaultPriceDates = new PriceIdentificationDates(
                riskDate,
                trade.Strip.Part1.StartDate);

            if (trade.TradeStartDate > riskDate)
            {
                defaultPriceDates.RiskDay = trade.TradeStartDate.Value;
                price.Live.TradeLivePrice = defaultGetPriceFuncs.GetPrice(
                    calendarId,
                    officialProduct.OfficialProductId,
                    defaultPriceDates,
                    livePricesProvider);
                return price;
            }

            List<int> officialProductIds = new List<int>();
            int settlementOfficialProductId =
                officialProduct.SettlementProductId ?? officialProduct.OfficialProductId;
            int officialProductCurrencyId = officialProduct.CurrencyId;
            FxOfficialProductPnLMap currencyToOfficialProdIdMap = _currencyIdOfficialProductMapper
                .GetOfficialProductAndHolidayCalendarIDsForCurrency(officialProductCurrencyId);

            officialProductIds.Add(settlementOfficialProductId);
            officialProductIds.Add(currencyToOfficialProdIdMap.OfficialProductId);

            TryGetResult<Dictionary<int, decimal>> totalSettles = GetTotalSettlementsForMonth(
                calendarId,
                officialProductIds,
                defaultPriceDates);

            if (!totalSettles.HasValue)
            {
                price.Live.TradeLivePrice = defaultGetPriceFuncs.GetPrice(
                    calendarId,
                    officialProduct.OfficialProductId,
                    new PriceIdentificationDates(trade.TradeStartDate.Value, defaultPriceDates.StartDate),
                    livePricesProvider);
                price.UsedSettle = false;
                return price;
            }

            try
            {
                price.UsedSettle = true;
                price.Settles = totalSettles.Value;

                price.PartialPrice = GetPartialFullmonth(
                    calendarId,
                    settlementOfficialProductId,
                    defaultPriceDates,
                    livePricesProvider,
                    GetPriceProvider(GetPriceSelectionFlag(GetPriceSelection.Live)));

                price.PartialFxPrice = GetPartialFullmonth(
                    currencyToOfficialProdIdMap.HolidayCalendarId,
                    currencyToOfficialProdIdMap.OfficialProductId,
                    defaultPriceDates,
                    livePricesProvider,
                    GetPriceProvider(GetPriceSelectionFlag(GetPriceSelection.Live, GetPriceSelection.Fx)));

                decimal positionFactor = product.PositionFactor ?? 1;

                price.Live.TradeLivePrice = new Money(
                    price.PartialPrice * price.PartialFxPrice * positionFactor,
                    isoName.Value);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error calculating live price for trade {0}", trade.TradeId);
                return price;
            }

            return price;
        }

        private void SetSettlementForLiveOption(TradeCapture trade)
        {
            if (trade.OrigTradeID == ForceLivePriceWithSettlementOverride)
            {
                _useSettlementForLive = true;
            }
            else
            {
                _useSettlementForLive = false;
            }
        }

        private FunctionGetPrice GetPriceProvider(int priceSelectionOptions)
        {
            if (_useSettlementForLive)
            {
                return _priceProviderFuncsByPriceOptions[GetPriceSelectionFlag(GetPriceSelection.Settlement)];
            }

            return _priceProviderFuncsByPriceOptions[priceSelectionOptions];
        }

        private TryGetRef<string> GetCurrencyName(OfficialProduct officialProduct)
        {
            string isoName = null;

            if (null != officialProduct && null != officialProduct.Currency)
            {
                isoName = officialProduct.Currency.IsoName;
            }
            else
            {
                _log.Error(
                    "TocomPricesCalculationService: Error geting currency for officialProduct, currency defaulted to "
                        + "JPY by Tocom calculator");
            }

            return new TryGetRef<string>(isoName, String.IsNullOrWhiteSpace);
        }

        private TryGetResult<Dictionary<int, decimal>> GetTotalSettlementsForMonth(
            int calendarId,
            List<int> officialProductIds,
            PriceIdentificationDates priceDates)
        {
            Dictionary<int, decimal> sumSettles = new Dictionary<int, decimal>();

            foreach (int officialProductId in officialProductIds)
            {
                PriceIdentificationDates officialProdPriceDates =
                    new PriceIdentificationDates(
                        new DateTime(
                            priceDates.RiskDay.Year,
                            priceDates.RiskDay.Month,
                            DateTime.DaysInMonth(priceDates.RiskDay.Year, priceDates.RiskDay.Month)),
                        priceDates.StartDate);

                sumSettles[officialProductId] = SumCurrentMonthHistoricSettlements(
                    calendarId,
                    officialProductId,
                    officialProdPriceDates);
            }

            return new TryGetRef<Dictionary<int, decimal>>(
                sumSettles,
                val => val.All(offProdSettle => offProdSettle.Value == 0));
        }

        private decimal GetPartialFullmonth(
            int calendarId,
            int officialProductId,
            PriceIdentificationDates priceDates,
            IPricesProvider livePricesProvider,
            FunctionGetPrice getPriceFuncs)
        {
            decimal sumSettles = SumCurrentMonthHistoricSettlements(calendarId, officialProductId, priceDates);
            decimal todayPrice = GetTodaysPrice(
                calendarId,
                officialProductId,
                priceDates,
                livePricesProvider,
                getPriceFuncs);
            decimal sumPriceRemainder = GetSumRemainingPrices(
                calendarId,
                officialProductId,
                priceDates,
                livePricesProvider,
                getPriceFuncs);

            decimal workingDays = _businessDaysCalculator.GetNumberBusinessDaysInMonth(calendarId, priceDates.RiskDay);
            decimal partialFullmonth = (sumSettles + todayPrice + sumPriceRemainder) / workingDays;

            return partialFullmonth;
        }

        private decimal GetSumRemainingPrices(
            int calendarId,
            int officialProductId,
            PriceIdentificationDates priceDates,
            IPricesProvider livePricesProvider,
            FunctionGetPrice getPriceFuncs)
        {
            int numRemainingDays = _businessDaysCalculator.GetRemainingBusinessDaysInMonth(
                calendarId,
                priceDates.RiskDay);
            decimal livePrice =
                getPriceFuncs.GetPrice(calendarId, officialProductId, priceDates, livePricesProvider).Amount;

            return livePrice * numRemainingDays;
        }

        private decimal GetTodaysPrice(
            int calendarId,
            int officialProductId,
            PriceIdentificationDates priceDates,
            IPricesProvider livePricesProvider,
            FunctionGetPrice getPriceFuncs)
        {
            if (_businessDaysCalculator.IsTodayHoliday(priceDates.RiskDay, calendarId))
            {
                return 0M;
            }

            Money settleOnDay;
            bool settlePresent = _settlementPricesStorage.TryGetPrice(
                officialProductId,
                priceDates.RiskDay,
                priceDates.StartDate,
                out settleOnDay);

            if (settlePresent)
            {
                return settleOnDay.Amount;
            }

            return getPriceFuncs.GetPrice(calendarId, officialProductId, priceDates, livePricesProvider).Amount;
        }

        private Money GetLivePrice(
            int calendarId,
            int officialProductId,
            PriceIdentificationDates priceDates,
            IPricesProvider livePricesProvider)
        {
            ProductDateType monthlyProduct = ProductDateType.MonthYear;
            // TODO: What happens if the official product is not in the storage?
            OfficialProduct officialProduct = _productsStorage.TryGetOfficialProduct(officialProductId).Value;
            string mappingColumn = officialProduct.MappingColumn;

            TryGetResult<Money> livePrice = livePricesProvider.TryGetProductPrice(
                0,
                priceDates.RiskDay,
                monthlyProduct,
                mappingColumn,
                officialProductId);

            if (livePrice.HasValue)
            {
                return livePrice.Value;
            }

            return Money.CurrencyDefault(officialProduct.Currency.IsoName);
        }

        private Money GetFxLivePrice(
            int calendarId,
            int officialProductId,
            PriceIdentificationDates priceDates,
            IPricesProvider livePricesProvider)
        {
            // TODO: What happens if the official product is not in the storage?
            OfficialProduct officialProduct = _productsStorage.TryGetOfficialProduct(officialProductId).Value;
            Currency currency = officialProduct.Currency;
            TryGetResult<decimal> fxPrice = livePricesProvider.TryGetFxPrice(priceDates.RiskDay, currency, _usdCurrency);

            if (fxPrice.HasValue)
            {
                return new Money(fxPrice.Value, currency.IsoName);
            }

            return Money.CurrencyDefault(currency.IsoName);
        }

        private Money DummyGetSettlementPriceForLivePrice(
            int calendarId,
            int officialProductId,
            PriceIdentificationDates priceDates,
            IPricesProvider livePrices)
        {
            return GetSettlementReplacementForLivePrice(calendarId, officialProductId, priceDates);
        }

        private Money GetSettlementReplacementForLivePrice(
            int calendarId,
            int officialProductId,
            PriceIdentificationDates priceDates)
        {
            TryGetResult<Money> settleOnDay;
            Currency currency = _productsStorage.TryGetOfficialProduct(officialProductId).Value.Currency;
            Money resultSettlement = Money.CurrencyDefault(currency.IsoName);

            settleOnDay = SearchSettlementsFromTodayBack(
                calendarId,
                officialProductId,
                priceDates,
                currency);

            if (settleOnDay.HasValue)
            {
                return settleOnDay.Value;
            }

            settleOnDay = SearchSettlementsFromTomorrowForward(
                calendarId,
                officialProductId,
                priceDates,
                currency);

            if (settleOnDay.HasValue)
            {
                return settleOnDay.Value;
            }

            return resultSettlement;
        }

        private TryGetResult<Money> SearchSettlementsFromTomorrowForward(
            int calendarId,
            int officialProductId,
            PriceIdentificationDates priceDates,
            Currency currency)
        {
            DateTime lastDayInMonth = priceDates.RiskDay.LastDayOfMonth();
            IEnumerable<DateTime> busDaysToEndOfMonth = GetBusinessDays(
                priceDates.RiskDay.Date,
                lastDayInMonth,
                EnumerateDateRange.Range,
                calendarId);

            return GetFirstSettlementPrice(
                busDaysToEndOfMonth,
                officialProductId,
                currency.IsoName,
                priceDates.StartDate);
        }

        private IEnumerable<DateTime> GetBusinessDays(
            DateTime start,
            DateTime end,
            Func<DateTime, DateTime, IEnumerable<DateTime>> dateRangeConstructor,
            int calendarId)
        {
            return
                dateRangeConstructor(start, end)
                    .Where(currentDay => !_businessDaysCalculator.IsTodayHoliday(currentDay, calendarId));
        }

        private TryGetResult<Money> GetFirstSettlementPrice(
            IEnumerable<DateTime> businessDays,
            int officialProductId,
            string currencyIsoName,
            DateTime tradeStartDate)
        {
            Money settleOnDay = Money.CurrencyDefault(currencyIsoName);
            DateTime firstSettlementDate = businessDays
                .FirstOrDefault(
                    currDay =>
                        _settlementPricesStorage.TryGetPrice(
                            officialProductId,
                            currDay,
                            tradeStartDate,
                            out settleOnDay));

            return new TryGetVal<Money>(settleOnDay, val => firstSettlementDate == DateTime.MinValue);
        }

        private TryGetResult<Money> SearchSettlementsFromTodayBack(
            int calendarId,
            int officialProductId,
            PriceIdentificationDates priceDates,
            Currency currency)
        {
            IEnumerable<DateTime> busDaysBackToFirstOfMonth = GetBusinessDays(
                priceDates.RiskDay.Date,
                priceDates.RiskDay.FirstDayOfMonth(),
                EnumerateDateRange.ReverseRange,
                calendarId);

            return GetFirstSettlementPrice(
                busDaysBackToFirstOfMonth,
                officialProductId,
                currency.IsoName,
                priceDates.StartDate);
        }

        private decimal SumCurrentMonthHistoricSettlements(
            int calendarId,
            int officialProductId,
            PriceIdentificationDates priceDates)
        {
            IEnumerable<DateTime> monthBusDaysTilYesterday = GetBusinessDays(
                priceDates.RiskDay.FirstDayOfMonth(),
                priceDates.RiskDay.Date,
                EnumerateDateRange.ExclusiveRange,
                calendarId);
            decimal sumOfSettlements = monthBusDaysTilYesterday.Aggregate(
                0M,
                (settleSum, currentDay) =>
                    AddSettlementAmountForDay(settleSum, currentDay, priceDates.StartDate, officialProductId));

            return sumOfSettlements;
        }

        private decimal AddSettlementAmountForDay(
            decimal initialAmount,
            DateTime settlementDate,
            DateTime tradeStartDate,
            int officialProductId)
        {
            Money settleOnDay;
            decimal total = initialAmount;

            if (_settlementPricesStorage.TryGetPrice(
                officialProductId,
                settlementDate,
                tradeStartDate,
                out settleOnDay))
            {
                total += settleOnDay.Amount;
            }

            return total;
        }

        public void Initialise()
        {
            if (!_thisInstanceHasInitialised)
            {
                _currencyIdOfficialProductMapper.ResetData(
                    _fxOfficialProdPnLMapDataProvider.GetFxOfficialProductPnLMaps());
                GetUsdCurrency();
                _settlementPricesStorage.Update();
                _thisInstanceHasInitialised = true;
            }
        }

        private void GetUsdCurrency()
        {
            TryGetResult<Currency> currencyResult = _currencyProvider.TryGetCurrency(UsdCurrencyIsoName);

            if (!currencyResult.HasValue)
            {
                currencyResult = UpdateCurrencyProviderAndGetUsdCurrency();
            }

            _usdCurrency = currencyResult.Value;
        }

        private static TryGetResult<Currency> UpdateCurrencyProviderAndGetUsdCurrency()
        {
            _currencyProvider.Update(new List<int>());

            TryGetResult<Currency> currencyResult = _currencyProvider.TryGetCurrency(UsdCurrencyIsoName);

            if (!currencyResult.HasValue)
            {
                throw new InvalidOperationException(
                    "TocomPricesCalculationService: Attempting to initialise with no USD currency data in the "
                        + "database.");
            }

            return currencyResult;
        }
    }
}
