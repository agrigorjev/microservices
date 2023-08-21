using System.Collections.Concurrent;
using Optional;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Mandara.CalendarsService.Data;
using Mandara.CalendarsService.Configuration;
using System.Collections.Generic;

namespace Mandara.CalendarsService.Services;

public class CalendarsStorage : ICalendarsStorage
{
    private ConcurrentDictionary<int, StockCalendar> StockCalendars { get; set; } = new();
    private ConcurrentDictionary<int, List<CalendarExpiryDate>> CalendarExpiryDates { get; set; } = new();
    private ConcurrentDictionary<int, List<CalendarHoliday>> CalendarHolidays { get; set; } = new();


    private readonly DataStoragesSettings _serviceSettings;
    private readonly IDbContextFactory<MandaraEntities> _contextFactory;

    public CalendarsStorage(IOptions<DataStoragesSettings> serviceSettings,
        IDbContextFactory<MandaraEntities> contextFactory)
    {
        _serviceSettings = serviceSettings.Value;
        _contextFactory = contextFactory;
    }

    public List<CalendarHoliday> GetCalendarHolidays(int id)
    {

        if (CalendarHolidays.TryGetValue(id, out List<CalendarHoliday>? data))
        {
            if (data != null)
                return data;
        }

        return LoadCalendarHoliday(id);
    }

    public List<CalendarHoliday> GetCalendarHolidays()
    {
        return CalendarHolidays.Values.SelectMany(x=>x).ToList();
    }

    public List<CalendarExpiryDate> GetCalendarExpiryDates(int id)
    {
        if (CalendarExpiryDates.TryGetValue(id, out List<CalendarExpiryDate>? data))
        {
            if (data != null)
                return data;
        }

       return  LoadCalendarExpiryDates(id);
    }

    public List<CalendarExpiryDate> GetCalendarExpiryDates()
    {
        return CalendarExpiryDates.Values.SelectMany(x => x).ToList();
    }

    public Option<StockCalendar> GetStockCalendar(int id)
    {
        if (StockCalendars.TryGetValue(id, out StockCalendar? data))
        {
            if (data == null)
                return Option.None<StockCalendar>();

            return Option.Some(data);
        }

        StockCalendar dataLoaded = LoadStockCalendar(id);

        return dataLoaded.IsDefault() ? Option.None<StockCalendar>() : Option.Some(dataLoaded);
    }

    public List<StockCalendar> GetStockCalendars()
    {
        return StockCalendars.Values.ToList();
    }

  

    public void Update()
    {
        using (MandaraEntities productsDb = _contextFactory.CreateDbContext())
        {
            StockCalendars = new ConcurrentDictionary<int, StockCalendar>(GetStockCalendarQuery(productsDb).ToDictionary(calendar => calendar.CalendarId));
            CalendarExpiryDates = new ConcurrentDictionary<int, List<CalendarExpiryDate>>(GetAllExpiryDatesQuery(productsDb).GroupBy(calendar => calendar.CalendarId).ToDictionary(x =>x.Key,x=>x.ToList()));
            CalendarHolidays = new ConcurrentDictionary<int, List<CalendarHoliday>>(GetAllHolidaysQuery(productsDb).GroupBy(calendar => calendar.CalendarId).ToDictionary(x => x.Key, x => x.ToList()));
        }

    }

    private StockCalendar LoadStockCalendar(int id)
    {
        using (MandaraEntities productsDb = _contextFactory.CreateDbContext())
        {
            StockCalendar stockCalendar = GetStockCalendarQuery(productsDb).FirstOrDefault(calendar => calendar.CalendarId == id) ?? StockCalendar.Default;
            if (!stockCalendar.IsDefault())
            {
                StockCalendars.TryAdd(stockCalendar.CalendarId, stockCalendar);
            }
            return stockCalendar;
        }

    }
    private List<CalendarHoliday> LoadCalendarHoliday(int id)
    {
        using (MandaraEntities productsDb = _contextFactory.CreateDbContext())
        {
            List<CalendarHoliday> calendarHoliday = GetAllHolidaysQuery(productsDb).Where(calendar => calendar.CalendarId == id).ToList();
            if (calendarHoliday.Count>0)
            {
                CalendarHolidays.TryAdd(id, calendarHoliday);
            }
            return calendarHoliday;
        }

    }
    private List<CalendarExpiryDate> LoadCalendarExpiryDates(int id)
    {
        using (MandaraEntities productsDb = _contextFactory.CreateDbContext())
        {
            List<CalendarExpiryDate> expiryDates = GetAllExpiryDatesQuery(productsDb).Where(calendar => calendar.CalendarId == id).ToList();
            if (expiryDates.Count>0)
            {
                CalendarExpiryDates.TryAdd(id, expiryDates);
            }
            return expiryDates;
        }

    }


    private static IQueryable<StockCalendar> GetStockCalendarQuery(MandaraEntities cxt)
    {
        return cxt.StockCalendars.Include(x => x.Holidays).Include(x => x.FuturesExpiries);
    }

    private static IQueryable<CalendarHoliday> GetAllHolidaysQuery(MandaraEntities cxt)
    {
        return cxt.CalendarHolidays;
    }

    private static IQueryable<CalendarExpiryDate> GetAllExpiryDatesQuery(MandaraEntities cxt)
    {
        return cxt.CalendarExpiryDates;
    }

    
}

   