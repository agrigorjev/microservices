using System;

namespace Mandara.Entities.Calculation
{
    public static class RangeHelper
    {
        public static bool Overlap<T>(Range<T> left, Range<T> right)
            where T : IComparable<T>
        {
            if (left.Start.CompareTo(right.Start) == 0)
                return true;

            if (left.Start.CompareTo(right.Start) > 0)
                return left.Start.CompareTo(right.End) <= 0;

            return right.Start.CompareTo(left.End) <= 0;
        }
    }
}