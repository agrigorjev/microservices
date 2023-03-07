using Mandara.Entities;
using Mandara.Extensions.Option;
using System.Collections.Generic;

namespace Mandara.Business.DataInterface
{
    public interface ICurrencyProvider
    {
        void Update(IEnumerable<int> externallyKnownCurrencyPairIds);
        TryGetResult<CurrencyPair> TryGetCurrencyPair(int currencyPairId);
        TryGetResult<CurrencyPair> TryGetCurrencyPair(int currency1Id, int currency2Id);
        TryGetResult<CurrencyPair> TryGetCurrencyPair(string currency1IsoName, string currency2IsoName);
        TryGetResult<Currency> TryGetCurrency(int currencyId);
        TryGetResult<Currency> TryGetCurrency(int currencyId, MandaraEntities dbContext);
        TryGetResult<Currency> TryGetCurrency(string currencyIsoName);
        IEnumerable<Currency> Currencies();
    }
}
