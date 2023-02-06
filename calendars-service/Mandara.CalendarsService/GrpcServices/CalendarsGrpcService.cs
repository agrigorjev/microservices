using Grpc.Core;
using Mandara.CalendarsService.GrpcDefinitions;
using Mandara.CalendarsService.Services;
using Mandara.CalendarsService.Data;
using Mandara.CalendarsService.DataConverters;
using static Mandara.CalendarsService.GrpcDefinitions.CalendarService;

namespace Mandara.CalendarsService.GrpcServices;

public class CalendarsGrpcService : CalendarServiceBase
{
    private readonly ICalendarsStorage _cache;
    private readonly DataConverters.StockCalendarConverter stockCalendarConverter = new DataConverters.StockCalendarConverter();
    private readonly DataConverters.CalendarHolidayConverter calendarHolidayConverter   = new DataConverters.CalendarHolidayConverter();
    private readonly DataConverters.CalendarExpiryDateConverter calendarExpiryDateConverter = new DataConverters.CalendarExpiryDateConverter();

    public CalendarsGrpcService(ICalendarsStorage cache)
    {
        _cache = cache;
    }

    public override Task<StockCalendarsGrpcMessage> GetAllStockCalendars(GetAllRequestMessage request, ServerCallContext context)
    {
        StockCalendarsGrpcMessage responseMessage = new();
        _cache.GetStockCalendars().ForEach(calendar => responseMessage.StockCalendars.Add(stockCalendarConverter.Convert(calendar)));
        return Task.FromResult(responseMessage);
    }

    public override Task<StockCalendarGrpcResponse> GetStockCalendar(GetByIdRequestMessage request, ServerCallContext context)
    {
        StockCalendarGrpcResponse responseMessage = new();

        var calendar = _cache.GetStockCalendar(request.Id);

        responseMessage.StockCalendaryData = stockCalendarConverter.Convert(calendar.ValueOr(StockCalendar.Default));

        return Task.FromResult(responseMessage);
    }

    public override Task<HolidaysGrpcMessage> GetAllHolidays(GetAllRequestMessage request, ServerCallContext context)
    {
        HolidaysGrpcMessage responseMessage = new();
        _cache.GetCalendarHolidays().ForEach(calendar => responseMessage.Holidays.Add(calendarHolidayConverter.Convert(calendar)));
        return Task.FromResult(responseMessage);
    }

    public override Task<HolidaysGrpcMessage> GetHolidays(GetByIdRequestMessage request, ServerCallContext context)
    {
        HolidaysGrpcMessage responseMessage = new();
        _cache.GetCalendarHolidays(request.Id).ForEach(calendar => responseMessage.Holidays.Add(calendarHolidayConverter.Convert(calendar)));
        return Task.FromResult(responseMessage);
    }

    public override Task<ExpiryDatesGrpcMessage> GetAllExpiryDates(GetAllRequestMessage request, ServerCallContext context)
    {
        ExpiryDatesGrpcMessage responseMessage = new();
        _cache.GetCalendarExpiryDates().ForEach(calendar => responseMessage.ExpiryDates.Add(calendarExpiryDateConverter.Convert(calendar)));
        return Task.FromResult(responseMessage);
    }

    public override Task<ExpiryDatesGrpcMessage> GetExpiryDates(GetByIdRequestMessage request, ServerCallContext context)
    {
        ExpiryDatesGrpcMessage responseMessage = new();
        _cache.GetCalendarExpiryDates(request.Id).ForEach(calendar => responseMessage.ExpiryDates.Add(calendarExpiryDateConverter.Convert(calendar)));
        return Task.FromResult(responseMessage);
    }
}