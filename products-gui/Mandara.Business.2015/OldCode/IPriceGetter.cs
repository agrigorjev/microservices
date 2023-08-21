using System;
using Mandara.Entities;

namespace Mandara.Business.OldCode
{
    public interface IPriceGetter
    {
        decimal? GetTradePrice(TradeCapture trade);

        decimal? GetProductPrice(int productId, DateTime productDate, ProductDateType priceDateType, string mappingColumn,
                                 object tradeStartDateObject = null, object tradeEndDateObject = null);
    }
}