using System;

namespace Mandara.Entities.EntitiesCustomization
{
    public class StripPart
    {
        public DateTime StartDate { get; private set; }
        public ProductDateType DateType { get; private set; }
        private static readonly DateTime _defaultDate = DateTime.MinValue;
        private const ProductDateType DefaultProductDateType = ProductDateType.NotSet;
        public static StripPart Default { get; private set; }

        static StripPart()
        {
            Default = new StripPart(
                        new Tuple<DateTime, ProductDateType>(_defaultDate, (ProductDateType)(DefaultProductDateType)));

        }

        public StripPart(Tuple<DateTime, ProductDateType> part)
        {
            StartDate = part.Item1;
            DateType = part.Item2;
        }

        public string ToPeriodOnlyString()
        {
            switch (DateType)
            {
                case ProductDateType.Day:
                case ProductDateType.Daily:
                case ProductDateType.MonthYear:
                {
                    return StartDate.ToString("MMMM");
                }

                case ProductDateType.Quarter:
                {
                    return $"Q{StartDate.Month / 3 + 1}";
                }

                case ProductDateType.Year:
                {
                    return $"CAL{StartDate.Year}";
                }

                case ProductDateType.Custom:
                {
                    return "Custom";
                }

                default:
                {
                    return "Unknown";
                }
            }
        }

        public bool IsDefault()
        {
            return StartDate.Equals(_defaultDate) && DateType == DefaultProductDateType;
        }
    }
}