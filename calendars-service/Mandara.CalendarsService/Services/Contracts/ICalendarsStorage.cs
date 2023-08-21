using Mandara.CalendarsService.Data;
using Optional;

namespace Mandara.CalendarsService.Services;

public interface ICalendarsStorage
{

    void Update();

    List<CalendarHoliday> GetCalendarHolidays(int id);
    List<CalendarHoliday> GetCalendarHolidays();

    List<CalendarExpiryDate> GetCalendarExpiryDates(int id);
    List<CalendarExpiryDate> GetCalendarExpiryDates();

    Option<StockCalendar> GetStockCalendar(int id);
    List<StockCalendar> GetStockCalendars();


}