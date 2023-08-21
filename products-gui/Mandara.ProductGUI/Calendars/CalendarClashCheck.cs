using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mandara.Business.Managers.Calendars;
using Mandara.Entities;
using Optional.Collections;

namespace Mandara.ProductGUI.Calendars
{
    public class CalendarClashCheck
    {
        public static readonly string DefaultName = string.Empty;
        private List<StockCalendar> _calendars = new List<StockCalendar>();
        private Dictionary<(int, int), bool> _clashingCalendars = new Dictionary<(int, int), bool>();

        public void InitHolidayCalendars(List<StockCalendar> holidayCalendars)
        {
            _calendars = holidayCalendars;
        }

        public void UpdateHolidayCalendars(UpdatedCalendars changes)
        {
            UpdateCalendars(changes, UpdateCalendarHolidays);
        }

        private void UpdateCalendarHolidays(StockCalendar existing, StockCalendar updated)
        {
            existing.Holidays = updated.Holidays;
        }

        private void UpdateCalendars(
            UpdatedCalendars changes,
            Action<StockCalendar, StockCalendar> updateCalendarDates)
        {
            if (!_calendars.Any())
            {
                return;
            }

            _calendars = _calendars.Where(calendar => !changes.Removed.Contains(calendar.CalendarId))
                                   .ToList();
            _calendars.AddRange(changes.Added);
            changes.Modified.ForEach(
                changed =>
                {
                    StockCalendar existing = _calendars.First(
                        calendar => calendar.CalendarId == changed.CalendarId);

                    existing.Name = changed.Name;
                    updateCalendarDates(existing, changed);
                });
        }

        public void InitExpiryCalendars(List<StockCalendar> expiryCalendars)
        {
            _calendars = expiryCalendars;
        }

        public void UpdateExpiryCalendars(UpdatedCalendars changes)
        {
            UpdateCalendars(changes, UpdateCalendarExpiries);
        }

        private void UpdateCalendarExpiries(StockCalendar existing, StockCalendar updated)
        {
            existing.FuturesExpiries = updated.FuturesExpiries;
        }

        public (bool, string) HasExpiryDate(DateTime date, int calendarId)
        {
            CalendarExpiryDate matchingExpiry = GetCalendar(calendarId)
                                              .FuturesExpiries.FirstOrNone(expiry => date.Equals(expiry.ExpiryDate))
                                              .ValueOr(CalendarExpiryDate.Default);

            return matchingExpiry.IsDefault() ? (false, DefaultName) : (true, matchingExpiry.StockCalendar.Name);
        }

        private StockCalendar GetCalendar(int id)
        {
            return _calendars.First(calendar => id == calendar.CalendarId);
        }

        public (bool, string) HasHolidayDate(DateTime date, int calendarId)
        {
            CalendarHoliday matchingHoliday = GetCalendar(calendarId)
                                              .Holidays.FirstOrNone(holiday => date.Equals(holiday.HolidayDate))
                                              .ValueOr(CalendarHoliday.Default);

            return matchingHoliday.IsDefault() ? (false, DefaultName) : (true, matchingHoliday.StockCalendar.Name);
        }

        public bool DoCalendarsClash(StockCalendar holidayCalendar, StockCalendar expiryCalendar)
        {
            if (_clashingCalendars.TryGetValue(
                (holidayCalendar.CalendarId, expiryCalendar.CalendarId),
                out bool hasClash))
            {
                return hasClash;
            }

            hasClash =
                DoHolidaysAndExpiriesClash(holidayCalendar.Holidays.ToList(), expiryCalendar.FuturesExpiries.ToList())
                || DoHolidaysAndExpiriesClash(
                    expiryCalendar.Holidays.ToList(),
                    holidayCalendar.FuturesExpiries.ToList());

            _clashingCalendars.Add((holidayCalendar.CalendarId, expiryCalendar.CalendarId), hasClash);
            return hasClash;
        }

        private bool DoHolidaysAndExpiriesClash(List<CalendarHoliday> holidays, List<CalendarExpiryDate> expiryDates)
        {
            return expiryDates.Any(
                date => holidays.BinarySearch(
                            new CalendarHoliday() { HolidayDate = date.ExpiryDate },
                            new HolidayComparer())
                        >= 0);
        }

        private class HolidayComparer : IComparer<CalendarHoliday>
        {
            public int Compare(CalendarHoliday x, CalendarHoliday y)
            {
                return x.HolidayDate.CompareTo(y.HolidayDate);
            }
        }
    }
}
