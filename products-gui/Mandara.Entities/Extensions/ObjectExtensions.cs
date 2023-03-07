using System;

namespace Mandara.Entities.Extensions
{
    public static class ObjectExtensions
    {
        [Obsolete]
        public static bool IsNull(this object instanse)
        {
            return instanse == null;
        }

        [Obsolete]
        public static string ToDataString(this object value)
        {
            return value == null ? "" : value.ToString();
        }
    }
}
