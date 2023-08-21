using Google.Protobuf.WellKnownTypes;

namespace Mandara.TradeApiService;

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
        if (dateTime.HasValue)
        {
            return Timestamp.FromDateTime(dateTime.Value.ensureUtc());
        }
        else
        {
            return new Timestamp();
        }
    }

    public static Timestamp toProtoTimestamp(this DateTime dateTime)
    {
        return Timestamp.FromDateTime(dateTime.ensureUtc());
    }
}