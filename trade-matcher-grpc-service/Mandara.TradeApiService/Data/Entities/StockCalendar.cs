using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Mandara.TradeApiService.Data
{
    [Serializable]
    [Table("stock_calendars")]
    public class StockCalendar
    {
        public StockCalendar()
        {
            FuturesExpiries = new HashSet<CalendarExpiryDate>();
            Holidays = new HashSet<CalendarHoliday>();
            Products = new HashSet<Product>();
        }

        [Column("calendar_id")]
        [Key]
        public int CalendarId { get; set; }

        [Column("name")]
        [StringLength(255)]
        public string Name { get; set; }

        [Column("correction")]
        public int? Correction { get; set; }

        [Column("timezone")]
        [StringLength(255)]
        public string Timezone { get; set; }

        [Column("calendar_type")]
        public int? CalendarTypeDb { get; set; }

        public virtual ICollection<CalendarExpiryDate> FuturesExpiries { get; set; }

        public virtual ICollection<CalendarHoliday> Holidays { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        [NotMapped]
        public CalendarType CalendarType
        {
            get => CalendarTypeDb == null ? CalendarType.ExpiryAndHolidays : (CalendarType)CalendarTypeDb;
            set => CalendarTypeDb = (int) value;
        }

        public override bool Equals(object obj)
        {
            return ((obj as StockCalendar)?.CalendarId ?? int.MinValue) == CalendarId;
        }

        public override int GetHashCode()
        {
            return CalendarId;
        }

        public override string ToString()
        {
            return Name;
        }

        public const int DefaultId = 0;
        public const string DefaultCalendarName = "DefaultCal";

        [NotMapped]
        public static StockCalendar Default = new StockCalendar()
        {
            CalendarId = DefaultId,
            CalendarType = CalendarType.ExpiryAndHolidays,
            Name = DefaultCalendarName,
            Correction = 0,
            FuturesExpiries = new List<CalendarExpiryDate>(),
            Holidays = new List<CalendarHoliday>(),
            Timezone = TimeZone.CurrentTimeZone.StandardName,
        };

        public bool IsDefault()
        {
            return Object.ReferenceEquals(this, Default)
                   || (DefaultId == CalendarId
                       && DefaultCalendarName == Name
                       && !FuturesExpiries.Any()
                       && !Holidays.Any());
        }

        public bool IsHoliday(DateTime testDay)
        {
            return Holidays.Any(holiday => holiday.Equals(testDay));
        }
    }
}
