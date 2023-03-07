using System;

namespace Mandara.Business
{
    public class SealsPriceParser
    {
        public static decimal? ParsePrice(string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0M;

            input = input.Trim();

            if ("s".Equals(input, StringComparison.InvariantCultureIgnoreCase) ||
                "s+".Equals(input, StringComparison.InvariantCultureIgnoreCase) ||
                "s-".Equals(input, StringComparison.InvariantCultureIgnoreCase))
                return 0M;

            if (input.StartsWith("s", StringComparison.InvariantCultureIgnoreCase))
                input = input.Substring(1);

            decimal price;
            if (Decimal.TryParse(input, out price))
                return price;

            return null;
        } 
    }
}