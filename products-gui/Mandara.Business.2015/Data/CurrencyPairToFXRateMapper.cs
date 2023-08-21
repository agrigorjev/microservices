using Mandara.Business.DataInterface;
using Mandara.Entities;
using Mandara.Extensions.Option;
using System;
using System.Collections.Generic;

namespace Mandara.Business.Data
{
    internal class CurrencyPairToFXRateMapper
    {
        private readonly ICurrencyProvider _currencyProvider;
        private Dictionary<string, decimal> _fxRatesByCurrencyPairName = new Dictionary<string, decimal>();

        public CurrencyPairToFXRateMapper(ICurrencyProvider currProvider)
        {
            _currencyProvider = currProvider;
        }

        public void SetPrices(Dictionary<string, decimal> fxRatesByCurrPairName)
        {
            _fxRatesByCurrencyPairName = new Dictionary<string, decimal>(fxRatesByCurrPairName);
        }

        public TryGetResult<decimal> TryGetPrice(DateTime valueDate, Currency currency1, Currency currency2)
        {
            return TryGetPrice(valueDate, currency1.CurrencyId, currency2.CurrencyId);
        }

        /// <summary>
        /// A given pair of currency IDs are checked against known currency pairs to try to get a CurrencyPair instance
        /// as well as used to constructed a function that takes a currency pair and returns whether the order of the
        /// IDs matches the currency pair or is inverted.  An inverted pair means the exchange rate should be inverted
        /// before being passed back.
        /// </summary>
        /// <param name="valueDate"></param>
        /// <param name="fromCurrencyId"></param>
        /// <param name="toCurrencyId"></param>
        /// <returns></returns>
        public TryGetResult<decimal> TryGetPrice(DateTime valueDate, int fromCurrencyId, int toCurrencyId)
        {
            Func<CurrencyPair, bool> isInvertedCurrencyPair = IsInvertedCurrencyPair(fromCurrencyId, toCurrencyId);
            TryGetResult<CurrencyPair> currencyPairResult = _currencyProvider.TryGetCurrencyPair(
                fromCurrencyId,
                toCurrencyId);

            return TryGetPrice(valueDate, currencyPairResult, isInvertedCurrencyPair);
        }

        private Func<CurrencyPair, bool> IsInvertedCurrencyPair(int fromCurrencyId, int toCurrencyId)
        {
            return (currencyPair) => currencyPair.IsInvertedPair(fromCurrencyId, toCurrencyId);
        }

        private decimal PostProcessInvertedRate(decimal rate)
        {
            return rate == 0 ? rate : 1 / rate;
        }

        /// <summary>
        /// The execution is the same as for the overload that receives the currency IDs, but using the currency names
        /// to find the currency pair and construct the function that determines whether the pair in the given order is
        /// inverted when compared to the known currency pair.
        /// </summary>
        /// <param name="valueDate"></param>
        /// <param name="fromCurrencyIsoName"></param>
        /// <param name="toCurrencyIsoName"></param>
        /// <returns></returns>
        public TryGetResult<decimal> TryGetPrice(
            DateTime valueDate,
            string fromCurrencyIsoName,
            string toCurrencyIsoName)
        {
            Func<CurrencyPair, bool> isInvertedCurrencyPair = IsInvertedCurrencyPair(
                fromCurrencyIsoName,
                toCurrencyIsoName);
            TryGetResult<CurrencyPair> currencyPairResult = _currencyProvider.TryGetCurrencyPair(
                fromCurrencyIsoName,
                toCurrencyIsoName);

            return TryGetPrice(valueDate, currencyPairResult, isInvertedCurrencyPair);
        }

        private Func<CurrencyPair, bool> IsInvertedCurrencyPair(string fromCurrencyIsoName, string toCurrencyIsoName)
        {
            return (currencyPair) => currencyPair.IsInvertedPair(fromCurrencyIsoName, toCurrencyIsoName);
        }

        /// <summary>
        /// If the currency pair result contains a valid currency pair, the isInvertedCurrencyPair function is used to
        /// determine whether the currency information used to find the CurrencyPair (IDs or ISO names) is inverted
        /// when compared to the CurrencyPair.  If inverted then the price post-processing function used is 
        /// PostProcessInvertedRate which returns 1/rate if the rate is not 0.0.  Otherwise the post-processing does
        /// nothing to the rate.
        /// </summary>
        /// <param name="valueDate"></param>
        /// <param name="currencyPairResult"></param>
        /// <param name="isInvertedCurrencyPair"></param>
        /// <returns></returns>
        private TryGetResult<decimal> TryGetPrice(
            DateTime valueDate,
            TryGetResult<CurrencyPair> currencyPairResult,
            Func<CurrencyPair, bool> isInvertedCurrencyPair)
        {
            if (!currencyPairResult.HasValue)
            {
                return new TryGetVal<decimal>((val) => true) { Value = Decimal.Zero };
            }

            TryGetResult<decimal> price;

            if (isInvertedCurrencyPair(currencyPairResult.Value))
            {
                price = TryGetPrice(valueDate, currencyPairResult.Value, PostProcessInvertedRate);
            }
            else
            {
                price = TryGetPrice(valueDate, currencyPairResult.Value, (value) => value);
            }

            return price;
        }

        /// <summary>
        /// Use the given CurrencyPair to check the loaded price data for a matching price.  Apply the given post-
        /// processing function to the price found.  Because decimal is a value type it's guaranteed to have a value,
        /// thought that value might be 0.0.  The post-processing function is assumed to handle that case correctly.
        /// </summary>
        /// <param name="valueDate"></param>
        /// <param name="currencyPair"></param>
        /// <param name="pricePostProcess"></param>
        /// <returns></returns>
        private TryGetResult<decimal> TryGetPrice(
            DateTime valueDate,
            CurrencyPair currencyPair,
            Func<decimal, decimal> pricePostProcess)
        {
            string fxKey = FxPriceKey.Create(
                valueDate,
                currencyPair);

            decimal price;
            bool success = _fxRatesByCurrencyPairName.TryGetValue(fxKey, out price);

            return new TryGetVal<decimal>((val) => !success || val == 0.0M) { Value = pricePostProcess(price) };
        }

    }
}
