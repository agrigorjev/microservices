using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.TradeApiService.Data
{
    [Serializable]
    [Table("exchanges")]
    public class Exchange
    {
        public const string IceExchangeName = "ICE";
        public const string IceOtcExchangeName = "ICE_OTC";
        public const string NymexExchangeName = "NYMEX";
        public const string CmeExchangeName = "CME";
        public const string SgxExchangeName = "SGX";
        public const string UnknownExchange = "Unknown";
        public const string IFEU = "IFEU";
        public const string IFLX = "IFLX";
        public const string IFUS = "IFUS";
        public const string IFED = "IFED";
        public const string NORX = "NORX";
        public const string NOS = "NOS";
        public const string TOCOM = "TOCOM";
        public const string TOCN = "TOCN";
        public const string XNYM = "XNYM";
        public const string NYM = "NYM";
        public const string CBT = "CBT";
        public const string OTC = "OTC";
        public const string Currenex = "Currenex";

        private StockCalendar _calendar;

        [Column("exchange_id")]
        [Key]
        public int ExchangeId { get; set; }

        [Column("exchange_name")]
        [StringLength(50)]
        public string? Name { get; set; }

        [Column("mapping_value")]
        [StringLength(10)]
        public string? MappingValue { get; set; }

        [Column("calendar_id")]
        public int? CalendarId { get; set; }

        [ForeignKey("CalendarId")]
        public virtual StockCalendar? Calendar
        {
            get
            {
                return _calendar;
            }
            set
            {
                _calendar = value;
                CalendarId = _calendar != null ? _calendar.CalendarId : (int?)null;
            }
        }

        [Column("timezone_id")]
        public string TimeZoneId { get; set; }

        [NotMapped]
        public TimeZoneInfo Timezone
        {
            get
            {
                return string.IsNullOrEmpty(TimeZoneId) ? null : TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            }
            set
            {
                TimeZoneId = value?.Id;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Exchange entity = obj as Exchange;

            if (entity == null)
            {
                return false;
            }

            return ExchangeId == entity.ExchangeId;
        }

        public override int GetHashCode()
        {
            return ExchangeId;
        }

        public override string ToString()
        {
            return Name;
        }

        private const string DefaultName = "FakeExchange";
        private const int DefaultId = -1;
        private const string DefaultMapping = "FakePriceCol";

        public static readonly Exchange Default = new Exchange() {
            Name = DefaultName,
            ExchangeId = DefaultId,
            Calendar = StockCalendar.Default,
            MappingValue = DefaultMapping,
        };

        public bool IsDefault()
        {
            return Object.ReferenceEquals(this, Default)
                   || (DefaultName == Name
                       && DefaultId == ExchangeId
                       && Calendar.IsDefault()
                       && DefaultMapping == MappingValue);
        }
    }
}
