using System;

namespace Mandara.Entities.EntitiesCustomization
{
    public class Strip
    {
        public StripPart Part1 { get; private set; }
        public StripPart Part2 { get; private set; }
        public bool IsTimeSpread { get; private set; }

        public static readonly Strip Default;

        static Strip()
        {
            Default = new Strip() { Part1 = StripPart.Default, Part2 = StripPart.Default };
        }

        private Strip() { }

        public Strip(Tuple<DateTime, ProductDateType> part1, Tuple<DateTime, ProductDateType> part2 = null)
        {
            Part1 = new StripPart(part1);

            if (part2 != null)
            {
                Part2 = new StripPart(part2);
                IsTimeSpread = !Part2.IsDefault();
            }
            else
            {
                Part2 = StripPart.Default;
            }
        }

        public string ToPeriodOnlyString()
        {
            string part1Period = Part1.ToPeriodOnlyString();

            if (IsTimeSpread)
            {
                return string.Format("{0}/{1}", part1Period, Part2.ToPeriodOnlyString());
            }

            return part1Period;
        }

        public bool IsDefault()
        {
            return Part1.IsDefault();
        }
    }
}