using EntityFramework.Extensions;
using EntityFramework.Future;
using Mandara.Business.Contracts;
using Mandara.Business.DataInterface;
using Mandara.Business.Services.Prices;
using Mandara.Date;
using Mandara.Entities;
using Mandara.Entities.Enums;
using Mandara.Extensions.Collections;
using Mandara.Extensions.Option;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Optional;
using Optional.Collections;
using Optional.Unsafe;

namespace Mandara.Business.Services
{
    public class ProductsStorage : ReplaceableDbContext, IProductsStorage
    {
        private class Queries
        {
            public FutureQuery<CalendarExpiryDate> LoadExpiryDates { get; private set; }
            public FutureQuery<CalendarHoliday> LoadHolidays { get; private set; }
            public FutureQuery<Product> LoadProducts { get; private set; }
            public FutureQuery<OfficialProduct> LoadOfficialProducts { get; private set; }

            public Queries(
                FutureQuery<CalendarExpiryDate> loadExpiries,
                FutureQuery<CalendarHoliday> loadHolidays,
                FutureQuery<Product> loadProducts,
                FutureQuery<OfficialProduct> loadOffProds)
            {
                LoadExpiryDates = loadExpiries;
                LoadHolidays = loadHolidays;
                LoadProducts = loadProducts;
                LoadOfficialProducts = loadOffProds;
            }
        }

        public Dictionary<int, SortedSet<DateTime>> CalendarHolidays { get; private set; }
        public Dictionary<int, Dictionary<DateTime, CalendarExpiryDate>> ExpiryDates { get; private set; }
        public Dictionary<int, Dictionary<DateTime, CalendarExpiryDate>> ExpiryDatesByMonths { get; private set; }
        public Dictionary<int, Product> Products { get; set; }
        public Dictionary<int, OfficialProduct> OfficialProducts { get; set; }

        private List<ProductCategory> ProductCategories { get; set; }
        private HashSet<string> _allCurrenciesUsed;
        private readonly MonthlyBusinessDayCalculator _businessDayCalculator;
        private ICurrencyProvider _currencies;
        private AutoResetEvent _serialiseUpdating = new AutoResetEvent(true);

        public ProductsStorage(ICurrencyProvider currencies)
        {
            CalendarHolidays = new Dictionary<int, SortedSet<DateTime>>();
            ExpiryDates = new Dictionary<int, Dictionary<DateTime, CalendarExpiryDate>>();
            ExpiryDatesByMonths = new Dictionary<int, Dictionary<DateTime, CalendarExpiryDate>>();
            Products = new Dictionary<int, Product>();
            OfficialProducts = new Dictionary<int, OfficialProduct>();
            ProductCategories = new List<ProductCategory>();
            _allCurrenciesUsed = new HashSet<string>();
            _businessDayCalculator = new MonthlyBusinessDayCalculator(this);
            _currencies = currencies;
        }

        public IEnumerable<Product> GetProducts()
        {
            return Products.Values;
        }

        public List<Product> GetProducts(Func<Product, bool> filter)
        {
            return Products.Values.Where(filter).ToList();
        }

        public HashSet<int> GetFuturesProductIds()
        {
            return Products.Values.Where(product => product.ProductTypeDb == (short)ProductType.Futures)
                           .Select(product => product.ProductId)
                           .ToHashSet();
        }

        public IEnumerable<OfficialProduct> GetOfficialProducts()
        {
            return OfficialProducts.Values;
        }

        public bool HasHoliday(int calendarId, DateTime dateToCheck)
        {
            return CalendarHolidays.TryGetValue(calendarId, out SortedSet<DateTime> holidays)
                   && holidays.Contains(dateToCheck);
        }

        public int CountHolidaysInMonth(int calendarId, DateTime month)
        {
            return CountHolidays(calendarId, month, CountHolidaysInMonth);
        }

        private int CountHolidays(
            int calendarId,
            DateTime baseDate,
            Func<SortedSet<DateTime>, DateTime, int> counter)
        {
            return CalendarHolidays.TryGetValue(calendarId, out SortedSet<DateTime> holidays)
                ? counter(holidays, baseDate)
                : 0;
        }

        private int CountHolidaysInMonth(SortedSet<DateTime> holidays, DateTime month)
        {
            return holidays.Count(holiday => holiday.Year == month.Year && holiday.Month == month.Month);
        }

        public int CountRemainingHolidaysInMonth(int calendarId, DateTime startDate)
        {
            return CountHolidays(calendarId, startDate, CountRemainingHolidaysInMonth);
        }

        private int CountRemainingHolidaysInMonth(SortedSet<DateTime> holidays, DateTime startDate)
        {
            return holidays.Count(
                holiday => holiday.Year == startDate.Year
                           && holiday.Month == startDate.Month
                           && holiday.Day > startDate.Day);
        }

        public List<DateTime> GetHolidaysInPeriod(int calendarId, DateRange periodToCheck)
        {
            if (!CalendarHolidays.TryGetValue(calendarId, out SortedSet<DateTime> holidays))
            {
                return new List<DateTime>();
            }

            return holidays.Where(holiday => periodToCheck.Start <= holiday && periodToCheck.End >= holiday).ToList();
        }

        public IEnumerable<OfficialProduct> GetOfficialProducts(
            Func<IEnumerable<OfficialProduct>, IEnumerable<OfficialProduct>> filterOfficialProducts)
        {
            return filterOfficialProducts(OfficialProducts.Values);
        }

        public DateTime? GetExpiryDate(Product product, int productYear, int productMonth)
        {
            DateTime expirationMonth = new DateTime(productYear, productMonth, 1);

            // trying to retrieve expiryDate from the CalculationCache
            if (!ExpiryDates.TryGetValue(
                product.ExpiryCalendar.CalendarId,
                out Dictionary<DateTime, CalendarExpiryDate> calendarExpiryDates))
            {
                return null;
            }

            if (!calendarExpiryDates.TryGetValue(expirationMonth, out CalendarExpiryDate calendarExpiryDate))
            {
                return null;
            }

            DateTime expiryDate = calendarExpiryDate.ExpiryDate;

            List<DateTime> holidays = new List<DateTime>();

            if (CalendarHolidays.TryGetValue(product.HolidaysCalendar.CalendarId, out SortedSet<DateTime> holidaysSet))
            {
                holidays = holidaysSet.ToList();
            }

            int rollingSign = 1;
            bool isCorrectDay = false;

            do
            {
                isCorrectDay = true;

                if (expiryDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    expiryDate = expiryDate.AddDays(rollingSign);
                    isCorrectDay = false;
                }

                if (expiryDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    expiryDate = expiryDate.AddDays(rollingSign);
                    isCorrectDay = false;
                }

                if (holidays.Contains(expiryDate))
                {
                    expiryDate = expiryDate.AddDays(rollingSign);
                    isCorrectDay = false;
                }
            }
            while (isCorrectDay == false);

            return expiryDate;
        }

        public void Update()
        {
            _serialiseUpdating.WaitOne();

            try
            {
                _currencies.Update(Enumerable.Empty<int>());

                using (MandaraEntities dbContext = _dbContextCreator())
                {
                    dbContext.Database.CommandTimeout = 0;

                    Queries loaders = PrepareQueries(dbContext);

                    LoadCalendars(loaders.LoadExpiryDates);
                    LoadHolidays(loaders.LoadHolidays);
                    Products = LoadProducts(loaders.LoadProducts, GetProductCurrencySetter(dbContext));

                    HashSet<string> productsCurrencyIsoNames = GetProductsCurrencies(Products.Values);

                    ProductCategories = dbContext.ProductCategories.ToList();
                    OfficialProducts = LoadOfficialProducts(loaders.LoadOfficialProducts);

                    IEnumerable<Currency> officialProdCurrencies = GetCurrencies();

                    SetAllCurrenciesUsed(productsCurrencyIsoNames, officialProdCurrencies);
                }
            }
            finally
            {
                _serialiseUpdating.Set();
            }
        }

        private Queries PrepareQueries(MandaraEntities dbContext)
        {
            return new Queries(
                GetExpiryDatesQuery(dbContext),
                GetHolidaysQuery(dbContext),
                GetProductsQuery(dbContext),
                GetOfficialProductsQuery(dbContext));
        }

        private FutureQuery<CalendarExpiryDate> GetExpiryDatesQuery(MandaraEntities dbContext)
        {
            List<int?> expiryTypes = new List<int?> { (int)CalendarType.ExpiryAndHolidays, (int)CalendarType.Expiry };

            return dbContext.CalendarExpiryDates.Where(
                calendar => calendar.StockCalendar.CalendarTypeDb == null
                            || expiryTypes.Contains(calendar.StockCalendar.CalendarTypeDb)).Future();
        }

        private FutureQuery<CalendarHoliday> GetHolidaysQuery(MandaraEntities dbContext)
        {
            List<int?> holidaysTypes =
                new List<int?> { (int)CalendarType.ExpiryAndHolidays, (int)CalendarType.Holidays };

            return dbContext.CalendarHolidays.Where(
                calendar => calendar.StockCalendar.CalendarTypeDb == null
                            || holidaysTypes.Contains(calendar.StockCalendar.CalendarTypeDb)).Future();
        }

        private FutureQuery<Product> GetProductsQuery(MandaraEntities dbContext)
        {
            return GetProductLoadQuery(dbContext).Future();
        }

        private FutureQuery<OfficialProduct> GetOfficialProductsQuery(MandaraEntities dbContext)
        {
            return dbContext.OfficialProducts.Future();
        }

        private Action<Product> GetProductCurrencySetter(MandaraEntities dbContext)
        {
            return (product) => SetProductCurrencies(product, dbContext);
        }

        private void LoadCalendars(FutureQuery<CalendarExpiryDate> loadExpiryDates)
        {
            List<CalendarExpiryDate> expiryDates = loadExpiryDates.ToList();

            BuildExpiryDatesByCalendar(expiryDates);
            BuildExpiryDatesByMonthsMap(expiryDates);
        }

        private void LoadHolidays(FutureQuery<CalendarHoliday> loadHolidays)
        {
            BuildCalendarHolidaysMap(loadHolidays.ToList());
        }

        private Dictionary<int, Product> LoadProducts(FutureQuery<Product> loadProducts, Action<Product> setCurrencies)
        {
            return loadProducts.Aggregate(
                new Dictionary<int, Product>(),
                (products, product) =>
                {
                    SetProductExpiryCalendar(product);
                    setCurrencies(product);
                    products.Add(product.ProductId, product);

                    return products;
                });
        }

        private void SetProductExpiryCalendar(Product product)
        {
            product.ExpiryCalendar.FuturesExpiries = ExpiryDates[product.calendar_id]
                                                     .Values.OrderBy(expiry => expiry.FuturesDate).ToList();
        }

        private HashSet<string> GetProductsCurrencies(IEnumerable<Product> products)
        {
            return products.SelectMany(product => product.CurrencyNames()).ToHashSet();
        }

        private Dictionary<int, OfficialProduct> LoadOfficialProducts(
            FutureQuery<OfficialProduct> loadOfficialProducts)
        {
            return loadOfficialProducts.ToDictionary(offProd => offProd.OfficialProductId);
        }

        private List<Currency> GetCurrencies()
        {
            return OfficialProducts.Values.Select(SetOfficialProductCurrency).ToList();
        }

        private Currency SetOfficialProductCurrency(OfficialProduct offProd)
        {
            offProd.Currency = _currencies.TryGetCurrency(offProd.CurrencyId).Value;
            _allCurrenciesUsed.Add(offProd.Currency.IsoName);

            return offProd.Currency;
        }

        private void SetAllCurrenciesUsed(
            HashSet<string> usedCurrencyIsoNames,
            IEnumerable<Currency> officialProdCurrencies)
        {
            _allCurrenciesUsed = officialProdCurrencies.Aggregate(
                usedCurrencyIsoNames,
                (currenciesUsed, currency) =>
                {
                    currenciesUsed.Add(currency.IsoName);

                    return currenciesUsed;
                });
        }

        private IQueryable<Product> GetProductLoadQuery(MandaraEntities dbContext)
        {
            return dbContext.Products
                            .Include(product => product.BalmoOnCrudeProduct)
                            .Include(product => product.Unit)
                            .Include(product => product.Exchange)
                            .Include(product => product.BalmoOnComplexProduct)
                            .Include(product => product.Category)
                            .Include(product => product.Category.Classes)
                            .Include(product => product.CategoryOverride)
                            .Include(product => product.HolidaysCalendar)
                            .Include(product => product.ExpiryCalendar)
                            .Include(product => product.OfficialProduct)
                            .Include(product => product.OfficialProduct.Currency)
                            .Include(product => product.UnderlyingFutures)
                            .Include(product => product.Aliases)
                            .Include(product => product.ComplexProduct)
                            .Include(product => product.ComplexProduct.ChildProduct1)
                            .Include(product => product.ComplexProduct.ChildProduct1.OfficialProduct)
                            .Include(product => product.ComplexProduct.ChildProduct1.HolidaysCalendar)
                            .Include(product => product.ComplexProduct.ChildProduct2)
                            .Include(product => product.ComplexProduct.ChildProduct2.OfficialProduct)
                            .Include(product => product.ComplexProduct.ChildProduct2.HolidaysCalendar);
        }

        /// <summary>
        ///     Because the Update method is doing so much, this method just gets a single product and does not add it
        ///     to local storage because there's no guarantee that the required expiry and other information will
        ///     already be held locally.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public TryGetResult<Product> TryGetProduct(int productId)
        {
            Option<Product> product = GetProduct(productId);

            return product.HasValue ? new TryGetRef<Product>(product.ValueOrFailure()) : new TryGetRef<Product>();
        }

        /// <summary>
        ///     Because the Update method is doing so much, this method just gets a single product and does not add it
        ///     to local storage because there's no guarantee that the required expiry and other information will
        ///     already be held locally.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public Option<Product> GetProduct(int productId)
        {
            if (!Products.TryGetValue(productId, out Product product))
            {
                using (MandaraEntities dbContext = _dbContextCreator())
                {
                    product = LoadSingleProduct(productId, dbContext);
                }
            }

            return null != product ? Option.Some(product) : Option.None<Product>();
        }

        private Product LoadSingleProduct(int productId, MandaraEntities dbContext)
        {
            // TODO: Remove the use of null here
            Product product = GetProductQuery(productId, dbContext).FirstOrDefault();

            if (null == product)
            {
                return product;
            }

            SetProductCurrencies(product, dbContext);
            product.CurrencyNames().ForEach(currency => _allCurrenciesUsed.Add(currency));
            AddProductCalendarsToMemory(product);
            product.OfficialProduct = GetOfficialProduct(product.OfficialProductId, dbContext);
            AddProductCategoriesToMemory(product.Category, product.CategoryOverride);
            Products.Add(productId, product);

            return product;
        }

        private FutureQuery<Product> GetProductQuery(int productId, MandaraEntities dbContext)
        {
            return GetProductLoadQuery(dbContext)
                   .Include(product => product.HolidaysCalendar.Holidays)
                   .Include(product => product.ExpiryCalendar.FuturesExpiries)
                   .Include(product => product.ComplexProduct.ChildProduct1.HolidaysCalendar.Holidays)
                   .Include(product => product.ComplexProduct.ChildProduct2.HolidaysCalendar.Holidays)
                   .Include(product => product.ComplexProduct.ChildProduct1.ExpiryCalendar.FuturesExpiries)
                   .Include(product => product.ComplexProduct.ChildProduct2.ExpiryCalendar.FuturesExpiries)
                   .Include(product => product.Category)
                   .Include(product => product.Category.SwapCrossPerProducts)
                   .Where(product => product.ProductId == productId).Future();
        }

        private void AddProductCalendarsToMemory(Product product)
        {
            AddExpiryCalendarToMemory(product.ExpiryCalendar);
            AddHolidayCalendarToMemory(product.HolidaysCalendar);

            if (product.IsComplexProduct())
            {
                AddExpiryCalendarToMemory(product.ComplexProduct.ChildProduct1.ExpiryCalendar);
                AddHolidayCalendarToMemory(product.ComplexProduct.ChildProduct1.HolidaysCalendar);
                AddExpiryCalendarToMemory(product.ComplexProduct.ChildProduct2.ExpiryCalendar);
                AddHolidayCalendarToMemory(product.ComplexProduct.ChildProduct2.HolidaysCalendar);
            }
        }

        private void AddExpiryCalendarToMemory(StockCalendar calendar)
        {
            if (null == calendar || ExpiryDates.ContainsKey(calendar.CalendarId))
            {
                return;
            }

            ExpiryDates[calendar.CalendarId] = calendar.FuturesExpiries.ToDictionary(expiry => expiry.FuturesDate);
            ExpiryDatesByMonths[calendar.CalendarId] = CreateExpiryDatesByMonth(calendar.FuturesExpiries);
        }

        private void AddHolidayCalendarToMemory(StockCalendar calendar)
        {
            if (null == calendar || CalendarHolidays.ContainsKey(calendar.CalendarId))
            {
                return;
            }

            CalendarHolidays[calendar.CalendarId] =
                new SortedSet<DateTime>(calendar.Holidays.Select(holiday => holiday.HolidayDate));
        }

        private OfficialProduct GetOfficialProduct(int offProdId, MandaraEntities dbContext)
        {
            if (OfficialProducts.TryGetValue(offProdId, out OfficialProduct offProd))
            {
                return offProd;
            }

            offProd = TryGetOfficialProduct(dbContext, offProdId).Value;
            OfficialProducts[offProdId] = offProd;
            SetOfficialProductCurrency(offProd);

            return offProd;
        }

        private void AddProductCategoriesToMemory(ProductCategory baseCategory, ProductCategory overrideCategory)
        {
            AddProductCategoryToMemory(baseCategory);
            AddProductCategoryToMemory(overrideCategory);
        }

        private void AddProductCategoryToMemory(ProductCategory categoryToAdd)
        {
            if (null == categoryToAdd
                || ProductCategories.Exists(category => category.CategoryId == categoryToAdd.CategoryId))
            {
                return;
            }

            ProductCategories.Add(categoryToAdd);
        }

        public IList<Product> ProductsByName(Regex nameMatches) =>
            Products.Values.Where(product => nameMatches.IsMatch(product.Name)).ToList();

        public TryGetResult<Product> TryGetProduct(int productId, MandaraEntities dbContext)
        {
            if (!Products.TryGetValue(productId, out Product product))
            {
                product = LoadSingleProduct(productId, dbContext);
            }

            return new TryGetRef<Product>(product);
        }

        private void SetProductCurrencies(Product product, MandaraEntities context)
        {
            SetCurrency(product.Currency1Id, currency => product.Currency1 = currency, context);
            SetCurrency(product.Currency2Id, currency => product.Currency2 = currency, context);
            SetCurrency(product.FeeClearingCurrencyId, currency => product.FeeClearingCurrency = currency, context);
            SetCurrency(product.FeeBlockCurrencyId, currency => product.FeeBlockCurrency = currency, context);
            SetCurrency(product.FeeCommissionCurrencyId, currency => product.FeeCommissionCurrency = currency, context);
            SetCurrency(product.FeeExchangeCurrencyId, currency => product.FeeExchangeCurrency = currency, context);
            SetCurrency(product.FeeNfaCurrencyId, currency => product.FeeNfaCurrency = currency, context);
            SetCurrency(product.FeePlattsCurrencyId, currency => product.FeePlattsCurrency = currency, context);
            SetCurrency(product.FeeCashCurrencyId, currency => product.FeeCashCurrency = currency, context);
        }

        private void SetCurrency(int? currencyId, Action<Currency> fieldSetter, MandaraEntities dbContext)
        {
            TryGetResult<Currency> currency = currencyId.HasValue
                ? GetCurrency(currencyId.Value, dbContext)
                : new TryGetRef<Currency>((Currency)null);

            if (currency.HasValue)
            {
                fieldSetter(currency.Value);
            }
        }

        private TryGetResult<Currency> GetCurrency(int currencyId, MandaraEntities dbContext)
        {
            return _currencies.TryGetCurrency(currencyId, dbContext);
        }

        /// <summary>
        ///     Because the Update method is doing so much, this method just gets a single product and does not add it
        ///     to local storage because there's no guarantee that the required expiry and other information will
        ///     already be held locally.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public TryGetResult<Product> TryGetProductWithUpdate(int productId)
        {
            if (!Products.TryGetValue(productId, out Product product))
            {
                Update();
                Products.TryGetValue(productId, out product);
            }

            return new TryGetRef<Product>() { Value = product };
        }

        /// <summary>
        ///     Because the Update method is doing so much, this method just gets a single official product and does not
        ///     add it to local storage because there's no guarantee that the required expiry and other information will
        ///     already be held locally.
        /// </summary>
        /// <param name="officialProductId"></param>
        /// <returns></returns>
        public TryGetResult<OfficialProduct> TryGetOfficialProduct(int officialProductId)
        {
            using (MandaraEntities dbContext = _dbContextCreator())
            {
                return TryGetOfficialProduct(dbContext, officialProductId);
            }
        }

        public TryGetResult<OfficialProduct> TryGetOfficialProduct(MandaraEntities dbContext, int officialProductId)
        {
            if (!OfficialProducts.TryGetValue(officialProductId, out OfficialProduct officialProduct))
            {
                officialProduct = dbContext.OfficialProducts.Include(x => x.Currency)
                                           .SingleOrDefault(x => x.OfficialProductId == officialProductId);
            }

            TryGetResult<OfficialProduct> officialProdResult = new TryGetRef<OfficialProduct>()
            {
                Value = officialProduct
            };

            return officialProdResult;
        }

        private void BuildCalendarHolidaysMap(List<CalendarHoliday> calendarHolidays)
        {
            CalendarHolidays = new Dictionary<int, SortedSet<DateTime>>();

            foreach (CalendarHoliday calendarHoliday in calendarHolidays)
            {
                if (CalendarHolidays.TryGetValue(calendarHoliday.CalendarId, out SortedSet<DateTime> set))
                {
                    set.Add(calendarHoliday.HolidayDate);
                }
                else
                {
                    set = new SortedSet<DateTime> { calendarHoliday.HolidayDate };

                    CalendarHolidays.Add(calendarHoliday.CalendarId, set);
                }
            }
        }

        public int CountHolidaysBetweenDates(
            int calendarId,
            DateTime startDate,
            DateTime endDate,
            bool includeEnd = false)
        {
            SortedSet<DateTime> set = CalendarHolidays[calendarId];
            List<DateTime> holidayDates = new List<DateTime>();

            foreach (DateTime dateTime in set)
            {
                if (dateTime >= startDate && dateTime <= endDate)
                {
                    holidayDates.Add(dateTime);
                }
            }

            if (!includeEnd && holidayDates.Count > 0)
            {
                if (holidayDates.Last() == endDate)
                {
                    return holidayDates.Count - 1;
                }
            }

            return holidayDates.Count;
        }

        private void BuildExpiryDatesByCalendar(List<CalendarExpiryDate> allExpiryDates)
        {
            ExpiryDates = allExpiryDates.GroupBy(expiry => expiry.CalendarId).ToDictionary(
                calendarExpiries => calendarExpiries.Key,
                calendarExpiries => calendarExpiries.ToDictionary(
                    expiry => expiry.FuturesDate,
                    expiry => expiry));
        }

        private void BuildExpiryDatesByMonthsMap(List<CalendarExpiryDate> calendarExpiryDates)
        {
            ExpiryDatesByMonths = calendarExpiryDates.GroupBy(expiryDates => expiryDates.CalendarId).ToDictionary(
                expiries => expiries.Key,
                expiries => CreateExpiryDatesByMonth(expiries.ToList()));
        }

        private static Dictionary<DateTime, CalendarExpiryDate> CreateExpiryDatesByMonth(
            ICollection<CalendarExpiryDate> expiries)
        {
            return expiries.DistinctWhere(expiry => expiry.ExpiryDate.FirstDayOfMonth()).ToDictionary(
                expiry => expiry.ExpiryDate.FirstDayOfMonth(),
                expiry => expiry);
        }

        public IEnumerable<string> GetAllCurrenciesUsed()
        {
            return _allCurrenciesUsed;
        }

        public IEnumerable<Currency> GetAllCurrencies()
        {
            return _currencies.Currencies();
        }

        public DateTime GetTasActivationDate(Product product, DateTime tradeDate)
        {
            DateTime tasActivationDate = tradeDate;

            if (tradeDate.IsWeekendDay())
            {
                if (tradeDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    tasActivationDate = tradeDate.AddDays(2);
                }
                else
                {
                    tasActivationDate = tradeDate.AddDays(1);
                }
            }

            if (product.IsNymex())
            {
                while (_businessDayCalculator.IsTodayHoliday(tasActivationDate, product.holidays_calendar_id.Value))
                {
                    tasActivationDate = tasActivationDate.AddDays(1);
                }
            }

            return tasActivationDate;
        }

        public List<int> GetNymexTasProductIdsWithGivenHoliday(DateTime holiday)
        {
            return Products.Values.Where(
                               product => TasType.NotTas != product.TasType
                                          && product.IsNymex()
                                          && product.holidays_calendar_id.HasValue
                                          && CalendarHolidays.ContainsKey(product.holidays_calendar_id.Value)
                                          && CalendarHolidays[product.holidays_calendar_id.Value].Contains(holiday))
                           .Select(p => p.ProductId).ToList();
        }

        public TryGetResult<Product> TryGetFxProductForCurrencyPair(string isoName1, string isoName2)
        {
            Product product = Products.Values.FirstOrDefault(
                x => x.Currency1?.IsoName == isoName1 && x.Currency2?.IsoName == isoName2);
            return new TryGetRef<Product>() { Value = product };
        }

        public TryGetResult<Product> TryGetProductByContractCode(string contractCode, string exchangeName)
        {
            Product product = Products.Values.FirstOrDefault(
                x => x.ExchangeContractCode == contractCode
                     && x.Exchange.Name == exchangeName);

            return new TryGetRef<Product>() { Value = product };
        }

        public Currency GetOfficialProductCurrency(int officialProductId)
        {
            return TryGetOfficialProduct(officialProductId).Value.Currency;
        }

        public void SanitiseCircularReferences()
        {
            GetProducts().ForEach(prod => prod.SanitiseCircularReferences());
        }

        public IEnumerable<ProductCategory> GetProductCategories()
        {
            return ProductCategories;
        }
    }
}