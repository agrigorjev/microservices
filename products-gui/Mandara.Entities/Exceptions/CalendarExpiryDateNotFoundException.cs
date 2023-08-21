using System;
using Mandara.Date;

namespace Mandara.Entities.Exceptions
{
    public class CalendarExpiryDateNotFoundException : ApplicationException
    {
        public CalendarExpiryDateNotFoundException(SourceDetail trade, StockCalendar calendar, DateTime expirationMonth)
            : base(ExpiryNotInCalendar(
                    calendar ?? StockCalendar.Default,
                    expirationMonth,
                    trade?.TradeCaptureId ?? TradeCapture.NoId))
        {
        }

        private static string ExpiryNotInCalendar(StockCalendar calendar, DateTime expirationMonth, int tradeId)
        {
            string expiry = expirationMonth.ToDayFirstString('/');

            return string.Format(
                "Calendar expiry date not found, trade [{3}], calendar [{0} ({1}], expiration month [{2}]",
                calendar.Name,
                calendar.CalendarId.ToString(),
                expiry,
                tradeId);
        }
    }
}