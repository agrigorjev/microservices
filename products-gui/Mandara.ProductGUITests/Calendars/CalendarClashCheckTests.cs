using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mandara.Entities;
using Mandara.Entities.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mandara.ProductGUI.Calendars
{
    [TestClass()]
    public class CalendarClashCheckTests
    {
        private int _calendarId = 1;
        private List<StockCalendar> _baseCalendars;
        private static readonly DateTime BaseExpiryDate = DateTime.Today;
        private static readonly DateTime BaseHoliday = DateTime.Today.AddDays(1);

        [TestInitialize]
        public void CreateCalendars()
        {
            _baseCalendars = CreateExpiryCalendars().Concat(CreateHolidayCalendars()).ToList();
        }

        private List<StockCalendar> CreateExpiryCalendars()
        {
            return new List<StockCalendar>()
            {
                CreateCalendar(CalendarType.Expiry, AddExpiryDatesToCalendar),
                CreateCalendar(CalendarType.Expiry, AddExpiryDatesToCalendar)
            };
        }

        private StockCalendar CreateCalendar(CalendarType typeToCreate, Func<StockCalendar, StockCalendar> addDates)
        {
            Interlocked.Increment(ref _calendarId);

            StockCalendar newCalendar = new StockCalendar()
            {
                CalendarId = _calendarId, Name = $"Calendar{_calendarId}", CalendarType = typeToCreate,
            };

            return addDates(newCalendar);
        }

        private StockCalendar AddExpiryDatesToCalendar(StockCalendar calendar)
        {
            DateTime expiryDate = GetCalendarBasedDate(BaseExpiryDate, calendar.CalendarId);
            DateTime startMonth = GetMonth(expiryDate);

            calendar.FuturesExpiries = new List<CalendarExpiryDate>()
            {
                new CalendarExpiryDate()
                {
                    CalendarId = _calendarId,
                    FuturesDate = startMonth,
                    ExpiryDate = expiryDate,
                    StockCalendar = calendar,
                },
                new CalendarExpiryDate()
                {
                    CalendarId = _calendarId,
                    FuturesDate = startMonth.AddMonths(1),
                    ExpiryDate = expiryDate.AddMonths(1),
                    StockCalendar = calendar,
                }
            };

            return calendar;
        }

        private DateTime GetCalendarBasedDate(DateTime baseDate, int id)
        {
            return baseDate.AddMonths(id);
        }

        private DateTime GetMonth(DateTime baseDate)
        {
            return new DateTime(baseDate.Year, baseDate.Month, 1);
        }

        private List<StockCalendar> CreateHolidayCalendars()
        {
            return new List<StockCalendar>()
            {
                CreateCalendar(CalendarType.Holidays, AddHolidaysToCalendar),
                CreateCalendar(CalendarType.Holidays, AddHolidaysToCalendar)
            };
        }

        private StockCalendar AddHolidaysToCalendar(StockCalendar calendar)
        {
            DateTime holiday = GetCalendarBasedDate(BaseHoliday, calendar.CalendarId);

            calendar.Holidays = new List<CalendarHoliday>()
            {
                new CalendarHoliday()
                {
                    CalendarId = _calendarId, HolidayDate = holiday, StockCalendar = calendar,
                },
                new CalendarHoliday()
                {
                    CalendarId = _calendarId, HolidayDate = holiday.AddMonths(1), StockCalendar = calendar,
                }
            };

            return calendar;
        }

        [TestMethod()]
        public void TestHasExpiryDate_NoClashingExpiry_ReturnsFalseAndEmptyName()
        {
            CalendarClashCheck clashCheck = new CalendarClashCheck();
            List<StockCalendar> expiryCalendars = GetCalendars(CalendarType.Expiry);
            int calendarId = expiryCalendars.First().CalendarId;

            clashCheck.InitExpiryCalendars(expiryCalendars);

            (bool firstHasClash, string firstClashingCalendar) = clashCheck.HasExpiryDate(
                GetCalendarBasedDate(BaseExpiryDate, calendarId).AddDays(1),
                calendarId);

            Assert.IsFalse(firstHasClash);
            Assert.AreEqual(CalendarClashCheck.DefaultName, firstClashingCalendar);
        }

        private List<StockCalendar> GetCalendars(CalendarType typeToGet)
        {
            return _baseCalendars.Where(calendar => typeToGet == calendar.CalendarType).ToList();
        }

        [TestMethod()]
        public void TestHasExpiryDate_OneClashingExpiry_OnlyOneCalendarClashes()
        {
            CalendarClashCheck clashCheck = new CalendarClashCheck();
            List<StockCalendar> expiryCalendars = GetCalendars(CalendarType.Expiry);
            int firstCalendar = expiryCalendars.First().CalendarId;
            DateTime expiryToTest = GetCalendarBasedDate(BaseExpiryDate, firstCalendar);

            clashCheck.InitExpiryCalendars(expiryCalendars);

            (bool firstHasClash, string firstClashingCalendar) = clashCheck.HasExpiryDate(expiryToTest, firstCalendar);

            Assert.IsTrue(firstHasClash);
            Assert.AreEqual($"Calendar{firstCalendar}", firstClashingCalendar);

            (bool lastHasClash, string lastClashingCalendar) = clashCheck.HasExpiryDate(
                expiryToTest,
                expiryCalendars.Last().CalendarId);

            Assert.IsFalse(lastHasClash);
            Assert.AreEqual(CalendarClashCheck.DefaultName, lastClashingCalendar);
        }

        [TestMethod()]
        public void TestHasHolidayDate_NoClashingExpiry_ReturnsFalseAndEmptyName()
        {
            CalendarClashCheck clashCheck = new CalendarClashCheck();
            List<StockCalendar> holidayCalendars = GetCalendars(CalendarType.Holidays);
            int calendarId = holidayCalendars.First().CalendarId;

            clashCheck.InitExpiryCalendars(holidayCalendars);

            (bool firstHasClash, string firstClashingCalendar) = clashCheck.HasHolidayDate(
                GetCalendarBasedDate(BaseHoliday, calendarId).AddDays(1),
                calendarId);

            Assert.IsFalse(firstHasClash);
            Assert.AreEqual(CalendarClashCheck.DefaultName, firstClashingCalendar);
        }

        [TestMethod()]
        public void TestHasHolidayDate_OneClashingExpiry_OnlyOneCalendarClashes()
        {
            CalendarClashCheck clashCheck = new CalendarClashCheck();
            List<StockCalendar> holidayCalendars = GetCalendars(CalendarType.Holidays);
            int firstCalendar = holidayCalendars.First().CalendarId;
            DateTime expiryToTest = GetCalendarBasedDate(BaseHoliday, firstCalendar);

            clashCheck.InitExpiryCalendars(holidayCalendars);

            (bool firstHasClash, string firstClashingCalendar) = clashCheck.HasHolidayDate(expiryToTest, firstCalendar);

            Assert.IsTrue(firstHasClash);
            Assert.AreEqual($"Calendar{firstCalendar}", firstClashingCalendar);

            (bool lastHasClash, string lastClashingCalendar) = clashCheck.HasHolidayDate(
                expiryToTest,
                holidayCalendars.Last().CalendarId);

            Assert.IsFalse(lastHasClash);
            Assert.AreEqual(CalendarClashCheck.DefaultName, lastClashingCalendar);
        }
    }
}