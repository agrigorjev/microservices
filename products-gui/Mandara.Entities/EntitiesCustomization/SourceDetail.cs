using System;
using System.Globalization;
using Mandara.Date;

namespace Mandara.Entities
{
    public partial class SourceDetail
    {
        public DateTime? TradeDateAsDate 
        {
            get
            {
                DateTime tradeDate;

                DateTime.TryParseExact(
                    TradeDate,
                    Formats.DayFirstDateFormat('/'),
                    null,
                    DateTimeStyles.None,
                    out tradeDate);

                return tradeDate;
            }
        }

        public bool IsBookout
        {
            get
            {
                return !string.IsNullOrEmpty(TradeType) &&
                       (TradeType.ToUpper() == "ASSIGNMENT" || TradeType.ToUpper() == "TENDER");
            }
        }
    }
}
