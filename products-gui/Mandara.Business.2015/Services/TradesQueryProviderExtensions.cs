using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Mandara.Entities;

namespace Mandara.Business.Services
{
    public static class TradesQueryProviderExtensions
    {
        public static IQueryable<TradeCapture> WithBooks(this IQueryable<TradeCapture> tradesQuery)
        {
            return tradesQuery.Include(trade => trade.Portfolio)
                              .Include(trade => trade.BuyBook)
                              .Include(trade => trade.SellBook);
        }
    }
}