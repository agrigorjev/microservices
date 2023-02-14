using Google.Protobuf.WellKnownTypes;
using Mandara.CalendarsService.Configuration;
using Mandara.CalendarsService.Data;
using Mandara.CalendarsService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mandara.CalendarsService;

public static class Helpers
{
    public static DateTime ensureUtc(this DateTime inDate)
    {
        if (inDate.Kind == DateTimeKind.Unspecified)
        {
            return DateTime.SpecifyKind(inDate, DateTimeKind.Utc);
        }
        else
        {
            return inDate.ToUniversalTime();
        }

    }

    public static Timestamp toProtoTimestamp(this DateTime? dateTime)
    {
        if(dateTime.HasValue)
        {
            return Timestamp.FromDateTime(dateTime.Value.ensureUtc());
        }
       else
        {
            return new Timestamp();
        }
    }
}