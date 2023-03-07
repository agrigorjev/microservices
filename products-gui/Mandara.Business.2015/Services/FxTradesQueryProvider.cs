using Mandara.Business.Dates;
using Mandara.Date;
using Mandara.Entities;
using Mandara.Entities.Enums;
using System.Linq;

namespace Mandara.Business.Services
{
    public class FxTradesQueryProvider
    {
        public static IQueryable<FxTrade> ApplyDateRangeConstraints(
            DateRange dateRange,
            IEndOfDayDateTimeProvider endOfDayDateTimeProvider,
            IQueryable<FxTrade> query)
        {
            DateRange eodUtcDateRange = endOfDayDateTimeProvider.GetUtcDateRangeAccordingToEodTimes(dateRange);

            if (eodUtcDateRange.HasStartDate())
            {
                query = query.Where(tc => eodUtcDateRange.Start <= tc.TradeCapture.UtcTransactTime);
            }

            if (eodUtcDateRange.HasEndDate())
            {
                query = query.Where(tc => tc.TradeCapture.UtcTransactTime < eodUtcDateRange.End);
            }

            return query;
        }

        public static IQueryable<FxTrade> FilterForFilledTradesOnly(IQueryable<FxTrade> query)
        {
            return query.Where(fxTrade => fxTrade.TradeCapture.OrdStatus == TradeOrderStatus.Filled);
        }
    }
}