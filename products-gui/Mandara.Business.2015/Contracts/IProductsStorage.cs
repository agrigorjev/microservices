using Mandara.Entities;
using Mandara.Extensions.Option;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mandara.Date;
using Optional;

namespace Mandara.Business.Contracts
{
    public interface IProductsStorage
    {
        [Obsolete]
        Dictionary<int, SortedSet<DateTime>> CalendarHolidays { get; }
        Dictionary<int, Dictionary<DateTime, CalendarExpiryDate>> ExpiryDates { get; }
        Dictionary<int, Dictionary<DateTime, CalendarExpiryDate>> ExpiryDatesByMonths { get; }
        [Obsolete]
        Dictionary<int, Product> Products { get; }
        [Obsolete]
        Dictionary<int, OfficialProduct> OfficialProducts { get; }

        void Update();

        TryGetResult<Product> TryGetProduct(int productId);

        Option<Product> GetProduct(int productId);

        IList<Product> ProductsByName(Regex productNameRegex);

        TryGetResult<Product> TryGetProduct(int productId, MandaraEntities productsContext);

        TryGetResult<Product> TryGetProductWithUpdate(int productId);

        TryGetResult<OfficialProduct> TryGetOfficialProduct(int officialProductId);

        TryGetResult<OfficialProduct> TryGetOfficialProduct(MandaraEntities dbContext, int officialProductId);

        IEnumerable<Product> GetProducts();

        List<Product> GetProducts(Func<Product, bool> filter);

        HashSet<int> GetFuturesProductIds();

        IEnumerable<OfficialProduct> GetOfficialProducts();

        IEnumerable<OfficialProduct> GetOfficialProducts(
            Func<IEnumerable<OfficialProduct>, IEnumerable<OfficialProduct>> filterOfficialProducts);

        bool HasHoliday(int calendarId, DateTime dateToCheck);

        int CountHolidaysInMonth(int calendarId, DateTime month);

        int CountRemainingHolidaysInMonth(int calendarId, DateTime startDate);

        List<DateTime> GetHolidaysInPeriod(int calendarId, DateRange periodToCheck);

        DateTime? GetExpiryDate(Product product, int productYear, int productMonth);

        IEnumerable<string> GetAllCurrenciesUsed();

        IEnumerable<Currency> GetAllCurrencies();

        DateTime GetTasActivationDate(Product product, DateTime tradeDate);

        List<int> GetNymexTasProductIdsWithGivenHoliday(DateTime holidayDate);

        TryGetResult<Product> TryGetFxProductForCurrencyPair(string isoName1, string isoName2);

        TryGetResult<Product> TryGetProductByContractCode(string contractCode, string exchangeName);

        Currency GetOfficialProductCurrency(int officialProductId);

        void SanitiseCircularReferences();

        IEnumerable<ProductCategory> GetProductCategories();
    }
}