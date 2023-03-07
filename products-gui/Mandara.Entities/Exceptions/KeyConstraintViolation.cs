using System.Text.RegularExpressions;

namespace Mandara.Entities.Exceptions
{
    public static class KeyConstraintViolation
    {
        public const string TableMatchName = "table";
        public const string KeyMatchName = "key";

        public static readonly Regex UniqueKeyViolationPattern = new Regex(
            $"(?<=UNIQUE KEY.+duplicate key in object 'dbo\\.)(?<{TableMatchName}>[^']+)'\\. The duplicate key value is "
            + $"\\((?<{KeyMatchName}>[^)]+)");
    }
}
