
using Google.Protobuf.WellKnownTypes;

namespace CustomTypes
{


    public static class TimestampExtra
    {
       public static DateTime? DateTimeNullable(this Timestamp v)
        {
            if(v==null) return null;
            return v.ToDateTime();

        }
    }
}
