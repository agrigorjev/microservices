using Mandara.Business.DataInterface;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Extensions.Option;
using System;
using System.Collections.Generic;
using Mandara.Date;

namespace Mandara.Business.Data
{
    public class SnapshotFxPricesProvider : ISnapshotFxPricesProvider
    {
        private readonly IFxPricesDataProvider _fxPricesDataProvider;
        private readonly CurrencyPairToFXRateMapper _fxRatesProvider;
        private Dictionary<string, decimal> _prices;
        private DateTime _snapshotDateTime = DateTime.MinValue;

        public SnapshotFxPricesProvider(IFxPricesDataProvider fxPricesDataProvider)
        {
            _fxPricesDataProvider = fxPricesDataProvider;
            _fxRatesProvider = new CurrencyPairToFXRateMapper(_fxPricesDataProvider.CurrencyProvider);
        }

        public void UpdatePrices(DateTime snapshotDateTime)
        {
            if (_snapshotDateTime.Equals(snapshotDateTime))
            {
                return;
            }

            Dictionary<string, decimal> snapshotPrices = _fxPricesDataProvider.GetSnapshotPrices(snapshotDateTime);

#if DEBUG
            string tempFilePathTemplate = string.Format(
                "fxSnapshotPrices_{0}_{1}_{2}.csv",
                snapshotDateTime.ToSortableShortDateTime('_'),
                SystemTime.Now().ToSortableShortDateTime(),
                System.Threading.Thread.CurrentThread.ManagedThreadId);
            TestPhasePriceDump.DumpPrices(tempFilePathTemplate, snapshotPrices);
#endif
            _prices = new Dictionary<string, decimal>(snapshotPrices);
            _fxRatesProvider.SetPrices(_prices);
        }

        public TryGetResult<decimal> TryGetPrice(DateTime valueDate, Currency fromCurrency, Currency toCurrency)
        {
            return TryGetPrice(valueDate, fromCurrency.CurrencyId, toCurrency.CurrencyId);
        }

        public TryGetResult<decimal> TryGetPrice(DateTime valueDate, int fromCurrencyId, int toCurrencyId)
        {
            return _fxRatesProvider.TryGetPrice(valueDate, fromCurrencyId, toCurrencyId);
        }

        public TryGetResult<decimal> TryGetPrice(DateTime valueDate, string fromCurrencyIsoName, string toCurrencyIsoName)
        {
            return _fxRatesProvider.TryGetPrice(valueDate, fromCurrencyIsoName, toCurrencyIsoName);
        }
    }
}