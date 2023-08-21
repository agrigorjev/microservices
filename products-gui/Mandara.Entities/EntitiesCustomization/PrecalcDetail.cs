using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Entities
{
    public class PrecalcDetail
    {
        public virtual DateTime Month { get; set; }
        public virtual Product Product { get; set; }
        public virtual int ProductId { get; set; }
        public virtual DateTime MinDay { get; set; }
        public virtual DateTime MaxDay { get; set; }

        protected Dictionary<DateTime, decimal?> daysPositions;
        [Obsolete("Switch to DailyPositions - zero is sufficient to show that the position for the day is zero.")]
        public virtual Dictionary<DateTime, decimal?> DaysPositions
        {
            get => daysPositions;
            set
            {
                if (value != null)
                {
                    daysPositions = value;
                    dailyPositions = BuildDailyPositions(value);
                }
                else
                {
                    daysPositions = new Dictionary<DateTime, decimal?>();
                    dailyPositions = new Dictionary<DateTime, decimal>();
                }
            }
        }

        // Once DaysPositions is removed (or the nullability is removed and this new collection is removed), this will
        // change to an auto-property.
        protected Dictionary<DateTime, decimal> dailyPositions;
        public virtual Dictionary<DateTime, decimal> DailyPositions
        {
            get => dailyPositions;
            set
            {
                if (value != null)
                {
                    dailyPositions = value;
                    daysPositions = value.ToDictionary(
                        posForDay => posForDay.Key,
                        posForDay => (decimal?)posForDay.Value);
                }
                else
                {
                    dailyPositions = new Dictionary<DateTime, decimal>();
                    daysPositions = new Dictionary<DateTime, decimal?>();
                }
            }
        }

        protected Dictionary<DateTime, decimal> BuildDailyPositions(Dictionary<DateTime, decimal?> daysPositions)
        {
            return null != daysPositions
                ? daysPositions.ToDictionary(posForDay => posForDay.Key, posForDay => posForDay.Value ?? 0M)
                : new Dictionary<DateTime, decimal>();
        }
    }
}
