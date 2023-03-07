using Mandara.Entities;
using System;
using Mandara.Date;

namespace Mandara.Business.Data
{
    public static class FxPriceKey
    {
        public static string Create(DateTime valueDate, CurrencyPair currencyPair)
        {
            return $"{valueDate.ToSortableShortDate()}_{currencyPair}";
        }
    }
}