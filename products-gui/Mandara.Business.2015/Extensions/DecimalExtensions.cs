namespace Mandara.Business.Extensions
{
    public static partial class DecimalExtensions
    {
        public static bool IsInRange(this decimal toTest, decimal target, decimal rangeSize = 0.0001M) =>
            (target - rangeSize) < toTest && toTest < (target + rangeSize);
    }
}