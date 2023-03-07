using System;
using System.Collections.Generic;
using System.Linq;
using Mandara.Date;
using Mandara.Date.Time;
using Mandara.Entities;
using Optional;
using Optional.Collections;
using Optional.Unsafe;

namespace Mandara.Business.Client.Bus.Messages.Exposures.SwapCrossAutomation
{
    public class ExpirySplit
    {
        public DateTime FirstExpiry { get; }
        public DateTime SecondExpiry { get; }
        public int DaysToExpiry { get; }
        public int FromExpiryToNextMonth { get; }

        public ExpirySplit(DateTime firstExp, DateTime secondExp, int daysToExp, int fromExpToNextMonth)
        {
            FirstExpiry = firstExp;
            SecondExpiry = secondExp;
            DaysToExpiry = daysToExp;
            FromExpiryToNextMonth = fromExpToNextMonth;
        }
    }

    public static class SplitAtExpiry
    {
        public static ExpirySplit SplitPositionMonthAtExpiry(Product swapCrossProduct, DateTime positionMonth)
        {
            DateTime firstPosDay = positionMonth.FirstDayOfMonth();
            DateTime lastPosDay = positionMonth.LastDayOfMonth();
            DateTime expiry = GetExpiryDate(positionMonth.FirstDayOfMonth());
            int daysInCurrentMonth = CountBusinessDays(
                firstPosDay.Latest(SystemTime.Today()),
                lastPosDay,
                swapCrossProduct.HolidaysCalendar ?? StockCalendar.Default);
            int daysToNextMonth = FromExpiryToNextMonth();
            DateTime secondExpiry = 0 == daysToNextMonth
                ? expiry
                : GetExpiryDate(positionMonth.AddMonths(1).FirstDayOfMonth());

            return new ExpirySplit(expiry, secondExpiry, daysInCurrentMonth - daysToNextMonth, daysToNextMonth);

            int FromExpiryToNextMonth()
            {
                return (expiry.Equals(lastPosDay)
                           ? 0
                           : CountBusinessDays(
                               expiry.AddDays(1),
                               lastPosDay,
                               swapCrossProduct.HolidaysCalendar ?? StockCalendar.Default))
                       - (swapCrossProduct.ExpiryCalendar.Correction ?? 0);
            }

            DateTime GetExpiryDate(DateTime expiryMonth)
            {
                Option<CalendarExpiryDate> securityExpires =
                    swapCrossProduct.ExpiryCalendar.FuturesExpiries.FirstOrNone(
                        futuresExp => futuresExp.ExpiryDate.FirstDayOfMonth() == expiryMonth);

                return securityExpires.ValueOrDefault()?.ExpiryDate ?? lastPosDay;
            }
        }

        private static int CountBusinessDays(DateTime startDate, DateTime endDate, StockCalendar holidays)
        {
            return new DateRange(startDate, endDate).WeekDays().Count(weekday => !holidays.IsHoliday(weekday));
        }
    }

    public static class DateTimeExtensions
    {
        public static bool IsLastDayOfMonth(this DateTime date)
        {
            return date.LastDayOfMonth().Equals(date);
        }

        public static DateTime Latest(this DateTime baseDate, DateTime testDate)
        {
            return baseDate >= testDate ? baseDate : testDate;
        }
    }

    public static class DateRangeExtensions
    {
        public static bool Contains(this DateRange dates, DateTime date)
        {
            return (dates.HasStartDate() && date >= dates.Start) && (dates.HasEndDate() && date <= dates.End);
        }

        public static IEnumerable<DateTime> WeekDays(this DateRange dates)
        {
            if (!dates.HasStartDate() || !dates.HasEndDate())
            {
                yield break;
            }

            DateTime currentDate = dates.Start;

            while (currentDate <= dates.End)
            {
                if (!currentDate.IsWeekendDay())
                {
                    yield return currentDate;
                }

                currentDate = currentDate.AddDays(1);
            }
        }
    }
}
