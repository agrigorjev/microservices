using System;
using Newtonsoft.Json;

namespace Mandara.Entities.Services
{
    [Obsolete]
    public class DayPositionMap
    {
        [JsonProperty(PropertyName = "d")]
        public DateTime Day { get; set; }
        [JsonProperty(PropertyName = "p")]
        public decimal? Position { get; set; }
    }

    /// <summary>
    /// Remove this comment when DayPositionMap is removed.
    /// A zero is sufficient to show that a day has no position.
    /// </summary>
    public class DayPosition
    {
        [JsonProperty(PropertyName = "d")]
        public DateTime Day { get; set; }
        [JsonProperty(PropertyName = "p")]
        public decimal Position { get; set; }

        [JsonConstructor]
        public DayPosition() { }

        public DayPosition(DateTime day, decimal position)
        {
            Day = day;
            Position = position;
        }
    }
}