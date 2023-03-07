using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Mandara.Business.Contracts;
using Mandara.Entities;
using Mandara.Entities.Enums;

namespace Mandara.Business.OldCode
{
    public class CalculationCache
    {
        private static Func<MandaraEntities> _productsDb;
        public static Func<MandaraEntities> ProductsDb
        {
            get => _productsDb ?? (_productsDb = CreateMandaraProductsDbContext);
            set => _productsDb = value ?? CreateMandaraProductsDbContext;
        }

        public Dictionary<int, SortedSet<CalendarHoliday>> CalendarHolidaysMap { get; private set; }
        public Dictionary<int, Dictionary<DateTime, CalendarExpiryDate>> ExpiryDatesMap { get; private set; }
        public Dictionary<int, Dictionary<DateTime, CalendarExpiryDate>> ExpiryDatesByMonthsMap { get; private set; }

        public List<CalendarExpiryDate> CalendarExpiryDates { get; private set; }
        public List<CalendarHoliday> CalendarHolidays { get; private set; }

        private Dictionary<int, Product> _productsMap;

        public CalculationCache Initialize(MandaraEntities context)
        {
            InitializeCalendars(context);
            BuildProductsMap(context);
            return this;
        }

        private void BuildProductsMap(MandaraEntities context)
        {
            DbQuery<Product> productsQuery =
                context.Products.Include("BalmoOnCrudeProduct")
                    .Include("BalmoOnComplexProduct")
                    .Include("Category")
                    .Include("CategoryOverride")
                    .Include("HolidaysCalendar")
                    .Include("ExpiryCalendar")
                    .Include("OfficialProduct")
                    .Include("UnderlyingFutures")
                    .Include("ComplexProduct");

            List<Product> products = productsQuery.ToList();

            _productsMap = products.ToDictionary(p => p.ProductId, p => p);
        }

        public Product GetProductById(int productId)
        {
            Product product;
            if (!_productsMap.TryGetValue(productId, out product))
            {
                using (var cxt = ProductsDb())
                {
                    BuildProductsMap(cxt);
                }

                _productsMap.TryGetValue(productId, out product);
            }

            return product;
        }

        public static MandaraEntities CreateMandaraProductsDbContext()
        {
            return new MandaraEntities(MandaraEntities.DefaultConnStrName, nameof(CalculationCache));
        }

        private class CalendarHolidayComparer : Comparer<CalendarHoliday>
        {
            public override int Compare(CalendarHoliday x, CalendarHoliday y)
            {
                return DateTime.Compare(x.HolidayDate, y.HolidayDate);
            }
        }

        private void BuildCalendarHolidaysMap()
        {
            CalendarHolidaysMap = new Dictionary<int, SortedSet<CalendarHoliday>>();

            var comparer = new CalendarHolidayComparer();

            foreach (CalendarHoliday calendarHoliday in CalendarHolidays)
            {
                SortedSet<CalendarHoliday> set;

                if (CalendarHolidaysMap.TryGetValue(calendarHoliday.CalendarId, out set))
                {
                    set.Add(calendarHoliday);
                }
                else
                {
                    set = new SortedSet<CalendarHoliday>(comparer);
                    set.Add(calendarHoliday);

                    CalendarHolidaysMap.Add(calendarHoliday.CalendarId, set);
                }
            }
        }

        public void Initialize(IProductsStorage productStorage)
        {
            using (MandaraEntities context = ProductsDb())
            {
                InitializeCalendars(context);
                // TODO: Oh, the horror that this is duplicating stuff that ProductStorage already does.
                _productsMap = productStorage.GetProducts().ToDictionary(product => product.ProductId);
            }
        }

        private void InitializeCalendars(MandaraEntities context)
        {
            List<int?> expiryTypes = new List<int?>
                {
                    (int) CalendarType.ExpiryAndHolidays,
                    (int) CalendarType.Expiry
                };
            DbQuery<CalendarExpiryDate> exDatesQuery = (DbQuery<CalendarExpiryDate>)context.CalendarExpiryDates
                .Where(
                    x => x.StockCalendar.CalendarTypeDb == null ||
                         expiryTypes.Contains(x.StockCalendar.CalendarTypeDb));
            CalendarExpiryDates = exDatesQuery.ToList();

            BuildExpiryDatesMap();
            BuildExpiryDatesByMonthsMap();

            List<int?> holidaysTypes = new List<int?>
                {
                    (int) CalendarType.ExpiryAndHolidays,
                    (int) CalendarType.Holidays
                };
            DbQuery<CalendarHoliday> holQuery = (DbQuery<CalendarHoliday>)context.CalendarHolidays
                .Where(
                    x => x.StockCalendar.CalendarTypeDb == null ||
                         holidaysTypes.Contains(x.StockCalendar.CalendarTypeDb));
            CalendarHolidays = holQuery.ToList();

            BuildCalendarHolidaysMap();
        }

        public int CountHolidaysBetweenDates(
            int calendarId,
            DateTime startDate,
            DateTime endDate,
            bool includeEnd = false)
        {
            return HolidaysBetweenDates(calendarId, startDate, endDate, includeEnd).Count();
        }

        public List<CalendarHoliday> HolidaysBetweenDates(int calendarId, DateTime startDate, DateTime endDate,
            bool includeEnd = false)
        {
            List<CalendarHoliday> holidayDates = new List<CalendarHoliday>();

            if (!CalendarHolidaysMap.TryGetValue(calendarId, out SortedSet<CalendarHoliday> set))
            {
                return holidayDates;
            }

            foreach (CalendarHoliday holiday in set)
            {
                if (holiday.HolidayDate >= startDate && holiday.HolidayDate <= endDate)
                {
                    holidayDates.Add(holiday);
                }
            }

            if (!includeEnd && holidayDates.Count > 0)
            {
                if (holidayDates.Last().HolidayDate == endDate)
                    holidayDates.RemoveAt(holidayDates.Count - 1);
            }

            return holidayDates;
        }

        private void BuildExpiryDatesMap()
        {
            ExpiryDatesMap = new Dictionary<int, Dictionary<DateTime, CalendarExpiryDate>>();

            foreach (CalendarExpiryDate expDate in CalendarExpiryDates)
            {
                Dictionary<DateTime, CalendarExpiryDate> dic;

                if (ExpiryDatesMap.TryGetValue(expDate.CalendarId, out dic))
                {
                    dic.Add(expDate.FuturesDate, expDate);
                }
                else
                {
                    dic = new Dictionary<DateTime, CalendarExpiryDate>();
                    dic.Add(expDate.FuturesDate, expDate);

                    ExpiryDatesMap.Add(expDate.CalendarId, dic);
                }
            }
        }

        private void BuildExpiryDatesByMonthsMap()
        {
            ExpiryDatesByMonthsMap = new Dictionary<int, Dictionary<DateTime, CalendarExpiryDate>>();

            foreach (CalendarExpiryDate expDate in CalendarExpiryDates)
            {
                DateTime key = new DateTime(expDate.ExpiryDate.Year, expDate.ExpiryDate.Month, 1);

                if (!ExpiryDatesByMonthsMap.TryGetValue(
                    expDate.CalendarId,
                    out Dictionary<DateTime, CalendarExpiryDate> firstMonthlyExpiry))
                {
                    firstMonthlyExpiry = new Dictionary<DateTime, CalendarExpiryDate>();

                    ExpiryDatesByMonthsMap.Add(expDate.CalendarId, firstMonthlyExpiry);
                }

                if (!firstMonthlyExpiry.TryGetValue(key, out CalendarExpiryDate _))
                {
                    firstMonthlyExpiry.Add(key, expDate);
                }
            }
        }

        public void Initialize()
        {
            using (MandaraEntities context = ProductsDb())
            {
                Initialize(context);
            }
        }

        public void AddProduct(int productId, Product product)
        {
            if (_productsMap.ContainsKey(productId))
                _productsMap.Remove(productId);

            _productsMap.Add(productId, product);
        }
    }
}