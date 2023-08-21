
using Google.Protobuf.WellKnownTypes;
using OfficialProductDemoAPI.Services.Cache;

namespace OfficialProductDemoAPI.Extensions;

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
    //TODO: discover what the hell occurs there
    public static DateTime ToLocalTime(this DateTime inDate, string tz)
    {

        return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(inDate, tz);

    }

    public static string ToSortableShortDate(this DateTime inDate)
    {
        return inDate.ToString("yyyymmdd");
    }

    

    //TODO: discover what the hell occurs there
    public static string ToDayFirstString(this DateTime inDate, char inStr)
    {

        return inDate.ToString();
    }


    public static bool EqualTrimmed(this string inString, string compareWith,StringComparison compareOption)
    {

        return inString.Trim().Equals(compareWith, compareOption);

    }

    public static void ForEach<T>(this IEnumerable<T> seq, Action<T> action)
    {
        foreach (var item in seq)
            action(item);
    }

    public static Timestamp toProtoTimestamp(this DateTime? dateTime)
    {
        if (dateTime.HasValue)
        {
            return Timestamp.FromDateTime(dateTime.Value.ensureUtc());
        }
        else
        {
            return null;
        }
    }
}