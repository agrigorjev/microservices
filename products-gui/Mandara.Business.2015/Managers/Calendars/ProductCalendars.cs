using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Mandara.Entities;
using Mandara.Entities.Enums;
using Ninject.Extensions.Logging;

namespace Mandara.Business.Managers.Calendars
{
    internal class ProductCalendars
    {
        private static readonly ILogger Logger = new NLogLoggerFactory().GetCurrentClassLogger();

        public event EventHandler<ExpiriesSavedEventArgs> CalendarExpiryDatesSaved;
        public event EventHandler<HolidaysSavedEventArgs> CalendarHolidaysSaved;

        public List<StockCalendar> GetCalendars()
        {
            using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
            {
                return dbContext.StockCalendars.Include("Holidays").Include("FuturesExpiries").ToList();
            }
        }

        private static MandaraEntities CreateMandaraProductsDbContext()
        {
            return new MandaraEntities(MandaraEntities.DefaultConnStrName, nameof(ProductManager));
        }

        public List<StockCalendar> GetHolidaysCalendars()
        {
            using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
            {
                List<int?> calendarTypes =
                    new List<int?> { (int)CalendarType.ExpiryAndHolidays, (int)CalendarType.Holidays };

                return LoadCalendars(dbContext, calendarTypes);
            }
        }

        private static List<StockCalendar> LoadCalendars(MandaraEntities dbContext, List<int?> calendarTypes)
        {
            return dbContext.StockCalendars.Include("Holidays").Include("FuturesExpiries").Where(
                calendar => calendar.CalendarTypeDb == null
                            || calendarTypes.Contains(calendar.CalendarTypeDb)).ToList();
        }

        public List<StockCalendar> GetExpiryCalendars()
        {
            using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
            {
                List<int?> calendarTypes =
                    new List<int?> { (int)CalendarType.ExpiryAndHolidays, (int)CalendarType.Expiry };

                return LoadCalendars(dbContext, calendarTypes);
            }
        }

        public void UpdateExpiryCalendars(CalendarChanges<NewExpiryCalendar> calendarChanges, int year)
        {
            UpdateCalendars(
                calendarChanges,
                AddCalendarWithExpiryDates,
                GetSaveModifiedExpiryCalendars(year),
                OnExpiryCalendarsUpdated);
        }

        //private (bool, List<StockCalendar>) AddCalendars<T>(
        //    List<T> addedCalendars,
        //    Func<List<T>, (bool, List<StockCalendar>)> executeAdd)
        //{
        //    return executeAdd(addedCalendars);
        //}

        private Func<List<StockCalendar>, List<StockCalendar>> GetSaveModifiedExpiryCalendars(int year)
        {
            return (modifiedCalendars) => SaveCalendarExpiryDates(year, modifiedCalendars);
        }

        private void UpdateCalendars<T>(
            CalendarChanges<T> calendarChanges,
            Func<List<T>, List<StockCalendar>> executeAdd,
            Func<List<StockCalendar>, List<StockCalendar>> executeModify,
            Action<List<StockCalendar>, List<int>, List<StockCalendar>> onUpdateSuccess)
        {
            List<StockCalendar> newCalendars = executeAdd(calendarChanges.AddedCalendars);
            bool removed = RemoveCalendars(calendarChanges.DeletedCalendars);
            List<StockCalendar> modifiedCalendars = executeModify(calendarChanges.ModifiedCalendars);

            List<int> removedCalendars = removed ? calendarChanges.DeletedCalendars : new List<int>();

            if (newCalendars.Any() || removedCalendars.Any() || modifiedCalendars.Any())
            {
                onUpdateSuccess(newCalendars, removedCalendars, modifiedCalendars);
            }
        }

        private void OnExpiryCalendarsUpdated(
            List<StockCalendar> added,
            List<int> removed,
            List<StockCalendar> modified)
        {
            CalendarExpiryDatesSaved?.Invoke(this, new ExpiriesSavedEventArgs(added, removed, modified));
        }

        public List<StockCalendar> SaveCalendarExpiryDates(int year, List<StockCalendar> updatedExpiryDates)
        {
            if (!updatedExpiryDates.Any())
            {
                return new List<StockCalendar>();
            }

            using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
            using (DbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    dbContext.Configuration.AutoDetectChangesEnabled = false;

                    List<StockCalendar> modifiedCalendars = UpdateExpiryCalendars(dbContext);

                    dbContext.ChangeTracker.DetectChanges();
                    dbContext.SaveChanges();
                    transaction.Commit();
                    return modifiedCalendars;
                }
                catch (SqlException saveError)
                {
                    Logger.Error(saveError, "Error saving updated expiry calendars\n");
                    return new List<StockCalendar>();
                }
            }

            List<StockCalendar> UpdateExpiryCalendars(MandaraEntities dbContext)
            {
                return updatedExpiryDates.Aggregate(new List<StockCalendar>(),
                    (modified, existingExpiry) =>
                    {
                        StockCalendar modifiedCalendar = UpdateExpiryCalendar(dbContext, existingExpiry);

                        if (null != modifiedCalendar)
                        {
                            modified.Add(modifiedCalendar);
                        }

                        return modified;
                    });
            }

            StockCalendar UpdateExpiryCalendar(MandaraEntities dbContext, StockCalendar existingExpiry)
            {
                StockCalendar existingCalendar = dbContext.StockCalendars.Include("FuturesExpiries")
                                                        .SingleOrDefault(
                                                            c => c.CalendarId == existingExpiry.CalendarId);

                if (existingCalendar == null)
                {
                    return null;
                }

                UpdateCalendarMetadata(existingCalendar, existingExpiry);
                UpdateExpiries(existingCalendar, existingExpiry, dbContext);
                ProductManager.SetIsChangedForRelatedProducts(dbContext, existingExpiry.CalendarId);
                return existingCalendar;
            }

            void UpdateCalendarMetadata(StockCalendar existingCalendar, StockCalendar existingExpiry)
            {
                existingCalendar.Name = existingExpiry.Name;
                existingCalendar.CalendarType = existingExpiry.CalendarType;
                existingCalendar.Timezone = existingExpiry.Timezone;
                existingCalendar.Correction = existingExpiry.Correction;
            }

            void UpdateExpiries(StockCalendar existingCalendar, StockCalendar targetCalendar, MandaraEntities dbContext)
            {
                List<CalendarExpiryDate> expiriesForYear = existingCalendar
                                                           .FuturesExpiries.Where(
                                                               expiryYear => expiryYear.FuturesDate.Year == year)
                                                           .ToList();

                targetCalendar.FuturesExpiries.ToList().ForEach(
                    date => { AddOrUpdateExpiry(expiriesForYear, date, existingCalendar, dbContext); });
            }

            void AddOrUpdateExpiry(
                List<CalendarExpiryDate> existingDates,
                CalendarExpiryDate expiryToSet,
                StockCalendar targetCalendar,
                MandaraEntities dbContext)
            {
                CalendarExpiryDate existingExpiry = existingDates.SingleOrDefault(
                    expiryMonth => expiryMonth.FuturesDate.Month == expiryToSet.FuturesDate.Month);

                if (existingExpiry == null)
                {
                    AddExpiry(expiryToSet, targetCalendar, dbContext);
                }
                else
                {
                    UpdateExpiry(existingExpiry, expiryToSet);
                }
            }

            void AddExpiry(CalendarExpiryDate expiryToSet, StockCalendar targetCalendar, MandaraEntities dbContext)
            {
                CalendarExpiryDate newExpiry = new CalendarExpiryDate()
                {
                    ExpiryDate = expiryToSet.ExpiryDate,
                    FuturesDate = expiryToSet.FuturesDate
                };

                targetCalendar.FuturesExpiries.Add(newExpiry);
                dbContext.CalendarExpiryDates.Add(newExpiry);
            }

            void UpdateExpiry(CalendarExpiryDate existingExpiry, CalendarExpiryDate expiryToSet)
            {
                existingExpiry.ExpiryDate = expiryToSet.ExpiryDate;
                existingExpiry.FuturesDate = expiryToSet.FuturesDate;
            }
        }

        public List<StockCalendar> AddCalendarWithExpiryDates(List<NewExpiryCalendar> added)
        {
            if (!added.Any())
            {
                return new List<StockCalendar>();
            }

            using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
            using (DbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<StockCalendar> newCalendars = added.Aggregate(
                        new List<StockCalendar>(),
                        (addedCalendars, newExpiry) =>
                        {
                            StockCalendar calendar = new StockCalendar
                            {
                                Name = newExpiry.Name,
                                CalendarType = CalendarType.Expiry,
                                Correction = newExpiry.DateCorrection,
                                Timezone = newExpiry.CalendarTimeZone?.Id,
                            };

                            newExpiry.ExpiryDates.ForEach(itm => calendar.FuturesExpiries.Add(itm));

                            dbContext.StockCalendars.Add(calendar);
                            addedCalendars.Add(calendar);
                            return addedCalendars;
                        });

                    dbContext.SaveChanges();
                    transaction.Commit();
                    return newCalendars;
                }
                catch (SqlException saveError)
                {
                    Logger.Error(saveError, "Failed to save new expiry calendar\n");
                    return new List<StockCalendar>();
                }
            }
        }

        /// <summary>
        /// Check calendar may be deleted
        /// </summary>
        /// <param name="id">IdOfCalendar</param>
        /// <returns>True if calendar may be deleted</returns>
        public bool CalendarMayBeRemoved(int id)
        {
            try
            {
                using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
                {
                    return dbContext.StockCalendars.Any(
                               calendar => calendar.CalendarId == id && calendar.Products.Count == 0)
                           && !dbContext.Exchanges.Any(exchange => exchange.Calendar.CalendarId == id);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(
                    ex,
                    "Error determining whether the calendar with ID [{0}] can be removed.\n",
                    id);
                return false;
            }
        }

        /// <summary>
        /// Remove calendar from DB
        /// </summary>
        /// <param name="calendarIds">Array of Ids of calendar to remove</param>
        public bool RemoveCalendars(List<int> calendarIds)
        {
            using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
            using (DbContextTransaction removeTransaction = dbContext.Database.BeginTransaction())
            {
                foreach (int id in calendarIds)
                {
                    try
                    {
                        StockCalendar calendar = dbContext.StockCalendars
                            .Include("FuturesExpiries")
                            .Include("Holidays")
                            .SingleOrDefault(sc => sc.CalendarId == id);

                        if (calendar == null)
                        {
                            continue;
                        }

                        calendar.Holidays.ToList().ForEach(cc => dbContext.CalendarHolidays.Remove(cc));
                        calendar.Holidays.Clear();
                        calendar.FuturesExpiries.ToList().ForEach(cc => dbContext.CalendarExpiryDates.Remove(cc));
                        calendar.FuturesExpiries.Clear();
                        dbContext.StockCalendars.Remove(calendar);
                        dbContext.SaveChanges();
                    }
                    catch (Exception whatEx)
                    {
                        Logger.Error(whatEx, "Error while trying to remove a calendar\n");
                        removeTransaction.Rollback();
                        return false;
                    }
                }

                removeTransaction.Commit();
                return true;
            }
        }

        public void UpdateHolidayCalendars(CalendarChanges<NewHolidayCalendar> calendarChanges, int year, int month)
        {
            UpdateCalendars(
                calendarChanges,
                AddCalendarWithHolidayDates,
                GetSaveModifiedHolidayCalendars(year, month),
                OnHolidayCalendarsUpdated);
        }

        private Func<List<StockCalendar>, List<StockCalendar>> GetSaveModifiedHolidayCalendars(
            int year,
            int month)
        {
            return (modifiedCalendars) => SaveCalendarHolidays(year, month, modifiedCalendars);
        }

        private void OnHolidayCalendarsUpdated(
            List<StockCalendar> added,
            List<int> removed,
            List<StockCalendar> modified)
        {
            CalendarHolidaysSaved?.Invoke(this, new HolidaysSavedEventArgs(added, removed, modified));
        }

        public List<StockCalendar> SaveCalendarHolidays(
            int year,
            int month,
            List<StockCalendar> updatedHolidays)
        {
            if (!updatedHolidays.Any())
            {
                return new List<StockCalendar>();
            }

            using (MandaraEntities context = CreateMandaraProductsDbContext())
            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.Configuration.AutoDetectChangesEnabled = false;

                    List<StockCalendar> modifiedCalendars = updatedHolidays.Aggregate(
                        new List<StockCalendar>(),
                        (modified, calendar) =>
                        {
                            modified.Add(UpdateHolidays(context, calendar));
                            return modified;
                        });

                    context.ChangeTracker.DetectChanges();
                    context.SaveChanges();
                    transaction.Commit();
                    return modifiedCalendars;
                }
                catch (SqlException saveError)
                {
                    Logger.Error(saveError, "Error saving updated holidays\n");
                    return new List<StockCalendar>();
                }
            }

            StockCalendar UpdateHolidays(MandaraEntities context, StockCalendar holidayCalendar)
            {
                StockCalendar existingCalendar = context.StockCalendars.Include(calendar => calendar.Holidays)
                                                        .SingleOrDefault(
                                                            calendar => calendar.CalendarId
                                                                        == holidayCalendar.CalendarId);

                if (existingCalendar == null)
                {
                    return null;
                }

                existingCalendar.Name = holidayCalendar.Name;

                RemoveExistingHolidays(existingCalendar, context);
                AddHolidaysToCalendar(holidayCalendar, existingCalendar, context);
                ProductManager.SetIsChangedForRelatedProducts(context, existingCalendar.CalendarId);
                return existingCalendar;
            }

            void RemoveExistingHolidays(StockCalendar existingCalendar, MandaraEntities context)
            {
                List<CalendarHoliday> holidays = existingCalendar
                                                 .Holidays.Where(
                                                     h => h.HolidayDate.Year == year && h.HolidayDate.Month == month)
                                                 .ToList();

                foreach (CalendarHoliday holiday in holidays)
                {
                    existingCalendar.Holidays.Remove(holiday);
                    context.CalendarHolidays.Remove(holiday);
                }
            }

            void AddHolidaysToCalendar(StockCalendar holidayCalendar, StockCalendar existingCalendar, MandaraEntities context)
            {
                holidayCalendar.Holidays.ToList().ForEach(
                    h =>
                    {
                        CalendarHoliday holiday = new CalendarHoliday() { HolidayDate = h.HolidayDate };

                        existingCalendar.Holidays.Add(holiday);
                        context.CalendarHolidays.Add(holiday);
                    });
            }
        }

        /// <summary>
        /// Add new calendar from holidays control
        /// </summary>
        /// <param name="added">Collection of new calendar(s) with holidays</param>
        public List<StockCalendar> AddCalendarWithHolidayDates(List<NewHolidayCalendar> added)
        {
            if (!added.Any())
            {
                return new List<StockCalendar>();
            }

            using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
            using (DbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<StockCalendar> newCalendars = added.Aggregate(
                        new List<StockCalendar>(),
                        (addedCalendars, newHoliday) =>
                        {
                            StockCalendar calendar = new StockCalendar()
                            {
                                Name = newHoliday.Name,
                                CalendarType = CalendarType.Holidays
                            };

                            dbContext.StockCalendars.Add(calendar);
                            newHoliday.Holidays.ForEach(itm => { calendar.Holidays.Add(itm); });
                            addedCalendars.Add(calendar);
                            return addedCalendars;
                        });

                    dbContext.SaveChanges();
                    transaction.Commit();
                    return newCalendars;
                }
                catch (SqlException saveError)
                {
                    Logger.Error(saveError, "Failed to save new holiday calendar\n");
                    return new List<StockCalendar>();
                }
            }
        }
    }
}
