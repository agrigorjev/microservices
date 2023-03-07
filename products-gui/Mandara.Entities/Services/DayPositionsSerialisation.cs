using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using Mandara.Date;

namespace Mandara.Entities.Services
{
    public static class DayPositionsSerialisation
    {
        private const string MonthYearFormat = "MM/yyyy";
        
        [Obsolete("Switch to DeserialiseDaysPositions which returns positions of 0 for days that have a zero position.")]
        public static Dictionary<DateTime, decimal?> DeserializeDaysNullablePositions(
            string serializedData,
            DateTime month,
            int productId,
            string source,
            int sourceId)
        {
            return
                GetDayPositionMaps(serializedData, month, productId, source, sourceId)
                    .ToDictionary(k => k.Day, v => v.Position);
        }

        [Obsolete("Only used by DeserializeDaysNullablePositions")]
        private static List<DayPositionMap> GetDayPositionMaps(
            string serializedData,
            DateTime month,
            int productId,
            string source,
            int sourceId)
        {
            List<DayPositionMap> dayPositionMaps = JsonConvert.DeserializeObject<List<DayPositionMap>>(serializedData);

            if (dayPositionMaps == null && !String.IsNullOrWhiteSpace(serializedData))
            {
                throw new Exception(NoDaysInPrecalc( source, sourceId, month, productId));
            }

            return dayPositionMaps ?? new List<DayPositionMap>();
        }

        private static string NoDaysInPrecalc(string source, int sourceId, DateTime posMonth, int productId)
        {
            return String.Format(
                "No days positions in precalc details for the {0} ID [{1}], month [{2}], product ID [{3}]",
                source,
                sourceId,
                posMonth.ToString(MonthYearFormat),
                productId);
        }

        public static Dictionary<DateTime, decimal> DeserialiseDailyPositions(
            string serializedData,
            DateTime month,
            int productId,
            string source,
            int sourceId)
        {
            return GetDailyPositions(
                    serializedData,
                    month,
                    productId,
                    source,
                    sourceId)
                .ToDictionary(posForDay => posForDay.Day, posForDay => posForDay.Position);
        }

        private static List<DayPosition> GetDailyPositions(
            string serializedData,
            DateTime month,
            int productId,
            string source,
            int sourceId)
        {
            List<DayPosition> positionPerDay = JsonConvert.DeserializeObject<List<DayPosition>>(serializedData);

            if (positionPerDay == null && !String.IsNullOrWhiteSpace(serializedData))
            {
                throw new Exception(NoDaysInPrecalc(source, sourceId, month, productId));
            }

            return positionPerDay ?? new List<DayPosition>();
        }

        public static string SerializeDaysPositions(Dictionary<DateTime, decimal?> daysPositions)
        {
            List<DayPositionMap> list =
                daysPositions.Select(x => new DayPositionMap { Day = x.Key, Position = x.Value })
                    .OrderBy(x => x.Day)
                    .ToList();

            string serialisedDaysPositions = JsonConvert.SerializeObject(
                list,
                Formatting.None,
                new IsoDateTimeConverter() { DateTimeFormat = Formats.DashSeparatedShortDate });

            return serialisedDaysPositions ?? String.Empty;
        }
    }
}