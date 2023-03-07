using Mandara.Business.DataInterface;
using Mandara.Entities;
using Mandara.Extensions.Option;
using System;
using System.Collections.Generic;
using Mandara.Date;
using Mandara.Date.Time;

namespace Mandara.Business.Data
{
    public class LiveFxPricesProvider : ILiveFxPricesProvider
    {
        private readonly IFxPricesDataProvider _fxPricesDataProvider;
        private readonly IFixedFxPricesDataProvider _fixedFxPricesDataProvider;
        private readonly CurrencyPairToFXRateMapper _fxRatesProvider;
        private Dictionary<string, decimal> _prices;

        public LiveFxPricesProvider(
            IFxPricesDataProvider fxPricesDataProvider,
            IFixedFxPricesDataProvider fixedFxPricesDataProvider)
        {
            _fxPricesDataProvider = fxPricesDataProvider;
            _fixedFxPricesDataProvider = fixedFxPricesDataProvider;
            _fxRatesProvider = new CurrencyPairToFXRateMapper(_fxPricesDataProvider.CurrencyProvider);

            UpdatePrices();
        }

        public void UpdatePrices()
        {
            Dictionary<string, decimal> livePrices = _fxPricesDataProvider.GetLivePrices();
#if DEBUG
            string tempFilePath = string.Format(
                "fxLivePrices_{0}_{1}.csv",
                SystemTime.Now().ToSortableShortDateTime('_'),
                System.Threading.Thread.CurrentThread.ManagedThreadId);
            TestPhasePriceDump.DumpPrices(tempFilePath, livePrices);
#endif
            _prices = new Dictionary<string, decimal>(livePrices);
            _fxRatesProvider.SetPrices(_prices);
        }

        public ILiveFxPricesProvider GetFixedLivePrices()
        {
            _fixedFxPricesDataProvider.Reset();
            return new LiveFxPricesProvider(_fixedFxPricesDataProvider, _fixedFxPricesDataProvider);
        }

        public TryGetResult<decimal> TryGetPrice(DateTime valueDate, Currency fromCurrency, Currency toCurrency)
        {
            return TryGetPrice(valueDate, fromCurrency.CurrencyId, toCurrency.CurrencyId);
        }

        public TryGetResult<decimal> TryGetPrice(DateTime valueDate, int fromCurrencyId, int toCurrencyId)
        {
            return _fxRatesProvider.TryGetPrice(valueDate, fromCurrencyId, toCurrencyId);
        }

        public TryGetResult<decimal> TryGetPrice(
            DateTime valueDate,
            string fromCurrencyIsoName,
            string toCurrencyIsoName)
        {
            return _fxRatesProvider.TryGetPrice(valueDate, fromCurrencyIsoName, toCurrencyIsoName);
        }
    }
}