using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.CalendarsService.Data
{
    [Table("calendar_futures_expiry")]
    public partial class CalendarExpiryDate
    {
        [Key]
        [Column("calendar_id", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CalendarId { get; set; }

        [Key]
        [Column("futures_date", Order = 1, TypeName = "date")]
        public DateTime FuturesDate { get; set; }

        [Column("expiry_date", TypeName = "date")]
        public DateTime ExpiryDate { get; set; }

        public virtual StockCalendar? StockCalendar { get; set; }

        private const int NoId = -1;
        private static readonly DateTime NoDate = DateTime.MinValue;

        public static readonly CalendarExpiryDate Default = new CalendarExpiryDate()
        {
            CalendarId = NoId,
            ExpiryDate = NoDate,
            FuturesDate = NoDate,
            StockCalendar = new StockCalendar() { CalendarType = CalendarType.Expiry },
        };

        [ReadOnly(true)]
        public bool IsDefault()
        {
            return NoId == CalendarId && NoDate == ExpiryDate && NoDate == FuturesDate;
        }
    }
}
