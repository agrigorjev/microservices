using Mandara.Entities;
using Mandara.Extensions.Option;
using System;

namespace Mandara.Business.DataInterface
{
    public interface IFxPricesProvider
    {
        TryGetResult<decimal> TryGetPrice(DateTime valueDate, Currency fromCurrency, Currency toCurrency);
        TryGetResult<decimal> TryGetPrice(DateTime valueDate, int fromCurrencyId, int toCurrencyId);
        TryGetResult<decimal> TryGetPrice(DateTime valueDate, string fromCurrencyIsoName, string toCurrencyIsoName);
    }
}