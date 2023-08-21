using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Mandara.Business.Audit;
using Mandara.Business.Contracts;
using Mandara.Business.Data;
using Mandara.Business.Managers;
using Mandara.Business.Managers.Calendars;
using Mandara.Business.Managers.Products;
using Mandara.Date;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Entities.Enums;
using Mandara.Entities.Extensions;
using Mandara.Entities.Parser;
using Mandara.Extensions.Guids;
using Mandara.Extensions.Option;
using Optional;

namespace Mandara.Business
{
    public class ProductManager
    {
        private readonly Lazy<GmiCodesMapper> _lazyGmiCodesMapper =
            new Lazy<GmiCodesMapper>(() => new GmiCodesMapper());

        private static readonly string[] IsoCurrencies = Enum.GetNames(typeof(CurrencyIso4217));
        private readonly CurrencyProvider _currencies = new CurrencyProvider();

        private readonly UnitsProvider _units = new UnitsProvider();

        public GmiCodesMapper GmiCodesMapper => _lazyGmiCodesMapper.Value;

        private List<Product> _products;
        private List<ABNMappings> _mappings;
        private readonly int _precalcSourceDetailsMonthsBack;

        private readonly ProductCalendars _calendars;
        private readonly OfficialProductsManager _officialProducts;

        public ProductManager()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["PrecalcSourceDetails_MonthsBack"], out int months))
            {
                months = 6;
            }

            _precalcSourceDetailsMonthsBack = months;
            _calendars = new ProductCalendars();
            _officialProducts = new OfficialProductsManager();
        }

        public Product GetProductWithValidityCheck(DateTime effectiveDate, string productName)
        {
            TryGetResult<Product> product = TryGetProductWithValidityCheck(effectiveDate, productName);
            if (!product.HasValue)
            {
                throw new FormatException($"The alias {productName} does not map to a product");
            }

            return product.Value;
        }

        public TryGetResult<Product> TryGetProductWithValidityCheck(DateTime effectiveDate, string productName)
        {
            if (_products == null)
            {
                _products = GetProducts();
            }

            Product product = _products.FirstOrDefault(
                p => p.Name == productName
                     && (p.ValidFrom ?? DateTime.MinValue) <= effectiveDate
                     && (p.ValidTo ?? DateTime.MaxValue) >= effectiveDate);

            return new TryGetRef<Product>(product);
        }

        /// <summary>
        /// Try to restore GMI code. First from ABN MAppings then from GMI Indexes
        /// </summary>
        /// <param name="productId">id of product</param>
        /// <param name="tradeDate">trade date</param>
        /// <returns>strin of ode or empty string</returns>
        public string ABN_GMI_CodeRestore(int productId, DateTime? tradeDate, DateTime? tradeStartDate)
        {
            if (_mappings == null)
            {
                _mappings = GetABNMappingsWProduct();
            }

            string result = _mappings.Where(
                                         m => m.Product.ProductId == productId
                                              && (tradeDate == null
                                                  || ((m.Product.ValidFrom == null
                                                          ? DateTime.MinValue
                                                          : m.Product.ValidFrom.Value)
                                                      <= tradeDate.Value
                                                      && (m.Product.ValidTo == null
                                                          ? DateTime.MaxValue
                                                          : m.Product.ValidTo.Value)
                                                      >= tradeDate.Value)))
                                     .Select(m => $"{m.ExchangeCode}_{m.ProductCode}").FirstOrDefault();

            if (result == null)
            {
                return tradeStartDate == null
                    ? string.Empty
                    : GmiCodesMapper.GMIBalmoCodeForProduct(productId, tradeStartDate.Value.Day);
            }

            return result;
        }

        /// <summary>
        ///Mapping product by given exchange code. Mapping occurs against contract code fields of product object.
        /// </summary>
        /// <param name="code">Exchange code from seals input file</param>
        /// <param name="balmodateComponent">Outgoing value indicating date ot month for Market_Expiry date</param>
        /// <param name="throwExceptions">If break execution by throwing exceptions</param>
        /// <returns>Mapped product, null if not found</returns>
        public Product GetProductByBalmoSealCode(string code, out int balmoDateComponent, bool throwExceptions = true)
        {
            //Get products list if not yet initialized
            if (_products == null)
            {
                _products = GetProducts();
            }

            //Call static
            return GetProductByBalmoSealCode(code, _products, out balmoDateComponent, throwExceptions);
        }

        /// <summary>
        /// Check if given date is a holiday for a product. Returns true if the date is a holiday.
        /// </summary>
        /// <param name="productId">Id of the product</param>
        /// /// <param name="holiday">Date to check</param>
        public bool CheckHoliday(int productId, DateTime holiday)
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                return context.Products.Include(x => x.HolidaysCalendar).Include(x => x.HolidaysCalendar.Holidays)
                              .Single(p => p.ProductId == productId).HolidaysCalendar.Holidays
                              .Any(h => h.HolidayDate == holiday);
            }
        }

        private static MandaraEntities CreateMandaraProductsDbContext()
        {
            return new MandaraEntities(MandaraEntities.DefaultConnStrName, nameof(ProductManager));
        }

        /// <summary>
        ///Static version of mapping product by given exchange code. Mapping occurs against contract code fields of product object.
        /// </summary>
        /// <param name="code">Exchange code from seals input file</param>
        /// <param name="balmodateComponent">Outgoing value indicating date ot month for Market_Expiry date</param>
        /// <param name="inp_products">List of products to check in case of static function use</param>
        /// <param name="throwExceptions">If break execution by throwing exceptions</param>
        /// <returns>Mapped product, null if not found</returns>
        public static Product GetProductByBalmoSealCode(
            string code,
            List<Product> inp_products,
            out int balmoDateComponent,
            bool throwExceptions = true)
        {
            balmoDateComponent = 0;
            if (code.Length > 2)
            {
                //Determine code range indicator
                string searchFor = code.ToUpper().Substring(0, 2);
                //Determine dateIndex
                char dateIndex = code.ToUpper()[2];
                //Inline delegate to check date component of balmo contract code
                Func<Product, string, char, Tuple<Product, int>> checkFitDateCode =
                    delegate (Product p, string s, char check)
                    {
                        //check range of code
                        int section = p.BalmoContractCode1 != s ? (p.BalmoContractCode2 != s ? 3 : 2) : 1;
                        //check alphabet index of date part of code
                        check = char.ToUpper(check);
                        //Check alphabet index of first letter pf first range,If null set to 'A'
                        char startPosition = string.IsNullOrEmpty(p.BalmoCodeFirstLetter)
                            ? 'A'
                            : char.ToUpper(p.BalmoCodeFirstLetter[0]);
                        int result_Index = 0;
                        if (startPosition > 64 && startPosition < 91 && check > 64 && check < 91)
                        {
                            switch (section)
                            {
                                case 1:
                                if (check >= startPosition)
                                {
                                    result_Index = check - startPosition + 1;
                                }

                                break;
                                case 2:
                                //'A'+ 31-('Z'-startPosition+1)
                                int lastIndex = startPosition + 5;
                                if (check <= lastIndex)
                                {
                                    //(check-'A'+1)+('Z'-startPosition+1)
                                    result_Index = 27 - startPosition + check;
                                }

                                break;
                                case 3:
                                //'A'+(31-('Z'-startPosition+26+1))
                                int finalIndex = startPosition - 21;
                                if (check <= finalIndex)
                                {
                                    //('Z' - startPosition+1) + (check - 'A'+1) + 26
                                    result_Index = 183 - check - startPosition;
                                }

                                break;
                            }
                        }

                        return new Tuple<Product, int>(p, result_Index);
                    };
                //List of products with given range codes
                var product = inp_products
                              .Where(
                                  p => p.BalmoContractCode1 == searchFor
                                       || p.BalmoContractCode2 == searchFor
                                       || p.BalmoContractCode3 == searchFor)
                              .Select(pp => checkFitDateCode(pp, searchFor, dateIndex)).FirstOrDefault();
                if (product == null || product.Item2 <= 0 || product.Item2 > 31)
                {
                    if (throwExceptions)
                    {
                        throw new FormatException(string.Format("The alias {0} does not map to a product", code));
                    }

                    return null;
                }

                balmoDateComponent = product.Item2;
                return product.Item1;
            }

            return null;
        }

        /// <summary>
        /// Changed version of ABNMapping.  Futures Code and Exchenge code mapped to product_code and exchange code ofabn_mappings
        /// </summary>
        /// <param name="effectiveDate">Date of ABN record.</param>
        /// <param name="productName">Futures code</param>
        /// <param name="exchangeCode">Exchange code</param>
        /// <param name="throwExceptions"></param>
        /// <returns>mapped Product or null</returns>
        public Product GetABNMappedProductWithValidityCheck(
            DateTime effectiveDate,
            string productCode,
            string exchangeCode,
            bool throwExceptions = true)
        {
            if (_mappings == null)
            {
                _mappings = GetABNMappingsWProduct();
            }

            Product product = _mappings.Where(m => m.ExchangeCode == exchangeCode && m.ProductCode == productCode)
                                       .Select(m => m.Product).FirstOrDefault(
                                           p => (p.ValidFrom == null ? DateTime.MinValue : p.ValidFrom.Value)
                                                <= effectiveDate
                                                && (p.ValidTo == null ? DateTime.MaxValue : p.ValidTo.Value)
                                                >= effectiveDate);

            if (product == null && throwExceptions)
            {
                throw new FormatException(
                    string.Format("The alias {0} does not map to a product", productCode + " " + exchangeCode));
            }

            return product;
        }

        public List<Product> GetProducts()
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                return GetProducts(context);
            }
        }

        /// <summary>
        /// Get products from an already instantiated context
        /// Caller responsible to dispose the context
        /// </summary>
        /// <param name="existingDbContext">Entity context</param>
        /// <returns></returns>
        public List<Product> GetProducts(MandaraEntities existingDbContext)
        {
            List<Product> allProducts = existingDbContext.Products.Include(x => x.OfficialProduct).Include(x => x.Exchange)
                .Include(x => x.Category).Include(x => x.HolidaysCalendar)
                .Include(x => x.ExpiryCalendar).ToList();

            return allProducts;
        }

        public OfficialProduct GetOfficialProduct(string displayName)
        {
            return _officialProducts.GetOfficialProduct(displayName);
        }

        public OfficialProduct GetOfficialProduct(int productId)
        {
            return _officialProducts.GetOfficialProduct(productId);
        }

        public Option<OfficialProduct> GetDefaultOfficialProduct()
        {
            return _officialProducts.GetDefaultOfficialProduct();
        }

        public (int, bool) SaveOfficialProduct(OfficialProduct product, AuditContext auditContext)
        {
            return _officialProducts.SaveOfficialProduct(product, auditContext);
        }

        public bool DeleteOfficialProduct(OfficialProduct product, AuditContext auditContext)
        {
            return _officialProducts.DeleteOfficialProduct(product, auditContext);
        }

        public void RegisterForAddedOfficialProduct(EventHandler<OfficialProductChangeEventArgs> offProdAddedHandler)
        {
            _officialProducts.OfficialProductAdded += offProdAddedHandler;
        }

        public void RegisterForDeletedOfficialProduct(EventHandler<OfficialProductChangeEventArgs> offProdDeletedHandler)
        {
            _officialProducts.OfficialProductDeleted += offProdDeletedHandler;
        }

        public void RegisterForChangedOfficialProduct(
            EventHandler<OfficialProductChangeEventArgs> offProdChangedHandler)
        {
            _officialProducts.OfficialProductChanged += offProdChangedHandler;
        }

        public List<OfficialProduct> GetOfficialProducts()
        {
            return _officialProducts.GetOfficialProducts();
        }

        public Product GetProductByEchangeCode(string code, bool throwExceptions = true)
        {
            if (_products == null)
            {
                _products = GetProducts();
            }

            var product = _products.FirstOrDefault(p => p.ExchangeContractCode == code);
            if (product == null && throwExceptions)
            {
                throw new FormatException(string.Format("The alias {0} does not map to a product", code));
            }

            return product;
        }

        /// <summary>
        /// Return ABNMappings list with product included in data set
        /// </summary>
        /// <returns>ABNMappings list with product</returns>
        private List<ABNMappings> GetABNMappingsWProduct()
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                return cxt.ABNMapping.Include(x => x.Product).Include(x => x.Product.OfficialProduct)
                          .Include(x => x.Product.Exchange).ToList();
            }
        }

        public void RegisterForHolidaysSaved(EventHandler<HolidaysSavedEventArgs> handler)
        {
            _calendars.CalendarHolidaysSaved += handler;
        }

        public void DeregisterForHolidaysSaved(EventHandler<HolidaysSavedEventArgs> handler)
        {
            _calendars.CalendarHolidaysSaved -= handler;
        }

        public void RegisterForExpiriesSaved(EventHandler<ExpiriesSavedEventArgs> handler)
        {
            _calendars.CalendarExpiryDatesSaved += handler;
        }

        public void DeregisterForExpiriesSaved(EventHandler<ExpiriesSavedEventArgs> handler)
        {
            _calendars.CalendarExpiryDatesSaved -= handler;
        }

        /// <summary>
        /// Check calendar may be deleted
        /// </summary>
        /// <param name="id">IdOfCalendar</param>
        /// <returns>True if calendar may be deleted</returns>
        public bool CalendarMayBeRemoved(int id)
        {
            return _calendars.CalendarMayBeRemoved(id);
        }

        public void UpdateExpiryCalendars(
            CalendarChanges<NewExpiryCalendar> calendarChanges,
            int year,
            AuditContext audit)
        {
            _calendars.UpdateExpiryCalendars(calendarChanges, year);
        }

        public void UpdateHolidayCalendars(
            CalendarChanges<NewHolidayCalendar> calendarChanges,
            int year,
            int month,
            AuditContext audit)
        {
            _calendars.UpdateHolidayCalendars(calendarChanges, year, month);
        }

        public List<StockCalendar> GetCalendars()
        {
            return _calendars.GetCalendars();
        }

        public List<StockCalendar> GetHolidaysCalendars()
        {
            return _calendars.GetHolidaysCalendars();
        }

        public List<StockCalendar> GetExpiryCalendars()
        {
            return _calendars.GetExpiryCalendars();
        }

        public int SaveProduct(Product product, AuditContext auditContext)
        {
            product.IsChanged = true;

            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                cxt.Database.CommandTimeout = 60;

                List<int?> allIds = new List<int?> { product.ProductId };
                PropagateIsChangedFlag(cxt, new List<int?> { product.ProductId }, allIds);

                if (product.IceProductsMappings != null)
                {
                    foreach (IceProductMapping mapping in product.IceProductsMappings)
                    {
                        IceProductMapping existingMapping =
                            cxt.ice_product_mappings.Include(x => x.Product)
                               .SingleOrDefault(x => x.IceProductId == mapping.IceProductId);

                        if (existingMapping == null)
                        {
                            continue;
                        }

                        if (existingMapping.IceProductId == mapping.IceProductId
                            && existingMapping.InternalProductId == mapping.InternalProductId)
                        {
                            continue;
                        }

                        throw new IceProductMappingExistException(
                            existingMapping.IceProductId,
                            existingMapping.Product.Name);
                    }
                }

                using (var transaction = cxt.Database.BeginTransaction())
                {
                    if (product.ComplexProduct == null)
                    {
                        ComplexProduct complexProduct =
                            cxt.ComplexProducts.FirstOrDefault(x => x.ProductId == product.ProductId);

                        if (complexProduct != null)
                        {
                            cxt.ComplexProducts.Remove(complexProduct);
                        }
                    }

                    cxt.Configuration.AutoDetectChangesEnabled = false;

                    cxt.Save(cxt.Products, product, p => p.ProductId, out Func<int> newIdFunc)
                       .Save(
                           cxt.ProductAliases,
                           product.Aliases,
                           a => a.AliasId,
                           a => a.ProductId == product.ProductId)
                       .Save(
                           cxt.ice_product_mappings,
                           product.IceProductsMappings,
                           m => m.IceProductId,
                           m => m.InternalProductId == product.ProductId)
                       .Save(
                           cxt.ProductBrokerage,
                           product.CompaniesBrokerages,
                           (a, b) => a.CompanyId == b.CompanyId && a.ProductId == b.ProductId,
                           b => b.ProductId == product.ProductId)
                       .Save(
                           cxt.ABNMapping,
                           product.ABNMappings,
                           m => m.mapping_id,
                           m => m.ProductId == product.ProductId)
                       .Save(
                           cxt.GmiBalmoCodes,
                           product.GmiBalmoCodes,
                           c => c.CodeId,
                           c => c.ProductId == product.ProductId).Save(
                           cxt.IceBalmoMappings,
                           product.IceBalmoMappings,
                           m => m.IceBalmoMappingId,
                           m => m.ProductId == product.ProductId);

                    if (product.ComplexProduct != null)
                    {
                        cxt.Save(
                            cxt.ComplexProducts,
                            product.ComplexProduct,
                            cp => cp.ProductId,
                            out Func<int> newComplexIdFunc);
                    }

                    cxt.ChangeTracker.DetectChanges();
                    cxt.SaveChanges();

                    product.ProductId = newIdFunc();

                    transaction.Commit();
                    return product.ProductId;
                }
            }
        }

        private static void PropagateIsChangedFlag(MandaraEntities cxt, List<int?> productIds, List<int?> allIds)
        {
            List<Product> products = cxt.Products.Where(
                                            x => productIds.Contains(x.ComplexProduct.ChildProduct1_Id)
                                                 || productIds.Contains(x.ComplexProduct.ChildProduct2_Id)
                                                 || productIds.Contains(x.BalmoOnComplexProductId)
                                                 || productIds.Contains(x.BalmoOnCrudeProductId)
                                                 || productIds.Contains(
                                                     x.BalmoOnComplexProduct.ComplexProduct.ChildProduct1_Id)
                                                 || productIds.Contains(
                                                     x.BalmoOnComplexProduct.ComplexProduct.ChildProduct2_Id))
                                        .ToList();

            List<Product> notVisitedProducts = products.Where(x => !allIds.Contains(x.ProductId)).ToList();
            foreach (Product product in notVisitedProducts)
            {
                product.IsChanged = true;
            }

            if (notVisitedProducts.Count > 0)
            {
                List<int?> nextPack = notVisitedProducts.Select(x => (int?)x.ProductId).ToList();
                allIds.AddRange(nextPack);

                PropagateIsChangedFlag(cxt, nextPack, allIds);
            }
        }

        public Product GetProduct(int productId)
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                DbQuery<Product> productQuery = context
                                                .Products.Include("Category").Include("CategoryOverride")
                                                .Include("HolidaysCalendar").Include("HolidaysCalendar.Holidays")
                                                .Include("ExpiryCalendar").Include("ExpiryCalendar.FuturesExpiries")
                                                .Include("OfficialProduct").Include("TasOfficialProduct")
                                                .Include("ComplexProduct").Include("BalmoOnCrudeProduct")
                                                .Include("BalmoOnComplexProduct")
                                                .Include("ComplexProduct.ChildProduct1")
                                                .Include("ComplexProduct.ChildProduct2").Include("Aliases")
                                                .Include("IceProductsMappings").Include("UnderlyingFutures")
                                                .Include("UnderlyingFuturesOverride").Include("CompaniesBrokerages")
                                                .Include("ABNMappings").Include("GmiBalmoCodes")
                                                .Include("IceBalmoMappings").Include("Exchange")
                                                .Include("MonthlyOfficialProduct").Include("Unit").Include("Currency1")
                                                .Include("Currency2").Include("FeeClearingCurrency")
                                                .Include("FeeCommissionCurrency").Include("FeeExchangeCurrency")
                                                .Include("FeeNfaCurrency").Include("FeeCashCurrency")
                                                .Include("FeeBlockCurrency").Include("FeePlattsCurrency");

                return productQuery.Single(p => p.ProductId == productId);
            }
        }

        public List<ProductCategory> GetGroups()
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                return context.ProductCategories.ToList();
            }
        }

        public List<Product> GetProducts_CrudeSwapsAndGasoilSwaps()
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                return context.Products.Where(p => p.ProductTypeDb == (int)ProductType.FuturesBasedSwap).ToList();
            }
        }

        public List<Product> GetProducts_Complex()
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                return context.Products.Where(p => p.ProductTypeDb == (int)ProductType.Diff).ToList();
            }
        }

        public List<Exchange> GetExchanges()
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                return cxt.Exchanges.Include("Calendar").OrderBy(e => e.Name).ToList();
            }
        }

        public int SaveGroup(ProductCategory group, AuditContext auditContext)
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                context.Configuration.AutoDetectChangesEnabled = false;

                context.Save(context.ProductCategories, group, g => g.CategoryId, out Func<int> newIdFunc).Save(
                    context.SwapCrossPerProducts,
                    group.SwapCrossPerProducts,
                    p => p.SwapCrossPerProductId,
                    p => p.CategoryId == group.CategoryId);

                context.ChangeTracker.DetectChanges();
                context.SaveChanges();
                transaction.Commit();

                group.CategoryId = newIdFunc();

                return group.CategoryId;
            }
        }

        public ProductCategory GetGroup(int groupId)
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                return context.ProductCategories.Include("Products").Include("SwapCrossPerProducts")
                              .Include("SwapCrossPerProducts.BalmoSwapCrossProduct")
                              .Single(p => p.CategoryId == groupId);
            }
        }

        public void SaveExchanges(List<Exchange> exchangesToSave, AuditContext auditContext)
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                context.Configuration.AutoDetectChangesEnabled = false;

                context.Save(context.Exchanges, exchangesToSave, e => e.ExchangeId, e => true);

                context.ChangeTracker.DetectChanges();
                context.SaveChanges();
                transaction.Commit();
            }
        }

        public void SaveUnits(List<Unit> units)
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                context.Configuration.AutoDetectChangesEnabled = false;

                context.Save(context.Units, units, e => e.UnitId, e => true);

                context.ChangeTracker.DetectChanges();
                context.SaveChanges();
                transaction.Commit();
            }
        }

        public List<ProductCategory> GetProductGroups()
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                DbQuery<ProductCategory> query = context.ProductCategories
                                                        .Include("Products").Include("Products.ExpiryCalendar")
                                                        .Include("Products.OfficialProduct")
                                                        .Include("Products.UnderlyingFutures").AsNoTracking();
                return query.ToList();
            }
        }

        public List<ParserDefaultProduct> GetDefaultProducts()
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                DbQuery<ParserDefaultProduct> parserDefaultProducts =
                    context.ParserDefaultProducts.Include("OfficialProduct").Include("Broker");

                return parserDefaultProducts.ToList();
            }
        }

        public bool DeleteProduct(Product product, AuditContext auditContext)
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                if (CountReferencesToProduct(product, context) > 0)
                {
                    return false;
                }

                Product existingProduct = context.Products.Include("ComplexProduct")
                                                 .Single(p => p.ProductId == product.ProductId);

                if (existingProduct.ComplexProduct != null)
                {
                    context.ComplexProducts.Remove(existingProduct.ComplexProduct);
                }

                context.SecurityDefinitions.RemoveRange(
                    context.SecurityDefinitions.Where(secDef => secDef.product_id.Value == product.ProductId));
                context.Products.Remove(existingProduct);
                context.SaveChanges();
                return true;
            }
        }

        private static int CountReferencesToProduct(Product product, MandaraEntities context)
        {
            // TODO: add check for messages
            return
                context.SourceDetails.Include(srcDetail => srcDetail.Product)
                       .Count(s => s.Product.ProductId == product.ProductId)
                + context.TradeCaptures.Include(trade => trade.SecurityDefinition)
                         .Include(secDef => secDef.SecurityDefinition.Product)
                         .Count(trade => trade.SecurityDefinition.product_id.Value == product.ProductId)
                + context.ComplexProducts.Include(cmplxProduct => cmplxProduct.ChildProduct1)
                         .Count(p => p.ChildProduct1.ProductId == product.ProductId)
                + context.ComplexProducts.Include(cmplxProduct => cmplxProduct.ChildProduct2)
                         .Count(p => p.ChildProduct2.ProductId == product.ProductId)
                + context.ProductAliases.Include(alias => alias.Product)
                         .Count(a => a.Product.ProductId == product.ProductId);
        }

        public bool DeleteGroup(ProductCategory group, AuditContext auditContext)
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                int count = context.Products.Include("Category").Count(p => p.Category.CategoryId == group.CategoryId);
                if (count > 0)
                {
                    return false;
                }

                ProductCategory existingGroup = context.ProductCategories.Single(p => p.CategoryId == group.CategoryId);

                context.ProductCategories.Remove(existingGroup);

                context.SaveChanges();

                return true;
            }
        }

        public void SaveDefaultProducts(IEnumerable<ParserDefaultProduct> defaultProducts)
        {
            using (MandaraEntities productsDb = CreateMandaraProductsDbContext())
            {
                List<ParserDefaultProduct> parserDefaultProducts =
                    productsDb.ParserDefaultProducts.Include("OfficialProduct").ToList();

                foreach (ParserDefaultProduct updatedProduct in defaultProducts)
                {
                    ParserDefaultProduct existingProduct =
                        parserDefaultProducts.SingleOrDefault(product => product.BrokerId == updatedProduct.BrokerId);

                    if (existingProduct != null)
                    {
                        existingProduct.OfficialProduct = productsDb.OfficialProducts.Single(
                            p => p.OfficialProductId
                                 == updatedProduct
                                    .OfficialProduct.OfficialProductId);
                    }
                    else
                    {
                        updatedProduct.OfficialProduct = productsDb.OfficialProducts.Single(
                            p => p.OfficialProductId
                                 == updatedProduct
                                    .OfficialProduct.OfficialProductId);

                        productsDb.ParserDefaultProducts.Add(updatedProduct);
                    }
                }

                List<ParserDefaultProduct> updatedProducts = defaultProducts.Intersect(parserDefaultProducts).ToList();
                List<ParserDefaultProduct> deletedProducts = parserDefaultProducts.Except(updatedProducts).ToList();

                foreach (ParserDefaultProduct deletedProduct in deletedProducts)
                {
                    productsDb.ParserDefaultProducts.Remove(deletedProduct);
                }

                productsDb.SaveChanges();
            }
        }

        public bool CanDeleteExchange(Exchange exchange)
        {
            if (exchange == null)
            {
                return true;
            }

            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                Exchange loadedExchange = cxt.Exchanges.FirstOrDefault(x => x.ExchangeId == exchange.ExchangeId);

                if (loadedExchange == null)
                {
                    return true;
                }

                List<Product> products = cxt.Products.Where(x => x.Exchange.ExchangeId == loadedExchange.ExchangeId)
                                            .ToList();

                return products.Count == 0;
            }
        }

        public void SaveCalendarExpiryDates(int year, List<StockCalendar> currentExpiryDates, AuditContext auditContext)
        {
            _calendars.SaveCalendarExpiryDates(year, currentExpiryDates);
        }

        public static void SetIsChangedForRelatedProducts(MandaraEntities productsDb, int calendarId)
        {
            List<Product> changedProducts = productsDb.Products.Where(
                                                          product => product.calendar_id == calendarId
                                                                     || product.holidays_calendar_id == calendarId)
                                                      .ToList()
                                                      .Select(
                                                          product =>
                                                          {
                                                              product.IsChanged = true;
                                                              return product;
                                                          })
                                                      .ToList();

            List<int?> changedProductIds = changedProducts.Select(product => (int?)product.ProductId).ToList();

            PropagateIsChangedFlag(productsDb, changedProductIds, changedProductIds);
        }

        public List<ProductAlias> GetProductAliasesWithProducts()
        {
            using (var context = CreateMandaraProductsDbContext())
            {
                return context.ProductAliases.Include("Product").Include("Product.OfficialProduct").ToList();
            }
        }

        public void MapSourceDetailsToSecurityDefinitions(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            worker.ReportProgress(0, "Initializing source details to security definitions mapping ...");

            int lastSourceDetailId = 0;
            List<SourceDetail> sourceDetails;

            ISecurityDefinitionsStorage _securityDefinitionsStorage = IoC.Get<ISecurityDefinitionsStorage>();
            _securityDefinitionsStorage.Update();
            IProductsStorage productsStorage = IoC.Get<IProductsStorage>();
            productsStorage.Update();

            int entitiesNum = 0;
            int entitiesProcessed = 0;

            DateTime sourceDetailsMinDate = SystemTime.Today().AddMonths(-1 * Math.Abs(_precalcSourceDetailsMonthsBack));

            using (var cxt = CreateMandaraProductsDbContext())
            {
                cxt.Database.CommandTimeout = 0;

                entitiesNum += cxt.SourceDetails
                                  .Where(
                                      sd => sd.SourceData.Date > sourceDetailsMinDate
                                            && sd.SecurityDefinitionId == null).AsNoTracking().Count();
            }

            Dictionary<string, SecurityDefinition> sdCache = SecurityDefinitionsManager.BuildSecurityDefinitionsMap(
                _securityDefinitionsStorage.GetSecurityDefinitions(),
                secDef => GetProductFromSecurityDefinition(secDef, productsStorage));

            while (true)
            {
                if (worker.CancellationPending)
                {
                    return;
                }

                using (var cxt = CreateMandaraProductsDbContext())
                {
                    cxt.Database.CommandTimeout = 0;
                    cxt.Configuration.AutoDetectChangesEnabled = false;

                    sourceDetails = cxt.SourceDetails
                                       .Where(
                                           sd => sd.SourceData.Date > sourceDetailsMinDate
                                                 && sd.SourceDetailId > lastSourceDetailId
                                                 && sd.SecurityDefinitionId == null).OrderBy(sd => sd.SourceDetailId)
                                       .Take(1000).ToList();

                    if (sourceDetails.Count == 0)
                    {
                        break;
                    }

                    Dictionary<int, SecurityDefinition> mapping = new Dictionary<int, SecurityDefinition>();

                    foreach (SourceDetail sourceDetail in sourceDetails)
                    {
                        if (sourceDetail.SecurityDefinitionId != null)
                        {
                            continue;
                        }

                        // TODO: What if the product is not in the storage?
                        TryGetResult<Product> getProductResult =
                            productsStorage.TryGetProduct(sourceDetail.product_id);
                        Product product = getProductResult.Value;

                        int productId = product.ProductId;
                        DateTime productDate = sourceDetail.ProductDate;
                        int productDateType = (int)sourceDetail.DateType;

                        string key = productId + "_" + productDateType + "_" + productDate.ToShortDateString();

                        if (!sdCache.TryGetValue(key, out SecurityDefinition securityDefinition))
                        {
                            securityDefinition = new SecurityDefinition
                            {
                                product_id = product.ProductId,
                                StripName =
                                    StripName.Get(
                                        product.Type,
                                        sourceDetail.DateType,
                                        sourceDetail.ProductDate),
                                ProductDescription = product.OfficialProduct.Name,
                                UnderlyingSymbol = GuidExtensions.NumericGuid(GuidExtensions.HalfGuidLength),
                                Exchange = product.Exchange.Name
                            };
                            securityDefinition.UnderlyingSecurityDesc = GetUnderlyingSecurityDesc(
                                securityDefinition.StripName,
                                securityDefinition.ProductDescription);
                            securityDefinition.StartDate = sourceDetail.ProductDate.ToSortableShortDate();
                            securityDefinition.StartDateAsDate = sourceDetail.ProductDate;
                            securityDefinition.UnderlyingMaturityDate =
                                sourceDetail.ProductDate.ToSortableShortDate();
                            securityDefinition.Strip1Date = sourceDetail.ProductDate;
                            securityDefinition.Strip1DateType = sourceDetail.DateType;

                            cxt.SecurityDefinitions.Add(securityDefinition);

                            sdCache.Add(key, securityDefinition);
                        }

                        sourceDetail.SecurityDefinition = securityDefinition;

                        if (sourceDetail.SecurityDefinition.SecurityDefinitionId > 0)
                        {
                            cxt.Entry(sourceDetail.SecurityDefinition).State = EntityState.Unchanged;
                        }

                        mapping.Add(sourceDetail.SourceDetailId, sourceDetail.SecurityDefinition);
                    }

                    cxt.SaveChanges();

                    sourceDetails.ForEach(
                        sd =>
                        {
                            if (mapping.ContainsKey(sd.SourceDetailId))
                            {
                                sd.SecurityDefinitionId = mapping[sd.SourceDetailId].SecurityDefinitionId;
                                cxt.Entry(sd).State = EntityState.Modified;
                            }
                        });

                    cxt.SaveChanges();
                }

                if (sourceDetails.Count > 0)
                {
                    lastSourceDetailId = sourceDetails[sourceDetails.Count - 1].SourceDetailId;
                }

                entitiesProcessed += sourceDetails.Count;

                worker.ReportProgress(
                    entitiesProcessed * 100 / entitiesNum,
                    "Mapping source details to security definitions...");
            }

            worker.ReportProgress(100, "Mapping finished.");
        }

        private Product GetProductFromSecurityDefinition(SecurityDefinition secDef, IProductsStorage productsStorage)
        {
            Product product = GetProductOrDefaultById(secDef.product_id.Value, productsStorage);

            return product;
        }

        private Product GetProductOrDefaultById(int productId, IProductsStorage productsStore)
        {
            TryGetResult<Product> getProductResult = productsStore.TryGetProduct(productId);

            if (!getProductResult.HasValue)
            {
                return null;
            }

            return getProductResult.Value;
        }

        private string GetUnderlyingSecurityDesc(string stripName, string productDescription)
        {
            string underlyingSecurityDesc = productDescription + " - " + stripName;
            int length = underlyingSecurityDesc.Length;

            if (length > 65)
            {
                int prodDescLength = productDescription.Length;
                string truncatedProductDescription = productDescription.Remove(prodDescLength - (length - 65));

                underlyingSecurityDesc = truncatedProductDescription + " - " + stripName;
            }

            return underlyingSecurityDesc;
        }

        public List<Unit> GetUnits()
        {
            using (var cxt = CreateMandaraProductsDbContext())
            {
                return cxt.Units.ToList();
            }
        }

        public bool CanDeleteUnit(Unit unit)
        {
            using (var cxt = CreateMandaraProductsDbContext())
            {
                if (cxt.Products.Any(x => x.UnitId == unit.UnitId))
                {
                    return false;
                }

                if (cxt.CompanyBrokerages.Any(x => x.UnitId == unit.UnitId))
                {
                    return false;
                }

                if (cxt.TradeTemplates.Any(x => x.UnitId == unit.UnitId))
                {
                    return false;
                }
            }

            return true;
        }

        public List<Currency> GetCurrencies()
        {
            using (var cxt = CreateMandaraProductsDbContext())
            {
                return cxt.Currencies.OrderBy(x => x.IsoName).ToList();
            }
        }

        public bool ValidateCurrency(string currency)
        {
            return !string.IsNullOrEmpty(currency) && IsoCurrencies.Contains(currency);
        }

        public void SaveCurrencies(List<Currency> currencies)
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                context.Configuration.AutoDetectChangesEnabled = false;

                context.Save(
                    context.Currencies,
                    currencies,
                    (currency1, currency2) => currency1.CurrencyId == currency2.CurrencyId,
                    currency => true,
                    true,
                    out List<Currency> deleted);

                // check deleted currencies
                List<int> deletedIds = deleted.Select(currency => currency.CurrencyId).ToList();
                OfficialProduct offProduct =
                    context.OfficialProducts.FirstOrDefault(it => deletedIds.Contains(it.CurrencyId));

                if (offProduct != null)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "cannot delete currency because it is used in {0} official product",
                            offProduct.Name));
                }

                context.ChangeTracker.DetectChanges();

                foreach (var currency in context.Currencies.Local)
                {
                    var state = context.Entry(currency).State;
                    if ((state == EntityState.Modified || state == EntityState.Added)
                        && !ValidateCurrency(currency.IsoName))
                    {
                        return;
                    }
                }

                context.SaveChanges();
                transaction.Commit();
            }
        }

        public bool CanDeleteCurrency(Currency currency)
        {
            using (var cxt = CreateMandaraProductsDbContext())
            {
                if (cxt.OfficialProducts.Any(x => x.CurrencyId == currency.CurrencyId))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class IceProductMappingExistException : Exception
    {
        public int IceProductId { get; }
        public string ProductName { get; }

        public IceProductMappingExistException(int iceProductId, string productName)
        {
            IceProductId = iceProductId;
            ProductName = productName;
        }
    }

    public class GmiCodesMapper
    {
        private readonly List<string> _exchangeCodes = new List<string>();
        private readonly Dictionary<string, GmiBalmoIndex> _balmoIndex;

        public GmiCodesMapper()
        {
            List<Product> products = GetBalmoProductsWithGmiBalmoCodes();

            _exchangeCodes = BuildExchangeCodesList(products);

            _balmoIndex = BuildProductsIndexFromGmiBalmoCodes(products);
        }

        private Dictionary<string, GmiBalmoIndex> BuildProductsIndexFromGmiBalmoCodes(List<Product> products)
        {
            Dictionary<string, GmiBalmoIndex> balmoIndex = new Dictionary<string, GmiBalmoIndex>();

            foreach (Product product in products)
            {
                foreach (GmiBalmoCode code in product.GmiBalmoCodes)
                {
                    char endChar = code.EndChar.ToUpperInvariant()[0];
                    char chr = code.StartChar.ToUpperInvariant()[0];
                    int dayCounter = code.PricingDay;
                    string prefix = code.PrefixChar.Trim().ToUpperInvariant();
                    string exchangeCode = code.ExchangeCode.Trim().ToUpperInvariant();

                    do
                    {
                        string indexKey = exchangeCode + "_" + prefix + chr;

                        if (!balmoIndex.ContainsKey(indexKey))
                        {
                            balmoIndex.Add(indexKey, new GmiBalmoIndex { Product = product, Day = dayCounter });
                        }

                        dayCounter++;
                        chr++;
                    }
                    while (chr <= endChar);
                }
            }

            return balmoIndex;
        }

        private List<string> BuildExchangeCodesList(List<Product> products)
        {
            return products.SelectMany(p => p.GmiBalmoCodes).Where(g => g.ExchangeCode != null)
                           .Select(g => g.ExchangeCode.Trim().ToUpperInvariant()).Distinct().ToList();
        }

        private List<Product> GetBalmoProductsWithGmiBalmoCodes()
        {
            List<Product> products = new List<Product>();

            using (MandaraEntities cxt = new MandaraEntities(
                MandaraEntities.DefaultConnStrName,
                nameof(GmiCodesMapper)))
            {
                products = cxt.Products.Include(x => x.GmiBalmoCodes).Include(x => x.OfficialProduct)
                              .Include(x => x.Exchange).Where(
                                  p => p.ProductTypeDb == (int)ProductType.Balmo && p.GmiBalmoCodes.Count > 0)
                              .ToList();
            }

            return products;
        }

        public bool TestExchangeCode(string exchangeCode)
        {
            if (string.IsNullOrWhiteSpace(exchangeCode))
            {
                return false;
            }

            return _exchangeCodes.Contains(exchangeCode.Trim().ToUpperInvariant());
        }

        public bool IsProductType(string gmiCode, ProductType type)
        {
            if (_balmoIndex.TryGetValue(gmiCode, out GmiBalmoIndex index))
            {
                return index.Product.Type == type;
            }

            throw new InvalidOperationException();
        }

        public GmiBalmoIndex MapProduct(string exchangeCode, string futureCode)
        {
            if (string.IsNullOrWhiteSpace(exchangeCode))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(futureCode))
            {
                return null;
            }

            string key = exchangeCode.Trim() + "_" + futureCode.Trim();

            if (_balmoIndex.TryGetValue(key.Trim().ToUpperInvariant(), out GmiBalmoIndex val))
            {
                return val;
            }

            return null;
        }

        public static void ToExchangeFutureCodes(string gmiCode, out string exchangeCode, out string futuresCode)
        {
            if (string.IsNullOrEmpty(gmiCode))
            {
                exchangeCode = null;
                futuresCode = null;
            }
            else
            {
                string[] codes = gmiCode.Split('_');
                if (codes.Length == 2)
                {
                    exchangeCode = codes[0];
                    futuresCode = codes[1];
                }
                else
                {
                    exchangeCode = null;
                    futuresCode = null;
                }
            }
        }

        /// <summary>
        /// Get first entry of GMI code for product by product Id
        /// </summary>
        /// <param name="productId">Id of product to search</param>
        /// <param name="tradeDate"></param>
        /// <returns>string, empty if no code</returns>
        public string GMIBalmoCodeForProduct(int productId, int tradeDate)
        {
            if (_balmoIndex.Values.Any(ind => ind.Product.ProductId == productId))
            {
                try
                {
                    return _balmoIndex.FirstOrDefault(
                                          kv => kv.Value.Product.ProductId == productId && kv.Value.Day == tradeDate)
                                      .Key;
                }
                catch
                {
                }
            }

            return string.Empty;
        }

        public List<Product> SearchProducts(string searchString)
        {
            List<Product> products = _balmoIndex.Where(
                x =>
                {
                    string gmiCode = x.Key.Split('_')[1];
                    return gmiCode.IndexOf(
                               searchString,
                               StringComparison.InvariantCultureIgnoreCase)
                           >= 0;
                }).Select(x => x.Value.Product).Distinct().ToList();

            return products;
        }
    }

    public class GmiBalmoIndex
    {
        public Product Product { get; set; }
        public int Day { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}] at day [{1}]", Product, Day);
        }
    }
}