using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.CalendarsService.Data
{
    [Table("calendar_holidays")]
    public partial class CalendarHoliday
    {
        [Key]
        [Column("calendar_id", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CalendarId { get; set; }

        [Key]
        [Column("holiday_date", Order = 1, TypeName = "date")]
        public DateTime HolidayDate { get; set; }

        [ForeignKey("CalendarId")]
        public virtual StockCalendar StockCalendar { get; set; }

        private const int NoCalendar = 0;
        private static readonly DateTime NoDate = DateTime.MinValue;

        public static readonly CalendarHoliday Default = new CalendarHoliday()
        {
            CalendarId = NoCalendar,
            HolidayDate = NoDate,
            StockCalendar = new StockCalendar() { CalendarType = CalendarType.Holidays },
        };

        [ReadOnly(true)]        
        public bool IsDefault()
        {
            return NoCalendar == CalendarId && NoDate == HolidayDate;
        }

        public bool Equals(DateTime testDate)
        {
            return HolidayDate.Equals(testDate);
        }
    }
}
