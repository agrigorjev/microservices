using System;

namespace Mandara.Entities.Calculation
{
    public class Range<T>
        where T : IComparable<T>
    {
        public readonly T Start;
        public readonly T End;

        public Range(T start, T end)
        {
            if (start.CompareTo(end) > 0)
            {
                T temp = start;
                start = end;
                end = temp;
            }

            Start = start;
            End = end;
        }
    }
}