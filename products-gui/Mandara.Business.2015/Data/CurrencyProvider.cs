using Mandara.Business.DataInterface;
using Mandara.Entities;
using Mandara.Entities.Extensions;
using Mandara.Extensions.Option;
using Optional;
using Optional.Unsafe;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Mandara.Extensions.Collections;
using Optional.Collections;

namespace Mandara.Business.Data
{
    public class CurrencyProvider : ReplaceableDbContext, ICurrencyProvider
    {
        private static Dictionary<int, CurrencyPair> _currencyPairsCache = new Dictionary<int, CurrencyPair>();
        private static Dictionary<int, Currency> _currencyById = new Dictionary<int, Currency>();
        private static Dictionary<string, Currency> _currencyByIsoName = new Dictionary<string, Currency>();

        public void Update(IEnumerable<int> externallyKnownCurrencyPairIds)
        {
            if (ShouldUpdateCurrencyData(externallyKnownCurrencyPairIds))
            {
                UpdateCurrencyCaches();
            }
        }

        private bool ShouldUpdateCurrencyData(IEnumerable<int> externallyKnownCurrencyPairIds)
        {
            return !externallyKnownCurrencyPairIds.Any()
                   || externallyKnownCurrencyPairIds.Except(_currencyPairsCache.Keys).Any();
        }

        private void UpdateCurrencyCaches()
        {
            using (MandaraEntities dbContext = _dbContextCreator())
            {
                UpdateCurrencyCaches(dbContext);
            }
        }

        private void UpdateCurrencyCaches(MandaraEntities dbContext)
        {
            UpdateCurrencyPairs(dbContext);
            UpdateCurrencies(dbContext);
        }

        private void UpdateCurrencyPairs(MandaraEntities dbContext)
        {
            _currencyPairsCache =
                dbContext.FxPairs.Include(x => x.FromCurrency)
                   .Include(x => x.ToCurrency)
                   .ToDictionary(
                       fxPair => fxPair.FxPairId,
                       fxPair => new CurrencyPair(fxPair.FxPairId, fxPair.FromCurrency, fxPair.ToCurrency));
        }

        private void UpdateCurrencies(MandaraEntities cxt)
        {
            IEnumerable<Currency> loadedCurrencies = cxt.Currencies;

            _currencyById = new Dictionary<int, Currency>();
            _currencyByIsoName = new Dictionary<string, Currency>();

            loadedCurrencies.ForEach(
                curr =>
                {
                    _currencyById.TryAdd(curr.CurrencyId, curr);
                    _currencyByIsoName.TryAdd(curr.IsoName, curr);
                });
        }
        public Option<CurrencyPair> GetCurrencyPair(int pairId)
        {
            bool success = _currencyPairsCache.TryGetValue(pairId, out CurrencyPair currencies);

            return (success && null != currencies) ? Option.Some(currencies) : Option.None<CurrencyPair>();
        }

        [Obsolete]
        public TryGetResult<CurrencyPair> TryGetCurrencyPair(int pairId)
        {
            Option<CurrencyPair> currencies = GetCurrencyPair(pairId);

            return new TryGetRef<CurrencyPair>(currencies.ValueOrDefault(), (curr) => !currencies.HasValue);
        }

        public Option<CurrencyPair> GetCurrencyPair(int fromCurrId, int toCurrId)
        {
            return GetCurrencyPair(currencies => currencies.Value.Equals(fromCurrId, toCurrId));
        }

        public Option<CurrencyPair> GetCurrencyPair(Func<KeyValuePair<int, CurrencyPair>, bool> currencyFilter)
        {
            KeyValuePair<int, CurrencyPair> currPairFromCache = _currencyPairsCache
                                                                .FirstOrNone(currencyFilter)
                                                                .ValueOr(
                                                                    new KeyValuePair<int, CurrencyPair>(
                                                                        0,
                                                                        CurrencyPair.Default));

            return ConstructCurrencyPairResult(currPairFromCache.Value);
        }

        [Obsolete]
        public TryGetResult<CurrencyPair> TryGetCurrencyPair(int fromCurrId, int toCurrId)
        {
            Option<CurrencyPair> currencies = GetCurrencyPair(fromCurrId, toCurrId);

            return new TryGetRef<CurrencyPair>(currencies.ValueOrDefault(), (currPair) => !currencies.HasValue);
        }

        private Option<CurrencyPair> ConstructCurrencyPairResult(CurrencyPair currPair)
        {
            return currPair.IsDefault() ? Option.Some(currPair) : Option.None<CurrencyPair>();
        }

        public Option<CurrencyPair> GetCurrencyPair(string fromIsoName, string toIsoName)
        {
            return GetCurrencyPair(currencies => currencies.Value.Equals(fromIsoName, toIsoName));
        }

        [Obsolete]
        public TryGetResult<CurrencyPair> TryGetCurrencyPair(string isoName1, string isoName2)
        {
            Option<CurrencyPair> currencies = GetCurrencyPair(isoName1, isoName2);

            return new TryGetRef<CurrencyPair>(currencies.ValueOrDefault(), (curr) => !currencies.HasValue);
        }

        public Option<Currency> GetCurrency(int id)
        {
            bool success = _currencyById.TryGetValue(id, out Currency currency);

            if (!success)
            {
                UpdateCurrencies(_dbContextCreator());

                success = _currencyById.TryGetValue(id, out currency);
            }

            return (success && null != currency) ? Option.Some(currency) : Option.None<Currency>();
        }

        [Obsolete]
        public TryGetResult<Currency> TryGetCurrency(int id)
        {
            Option<Currency> currency = GetCurrency(id);

            return new TryGetRef<Currency>(currency.ValueOrDefault(), (curr) => !currency.HasValue);
        }

        public Option<Currency> GetCurrency(int id, MandaraEntities dbContext)
        {
            Option<Currency> currency = GetCurrency(id);

            if (currency.HasValue)
            {
                return currency;
            }

            UpdateCurrencyCaches(dbContext);
            return GetCurrency(id);
        }

        [Obsolete]
        public TryGetResult<Currency> TryGetCurrency(int id, MandaraEntities dbContext)
        {
            Option<Currency> currency = GetCurrency(id);

            return new TryGetRef<Currency>(currency.ValueOrDefault(), (curr) => !currency.HasValue);
        }

        public Option<Currency> GetCurrency(string isoName)
        {
            bool success = _currencyByIsoName.TryGetValue(isoName, out Currency currency);

            if (!success)
            {
                UpdateCurrencies(_dbContextCreator());

                success = _currencyByIsoName.TryGetValue(isoName, out currency);
            }

            return (success && null != currency) ? Option.Some(currency) : Option.None<Currency>();
        }

        [Obsolete]
        public TryGetResult<Currency> TryGetCurrency(string isoName)
        {
            Option<Currency> currency = GetCurrency(isoName);

            return new TryGetRef<Currency>(currency.ValueOrDefault(), (curr) => !currency.HasValue);
        }

        public Option<Currency> GetFirstAvailableCurrency()
        {
            return _currencyById.Any() ? Option.Some(_currencyById.First().Value) : Option.None<Currency>();
        }

        public IEnumerable<Currency> Currencies()
        {
            foreach (Currency curr in _currencyById.Values)
            {
                yield return curr;
            }
        }
    }
}
