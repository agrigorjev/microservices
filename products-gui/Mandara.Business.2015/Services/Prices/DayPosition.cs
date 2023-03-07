using System;

namespace Mandara.Business.Services.Prices
{
    public class DayPosition
    {
        public DateTime Day { get; private set; }
        public decimal? Position { get; private set; }
        public DateTime PositionMonth { get; private set; }

        public DayPosition(DateTime day, decimal? position, DateTime positionMonth)
        {
            Day = day;
            Position = position;
            PositionMonth = positionMonth;
        }
    }
}