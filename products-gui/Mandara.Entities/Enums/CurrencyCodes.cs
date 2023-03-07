using System.Collections.Generic;

namespace Mandara.Entities.Enums
{
    public static class CurrencyCodes
    {
        public const string All = "All";

        public const string USD = "USD";
        public const string JPY = "JPY";
        public const string GBP = "GBP";
        public const string EUR = "EUR";

        public static readonly HashSet<string> AllCodes = new HashSet<string>() { All, USD, JPY, GBP, EUR };
    }
}