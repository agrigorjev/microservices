using System;

namespace Mandara.Entities.Enums
{
    public enum BaseMonth
    {
        Next,
        Current
    }

    public static class BaseMonthExtensions
    {
        public static DateTime GetOffsetMonth(this BaseMonth baseMonth, DateTime currentMonth)
        {
            return BaseMonth.Next == baseMonth ? currentMonth.AddMonths(1) : currentMonth;
        }

        public static DateTime GetOffsetMonth(this BaseMonth baseMonth, DateTime nextMonth, DateTime currentMonth)
        {
            return BaseMonth.Next == baseMonth ? nextMonth : currentMonth;
        }
    }
}