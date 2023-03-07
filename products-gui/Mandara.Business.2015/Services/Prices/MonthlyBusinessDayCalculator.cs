using System;
using System.Linq;
using Mandara.Business.Contracts;
using Mandara.Date;
using Mandara.Entities;
using Mandara.Entities.Enums;
using Mandara.Extensions.Option;
using Ninject.Extensions.Logging;

namespace Mandara.Business.Services.Prices
{
    public class MonthlyBusinessDayCalculator
    {
        private IProductsStorage _productsStorage;

        public MonthlyBusinessDayCalculator(IProductsStorage productsStorage)
        {
            _productsStorage = productsStorage;
        }

        public int GetNumberBusinessDaysInMonth(int calendarId, DateTime givenMonth)
        {
            int holidays = GetNumberHolidaysInMonth(calendarId, givenMonth);
            int weekdays = GetNumberWeekdaysInMonth(givenMonth);
            return weekdays - holidays;
        }

        public int GetRemainingBusinessDaysInMonth(int calendarId, DateTime riskDate)
        {
            int holidays = GetRemainingHolidaysInMonth(calendarId, riskDate);
            int weekdays = GetRemainingWeekdaysInMonth(riskDate, riskDate.Day);
            return weekdays - holidays;
        }

        private int GetNumberWeekdaysInMonth(DateTime givenMonth)
        {
            return GetRemainingWeekdaysInMonth(givenMonth, 0);
        }


        //TODO instead of Searching Contains for new DateTimeConstruction faster implementations can be implemented
        public int GetRemainingWeekdaysInMonth(DateTime givenMonth, int startDay)
        {
            DayOfWeek[] weekends = new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday };

            int year, month, numWeekdays;
            year = givenMonth.Year;
            month = givenMonth.Month;
            numWeekdays = 0;

            for (int dayInMonth = startDay + 1; dayInMonth <= DateTime.DaysInMonth(year, month); dayInMonth++)
            {
                if (!weekends.Contains((new DateTime(year, month, dayInMonth)).DayOfWeek))
                {
                    numWeekdays++;
                }
            }
            return numWeekdays;
        }

        //TODO following methods called numerous times for same results. Can be improved.
        public int GetNumberHolidaysInMonth(int calendarId, DateTime givenMonth)
        {
            return _productsStorage.CountHolidaysInMonth(calendarId, givenMonth);
        }

        public int GetRemainingHolidaysInMonth(int calendarId, DateTime riskDay)
        {
            return _productsStorage.CountRemainingHolidaysInMonth(calendarId, riskDay);
        }

        public bool IsTodayHoliday(DateTime riskDay, int calendarId)
        {
            return riskDay.IsWeekendDay() || _productsStorage.HasHoliday(calendarId, riskDay);
        }

        public bool IsProductHoliday(Product product, DateTime date)
        {
            return _productsStorage.HasHoliday(product.holidays_calendar_id ?? StockCalendar.DefaultId, date);
        }

        private DateTime GetNextBusinessDayForProduct(Product product, DateTime date)
        {
            date = date.AddDays(1);

            if (IsWeekendOrHoliday(date, product))
            {
                return GetNextBusinessDayForProduct(product, date);
            }
            return date;
        }

        private bool IsWeekendOrHoliday(DateTime date, Product product)
        {
            return date.IsWeekendDay() || IsProductHoliday(product, date);
        }

        public DateTime GetUsdConversionFxSpotDateForRiskDate(
            DateTime date,
            string foreignCurrencyIsoName,
            ILogger log)
        {
            TryGetResult<Product> fxProduct = _productsStorage.TryGetFxProductForCurrencyPair(
                CurrencyCodes.USD,
                foreignCurrencyIsoName);

            if (!fxProduct.HasValue)
            {
                log.Error(
                    "MonthlyBusinessDayCalculator: No FX product founds for currency pair {0}-{1}",
                    CurrencyCodes.USD,
                    foreignCurrencyIsoName);

                return date;
            }

            return GetNextBusinessDayForProduct(
                    fxProduct.Value,
                    GetNextBusinessDayForProduct(fxProduct.Value, date));
        }
    }
}
