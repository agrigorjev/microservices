using CsvHelper;
using Mandara.Business;
using Mandara.Business.Calculators;
using Mandara.Business.Contracts;
using Mandara.Business.Managers;
using Mandara.Business.OldCode;
using Mandara.Entities;
using Mandara.Entities.Calculation;
using Mandara.Entities.ErrorReporting;
using Mandara.Entities.Exceptions;
using Mandara.Extensions.Nullable;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Mandara.Date.Time;
using Mandara.Entities.Parser;

namespace Mandara.Import
{
    public class SourceDataImport
    {
        private readonly string[] _referenceHeaders =
        {
            "0",
            "ExchangeId",
            "BuySell",
            "Volume",
            "Market_Expiry",
            "Market_Product",
            "Price",
            "ExchangeTradeNumber"
        };
        private readonly ProductManager _productManager = new ProductManager();
        private readonly ILogger _logger = new NLogLoggerFactory().GetCurrentClassLogger();
        private readonly PrecalcPositionsCalculator _precalcPositionsCalculator;
        private readonly SourceDataManager _storage = new SourceDataManager();

        public SourceDataImport()
        {
            _precalcPositionsCalculator = new PrecalcPositionsCalculator(
                IoC.Get<IProductsStorage>(),
                IoC.Get<PricingPrePositionsManager>());
        }

        public List<Tuple<DataImportException, SourceDetail, bool>> ImportSourceDataFromCsvFile(
            string filename,
            DateTime effectiveDate,
            SourceDataType sourceType)
        {
            SourceData sourceData = _storage.GetSourceData(effectiveDate, sourceType);
            if (sourceData != null)
            {
                sourceData.ImportedDateTime = SystemTime.Now();
                _storage.ClearSourceDetails(sourceData);
            }
            else
            {
                sourceData = new SourceData
                {
                    SourceDataId = -1,
                    Date = effectiveDate,
                    ImportedDateTime = SystemTime.Now(),
                    Type = sourceType
                };
            }

            sourceData.FileName = Path.GetFileName(filename);

            var results = LoadFromCsv(effectiveDate, filename);
            List<SourceDetail> sourceDetails = new List<SourceDetail>();
            foreach (var sourceDetail in results)
            {
                if (sourceDetail.Item2.Product != null && sourceDetail.Item1 == null)
                    sourceDetails.Add(sourceDetail.Item2);
            }

            MapSourceDetailsToSecurityDefinitions(sourceDetails, _precalcPositionsCalculator);

            _storage.SaveSourceData(
                sourceData,
                sourceDetails,
                _precalcPositionsCalculator.ClearerPrecalcs,
                _precalcPositionsCalculator.ClearerSecurityPrecalcs);

            return results;
        }

        public List<Tuple<DataImportException, SourceDetail, bool>> LoadFromCsv(DateTime effectiveDate, string filename)
        {
            List<Tuple<DataImportException, SourceDetail, bool>> lines =
                new List<Tuple<DataImportException, SourceDetail, bool>>();
            CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Dictionary<string, Action<string, SourceRawData>> parseDict =
                    new Dictionary<string, Action<string, SourceRawData>>();

                foreach (var item in GetParseDictionary())
                {
                    parseDict[item.Item1] = item.Item2;
                }

                using (var stream = new StreamReader(filename))
                using (var csv = new CsvReader(stream, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    string[] headers = csv.ReadHeader()
                        ? csv.HeaderRecord
                        : new string[0];
                    
                    int productRowNumber = 1;

                    while (csv.Read())
                    {
                        lines.Add(GetCsvRow(effectiveDate, headers, csv, parseDict, productRowNumber));
                        productRowNumber++;
                    }
                }
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = originalCulture;
            }

            return lines;
        }

        private Tuple<DataImportException, SourceDetail, bool> GetCsvRow(
            DateTime effectiveDate,
            string[] headers,
            CsvReader reader,
            Dictionary<string, Action<string, SourceRawData>> parseDict,
            int productRowNumber)
        {
            bool isWarning = false;
            SourceRawData rawData = new SourceRawData();

            foreach (var header in headers)
            {
                if (parseDict.ContainsKey(header))
                {
                    parseDict[header](reader[header], rawData);
                }
            }

            SourceDetail sourceDetail = new SourceDetail();
            DataImportException e = null;

            try
            {
                sourceDetail.InstrumentDescription = rawData.RawInstrumentDescription;
                sourceDetail.Quantity = rawData.Quantity;
                sourceDetail.TradePrice = rawData.TradePrice;
                sourceDetail.RawDate = rawData.ProcessDate;
                sourceDetail.AccountNumber = rawData.AccountNumber;
                sourceDetail.ExchangeCode = rawData.ExchangeCode;
                sourceDetail.ExpiryDate = rawData.ExpiryDate;
                sourceDetail.FeeClearing = rawData.ClearingFee;
                sourceDetail.FeeCommission = rawData.ComissionFee;
                sourceDetail.FeeExchange = rawData.ExchangeFee;
                sourceDetail.FeeNfa = rawData.NfaFee;
                sourceDetail.FutureCode = rawData.FutureCode;
                sourceDetail.TradeDate = rawData.RawTradeDate;
                sourceDetail.TradeType = rawData.RawTradeType;

                var splitDescription = InstrumentParser.SplitDescription(rawData.RawInstrumentDescription);
                sourceDetail.ProductDate = splitDescription.Item2;
                sourceDetail.DateType = splitDescription.Item3;

                if (_productManager != null)
                {
                    Product product = null;

                    // try to map using ABN mapping details
                    product = _productManager.GetABNMappedProductWithValidityCheck(
                        rawData.TradeDate,
                        sourceDetail.FutureCode,
                        sourceDetail.Exchange_Code,
                        false);

                    // if it's an ICE product - try mapping using gmi balmo codes
                    if (product == null && _productManager.GmiCodesMapper.TestExchangeCode(sourceDetail.Exchange_Code))
                    {
                        GmiBalmoIndex index = _productManager.GmiCodesMapper.MapProduct(
                            sourceDetail.Exchange_Code,
                            sourceDetail.FutureCode);

                        if (index != null)
                        {
                            product = index.Product;
                            sourceDetail.ProductDate = new DateTime(sourceDetail.ProductDate.Year,
                                                                    sourceDetail.ProductDate.Month, index.Day);
                            sourceDetail.DateType = ProductDateType.Day;
                        }
                    }

                    // if we couldn't map product with gmi codes map it the old way
                    if (product == null)
                    {
                        product = _productManager.GetProductWithValidityCheck(rawData.TradeDate, splitDescription.Item1);

                        isWarning = true;
                        e = new DataImportException(
                            string.Format("GMI Code [{0}, {1}] was not found. Successfully mapped using old algorithm.",
                                            sourceDetail.ExchangeCode, sourceDetail.FutureCode), productRowNumber);
                    }

                    if (product != null)
                    {
                        sourceDetail.Product = product;
                    }
                }
            }
            catch (FormatException ex)
            {
                _logger.Error(ex, "SourceDataImport.GetCsvRow - exception reading row.");
                e = new DataImportException(ex.Message, productRowNumber);
            }

            return new Tuple<DataImportException, SourceDetail, bool>(e, sourceDetail, isWarning);
        }

        private IEnumerable<Tuple<string, Action<string, SourceRawData>>> GetParseDictionary()
        {
            foreach (var propertyLoop in typeof(SourceRawData).GetProperties())
            {
                var property = propertyLoop;
                var attributeInfo = property.GetCustomAttributes(true);
                var attribute = attributeInfo.SingleOrDefault(a => typeof(RawDataAttribute)
                                                    .IsAssignableFrom(a.GetType()));

                if (attribute != null)
                {
                    var rawDataAttribute = (RawDataAttribute)attribute;
                    yield return
                        new Tuple<string, Action<string, SourceRawData>>(
                            rawDataAttribute.FieldName,
                            (v, obj) => property.SetValue(obj, v, null));
                }
            }
        }

        /// <summary>
        /// Save received data to list of Source detail.
        /// </summary>
        /// <param name="effectiveDate">DateTime</param>
        /// <param name="values">Csv data values</param>
        /// <returns></returns>
        public Dictionary<int, Tuple<DataImportException, SourceDetail, bool>> LoadFromArray(
            DateTime effectiveDate,
            List<string[]> values)
        {
            Dictionary<int, Tuple<DataImportException, SourceDetail, bool>> lines =
                new Dictionary<int, Tuple<DataImportException, SourceDetail, bool>>();

            CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            try
            {
                values = values.OrderBy(v => int.Parse(v[0])).ToList();
                var parseDict = new Dictionary<string, Action<string, SourceRawData>>();

                foreach (var item in GetParseDictionary())
                {
                    parseDict[item.Item1] = item.Item2;
                }

                string[] headers = values[0];

                for (int i = 1; i < values.Count; i++)
                {
                    lines.Add(i, GetCsvRowData(effectiveDate, headers, values[i], parseDict, i));
                }
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = originalCulture;
            }

            return lines;
        }

        /// <summary>
        /// Performs convert of csv raw data to SourceDetail with exception if any
        /// </summary>
        /// <param name="effectiveDate"></param>
        /// <param name="headers"></param>
        /// <param name="values"></param>
        /// <param name="parseDict"></param>
        /// <param name="productRowNumber"></param>
        /// <returns></returns>
        private Tuple<DataImportException, SourceDetail, bool> GetCsvRowData(
            DateTime effectiveDate,
            string[] headers,
            string[] values,
            Dictionary<string, Action<string, SourceRawData>> parseDict,
            int productRowNumber)
        {
            bool isWarning = false;
            SourceRawData rawData = new SourceRawData();

            for (int i = 0; i < headers.Length; i++)
            {
                if (parseDict.ContainsKey(headers[i]))
                {
                    parseDict[headers[i]](values[i], rawData);
                }
            }

            SourceDetail sourceDetail = new SourceDetail();
            DataImportException e = null;

            try
            {
                sourceDetail.InstrumentDescription = rawData.RawInstrumentDescription;
                sourceDetail.Quantity = rawData.Quantity;
                sourceDetail.TradePrice = rawData.TradePrice;
                sourceDetail.RawDate = rawData.ProcessDate;
                sourceDetail.AccountNumber = rawData.AccountNumber;
                sourceDetail.ExchangeCode = rawData.ExchangeCode;
                sourceDetail.ExpiryDate = rawData.ExpiryDate;
                sourceDetail.FeeClearing = rawData.ClearingFee;
                sourceDetail.FeeCommission = rawData.ComissionFee;
                sourceDetail.FeeExchange = rawData.ExchangeFee;
                sourceDetail.FeeNfa = rawData.NfaFee;
                sourceDetail.FutureCode = rawData.FutureCode;
                sourceDetail.TradeDate = rawData.RawTradeDate;
                sourceDetail.TradeType = rawData.RawTradeType;

                Tuple<string, DateTime, ProductDateType> splitDescription =
                    InstrumentParser.SplitDescription(rawData.RawInstrumentDescription);

                sourceDetail.ProductDate = splitDescription.Item2;
                sourceDetail.DateType = splitDescription.Item3;

                if (_productManager != null)
                {
                    Product product = null;

                    // try to map using ABN mapping details
                    product = _productManager.GetABNMappedProductWithValidityCheck(
                        rawData.TradeDate,
                        sourceDetail.FutureCode,
                        sourceDetail.Exchange_Code,
                        false);

                    // if it's an ICE product - try mapping using gmi balmo codes
                    if (product == null && _productManager.GmiCodesMapper.TestExchangeCode(sourceDetail.Exchange_Code))
                    {
                        GmiBalmoIndex index = _productManager.GmiCodesMapper.MapProduct(
                            sourceDetail.Exchange_Code,
                            sourceDetail.FutureCode);

                        if (index != null)
                        {
                            product = index.Product;

                            sourceDetail.ProductDate = new DateTime(sourceDetail.ProductDate.Year,
                                                                    sourceDetail.ProductDate.Month, index.Day);
                            sourceDetail.DateType = ProductDateType.Day;
                        }
                    }

                    // if we couldn't map product with gmi codes map it the old way
                    if (product == null)
                    {
                        product = _productManager.GetProductWithValidityCheck(rawData.TradeDate, splitDescription.Item1);

                        isWarning = true;
                        e = new DataImportException(
                            string.Format("GMI Code [{0}, {1}] was not found. Successfully mapped using old algorithm.",
                                            sourceDetail.ExchangeCode, sourceDetail.FutureCode), productRowNumber);
                    }

                    if (product != null)
                    {
                        sourceDetail.Product = product;
                        sourceDetail.TradePrice = rawData.TradePrice
                                                  * GetContractSizeModifier(rawData.Multiplier, product);

                        if (product.IsProductDaily)
                        {
                            sourceDetail.DateType = sourceDetail.DateType1 = ProductDateType.Daily;
                            sourceDetail.Quantity = sourceDetail.Quantity * rawData.Multiplier / product.ContractSize;
                        }
                    }
                }

            }
            catch (FormatException ex)
            {
                _logger.Error(ex, "SourceDataImport.GetCsvRowData - exception reading row.");
                e = new DataImportException(ex.Message, productRowNumber);
            }

            return new Tuple<DataImportException, SourceDetail, bool>(e, sourceDetail, isWarning);
        }

        private decimal GetContractSizeModifier(decimal multiplier, Product product)
        {
            // multiplier from a product in our db
            decimal productVal = (product.PnlFactor ?? 1) * product.ContractSize;

            // guard checks
            if (multiplier == 0M)
            {
                multiplier = 1M;
            }

            if (productVal == 0M)
            {
                productVal = 1M;
            }

            // get number of exponents in multiplier from a file
            int exponentsAbn = (int)Math.Floor(Math.Log10(Convert.ToDouble(multiplier)));
            // get number of exponents in multiplier from a product in our db
            int exponentsProduct = (int)Math.Floor(Math.Log10(Convert.ToDouble(productVal)));
            int diffInExponents = exponentsAbn - exponentsProduct;
            double coefficient = Math.Pow(10, diffInExponents);

            return Convert.ToDecimal(coefficient);
        }

        /// <summary>
        /// Save data to database.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public List<Tuple<DataImportException, SourceDetail>> ImportSourceDataFromArray(
            List<string[]> values,
            DateTime effectiveDate,
            SourceDataType sourceType,
            out List<Error> expired)
        {
            SourceData sourceData = _storage.GetSourceData(effectiveDate, sourceType);

            if (sourceData != null)
            {
                sourceData.ImportedDateTime = SystemTime.Now();

                _storage.ClearSourceDetails(sourceData);
            }
            else
            {
                sourceData = new SourceData
                {
                    SourceDataId = -1,
                    Date = effectiveDate,
                    ImportedDateTime = SystemTime.Now(),
                    Type = sourceType
                };
            }

            Dictionary<int, Tuple<DataImportException, SourceDetail, bool>> results = LoadFromArray(
                effectiveDate,
                values);
            List<SourceDetail> sourceDetails = new List<SourceDetail>();

            foreach (var sourceDetail in results)
            {
                if (sourceDetail.Value.Item2.Product != null
                    && (sourceDetail.Value.Item1 == null || sourceDetail.Value.Item3))
                {
                    sourceDetails.Add(sourceDetail.Value.Item2);
                }
            }

            MapSourceDetailsToSecurityDefinitions(sourceDetails, _precalcPositionsCalculator);

            _storage.SaveSourceData(
                sourceData,
                sourceDetails,
                _precalcPositionsCalculator.ClearerPrecalcs,
                _precalcPositionsCalculator.ClearerSecurityPrecalcs);
            CalculationManager mgr = new CalculationManager();
            List<CalculationError> calculationErrors = new List<CalculationError>();
            expired = new List<Error>();

            using (var context = CreateMandaraProductsDbContext())
            {
                CalculationCache cache = new CalculationCache().Initialize(context);

                foreach (var sourceDetail in results)
                {
                    if (sourceDetail.Value.Item2.Product != null
                        && (sourceDetail.Value.Item1 == null || sourceDetail.Value.Item3))
                    {
                        if (mgr.CalculatePositions(
                                   cache,
                                   context,
                                   new List<SourceDetail>() { sourceDetail.Value.Item2 },
                                   effectiveDate,
                                   sourceType,
                                   out calculationErrors)
                               .Any())
                        {
                            expired.Add(
                                new Error(
                                    "CSV Import",
                                    ErrorType.ImportError,
                                    $"Expired product in csv row #{sourceDetail.Key + 1}",
                                    sourceDetail.Value.Item2.InstrumentDescription));
                        }
                    }

                }
            }

            return results.Select(r => new Tuple<DataImportException, SourceDetail>(r.Value.Item1, r.Value.Item2)).ToList();
        }

        private static MandaraEntities CreateMandaraProductsDbContext()
        {
            return new MandaraEntities(MandaraEntities.DefaultConnStrName, nameof(SourceDataImport));
        }

        public void MapSourceDetailsToSecurityDefinitions(
            List<SourceDetail> sourceDetails,
            Func<Product, SourceDetail, Action<SourceDetail, SecurityDefinition>, SecurityDefinition> buildSecurityDef,
            Action<SourceDetail, SecurityDefinition> buildPrecalcs)
        {
            if (!(sourceDetails?.Any()).True())
            {
                return;
            }

            using (MandaraEntities productsDb = CreateMandaraProductsDbContext())
            {
                productsDb.Database.CommandTimeout = 0;

                List<SecurityDefinition> allSecurityDefinitions = ReadSecurityDefinitions(productsDb);
                Dictionary<string, SecurityDefinition> sdCache =
                    SecurityDefinitionsManager.BuildSecurityDefinitionsMap(
                        allSecurityDefinitions,
                        (secDef) => secDef.Product);

                foreach (SourceDetail trade in sourceDetails.Where(abnTrade => null != abnTrade.Product).ToList())
                {
                    Product product = trade.Product;
                    var security = GetSecurityDefinition(
                        trade,
                        product,
                        sdCache,
                        buildSecurityDef,
                        buildPrecalcs,
                        productsDb);

                    trade.SecurityDefinition = security.secDef;

                    if (!security.precalcsBuilt && IsBalmoOrDailyProduct(product))
                    {
                        buildPrecalcs(trade, security.secDef);
                    }
                }
            }

            bool IsBalmoOrDailyProduct(Product product)
            {
                return product.Type == ProductType.Balmo || product.IsProductDaily;
            }
        }

        private (SecurityDefinition secDef, bool precalcsBuilt) GetSecurityDefinition(
            SourceDetail trade,
            Product product,
            Dictionary<string, SecurityDefinition> sdCache,
            Func<Product, SourceDetail, Action<SourceDetail, SecurityDefinition>, SecurityDefinition> buildSecurityDef,
            Action<SourceDetail, SecurityDefinition> buildPrecalcs,
            MandaraEntities productsDb)
        {
            bool precalcDetailsBuilt = false;
            int productId = product.ProductId;
            DateTime productDate = trade.ProductDate;
            int productDateType = (int)trade.DateType;
            string secDefKey = productId + "_" +
                                 productDateType + "_" +
                                 productDate.ToShortDateString();

            if (!sdCache.TryGetValue(secDefKey, out SecurityDefinition securityDefinition))
            {
                securityDefinition = buildSecurityDef(product, trade, buildPrecalcs);
                precalcDetailsBuilt = securityDefinition.HasPrecalcDetails();
                productsDb.SecurityDefinitions.Add(securityDefinition);
                sdCache.Add(secDefKey, securityDefinition);
            }

            return (securityDefinition, precalcDetailsBuilt);
        }

        public void MapSourceDetailsToSecurityDefinitions(
            List<SourceDetail> sourceDetails,
            PrecalcPositionsCalculator precalcPositionsCalculator = null)
        {
            MapSourceDetailsToSecurityDefinitions(sourceDetails, CreateNewSecurityDefinition, GetPrecalcsCreate());

            Action<SourceDetail, SecurityDefinition> GetPrecalcsCreate()
            {
                return null != precalcPositionsCalculator
                    ? precalcPositionsCalculator.CalculatePrecalcPositions
                    : (Action<SourceDetail, SecurityDefinition>)((trade, secDef) => { });
            }
        }

        private SecurityDefinition CreateNewSecurityDefinition(
            Product product,
            SourceDetail trade,
            Action<SourceDetail, SecurityDefinition> addPrecalcs)
        {
            SecurityDefinition securityDefinition = new SecurityDefinition();

            securityDefinition.product_id = product.ProductId;
            securityDefinition.StripName = StripName.Get(product.Type, trade.DateType, trade.ProductDate);
            securityDefinition.ProductDescription = product.OfficialProduct.Name;
            securityDefinition.UnderlyingSymbol =
                Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "").Substring(0, 16);
            securityDefinition.Exchange = product.Exchange.Name;
            securityDefinition.UnderlyingSecurityDesc = GetUnderlyingSecurityDesc(
                securityDefinition.StripName,
                securityDefinition.ProductDescription);
            securityDefinition.StartDate = trade.ProductDate.ToShortDateString();
            securityDefinition.StartDateAsDate = trade.ProductDate;
            securityDefinition.UnderlyingMaturityDate = trade.ProductDate.ToShortDateString();
            securityDefinition.Strip1Date = trade.ProductDate;
            securityDefinition.Strip1DateType = trade.DateType;

            addPrecalcs(trade, securityDefinition);
            return securityDefinition;
        }

        public void MapSourceDetailsToSecurityDefinitions1(
            List<SourceDetail> sourceDetails,
            PrecalcPositionsCalculator precalcPositionsCalculator = null)
        {
            if (sourceDetails == null)
                return;

            using (var cxt = CreateMandaraProductsDbContext())
            {
                cxt.Database.CommandTimeout = 0;

                List<SecurityDefinition> allSecurityDefinitions = ReadSecurityDefinitions(cxt);

                Dictionary<string, SecurityDefinition> sdCache =
                    SecurityDefinitionsManager.BuildSecurityDefinitionsMap(
                        allSecurityDefinitions,
                        (secDef) => secDef.Product);

                foreach (SourceDetail sourceDetail in sourceDetails.ToList())
                {
                    Product product = sourceDetail.Product;

                    if (product == null)
                    {
                        continue;
                    }

                    bool precalcDetailsBuilt = false;
                    int productId = product.ProductId;
                    DateTime productDate = sourceDetail.ProductDate;
                    int productDateType = (int)sourceDetail.DateType;

                    string key = productId + "_" +
                                         productDateType + "_" +
                                         productDate.ToShortDateString();

                    SecurityDefinition securityDefinition;
                    if (!sdCache.TryGetValue(key, out securityDefinition))
                    {
                        securityDefinition = new SecurityDefinition();
                        securityDefinition.product_id = product.ProductId;
                        securityDefinition.StripName = StripName.Get(
                            product.Type,
                            sourceDetail.DateType,
                            sourceDetail.ProductDate);
                        securityDefinition.ProductDescription = product.OfficialProduct.Name;
                        securityDefinition.UnderlyingSymbol =
                            Guid.NewGuid()
                                .ToString()
                                .Replace("{", "")
                                .Replace("}", "")
                                .Replace("-", "")
                                .Substring(0, 16);
                        securityDefinition.Exchange = product.Exchange.Name;
                        securityDefinition.UnderlyingSecurityDesc =
                            GetUnderlyingSecurityDesc(
                                securityDefinition.StripName,
                                securityDefinition.ProductDescription);
                        securityDefinition.StartDate = sourceDetail.ProductDate.ToShortDateString();
                        securityDefinition.StartDateAsDate = sourceDetail.ProductDate;
                        securityDefinition.UnderlyingMaturityDate = sourceDetail.ProductDate.ToShortDateString();
                        securityDefinition.Strip1Date = sourceDetail.ProductDate;
                        securityDefinition.Strip1DateType = sourceDetail.DateType;

                        cxt.SecurityDefinitions.Add(securityDefinition);

                        sdCache.Add(key, securityDefinition);

                        if (precalcPositionsCalculator != null)
                        {
                            precalcDetailsBuilt = true;
                            precalcPositionsCalculator.CalculatePrecalcPositions(sourceDetail, securityDefinition);
                        }
                    }

                    sourceDetail.SecurityDefinition = securityDefinition;

                    bool isBalmoOrDaily = product.Type == ProductType.Balmo
                                            || product.IsProductDaily;

                    if (!precalcDetailsBuilt && precalcPositionsCalculator != null && isBalmoOrDaily)
                    {
                        precalcPositionsCalculator.CalculatePrecalcPositions(sourceDetail, securityDefinition);
                    }

                }
            }
        }

        private List<SecurityDefinition> ReadSecurityDefinitions(MandaraEntities cxt)
        {
            List<SecurityDefinition> allSecurityDefinitions = new List<SecurityDefinition>();
            List<SecurityDefinition> packOfSecurityDefinitions;
            int maxSdId = 0;
            int takeNum = 10000;

            do
            {
                int id = maxSdId;

                packOfSecurityDefinitions = cxt.SecurityDefinitions
                    .Include(x => x.Product)
                    .AsNoTracking()
                    .Where(tc => tc.SecurityDefinitionId > id)
                    .Take(takeNum)
                    .OrderBy(tc => tc.SecurityDefinitionId)
                    .ToList();

                if (packOfSecurityDefinitions.Count > 0)
                {
                    allSecurityDefinitions.AddRange(packOfSecurityDefinitions);
                    maxSdId = packOfSecurityDefinitions[packOfSecurityDefinitions.Count - 1].SecurityDefinitionId;
                }
            } while (packOfSecurityDefinitions.Count == takeNum);

            return allSecurityDefinitions;
        }

        private static string GetUnderlyingSecurityDesc(string stripName, string productDescription)
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

        /// <summary>
        /// Performs map from values string array to raw seals class properties
        /// </summary>
        /// <returns>Collection of properies with map action</returns>
        private IEnumerable<Tuple<string, Action<string, SealRawData>>> GetSealParseDictionary()
        {
            foreach (var propertyLoop in typeof(SealRawData).GetProperties())
            {
                var property = propertyLoop;
                //Check for custom attributes mark 
                var attributeInfo = property.GetCustomAttributes(true);
                var attribute = attributeInfo.SingleOrDefault(a => typeof(RawDataAttribute)
                                                    .IsAssignableFrom(a.GetType()));

                if (attribute != null)
                {
                    var rawDataAttribute = (RawDataAttribute)attribute;
                    yield return new Tuple<string, Action<string, SealRawData>>(
                        rawDataAttribute.FieldName,
                        (v, obj) => property.SetValue(obj, v, null));
                }
            }
        }

        /// <summary>
        /// Transform collection of csv records to SealDetail objects
        /// </summary>
        /// <param name="effectiveDate">Date of data actual for</param>
        /// <param name="values">Array of actual csv records</param>
        /// <returns>Collection of sealDetails objects with import errors.Key is row number</returns>
        public Dictionary<int, Tuple<List<DataImportException>, SealDetail>> LoadSealFromArray(
            DateTime effectiveDate,
            List<string[]> values)
        {
            Dictionary<int, Tuple<List<DataImportException>, SealDetail>> lines =
                new Dictionary<int, Tuple<List<DataImportException>, SealDetail>>();

            //Setup neutral culture for data tpe conversion purpose
            CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            try
            {
                //Sort collection to restore row numbering of original file
                values = values.OrderBy(v => int.Parse(v[0])).ToList();
                //Prepare "mapping rules" collection
                var parseDict = new Dictionary<string, Action<string, SealRawData>>();
                foreach (var item in GetSealParseDictionary())
                {
                    parseDict[item.Item1] = item.Item2;
                }
                //Get headers
                string[] headers = values[0];
                //Process every single record

                for (int i = 1; i < values.Count; i++)
                {
                    lines.Add(i, GetCsvRowData(effectiveDate, headers, values[i], parseDict, i));
                }
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = originalCulture;
            }

            return lines;
        }

        /// <summary>
        /// Process single csv record to Seal details and exceptions
        /// </summary>
        /// <param name="effectiveDate">Date of data actual for</param>
        /// <param name="headers">Filed headers from csv file</param>
        /// <param name="values">Array of actual csv values of single row</param>
        /// <param name="parseDict">Collection of "mapping" rules</param>
        /// <param name="productRowNumber">Row number to remember if error</param>
        /// <returns>Seal Detail class along with collection of errors for current csv row</returns>
        private Tuple<List<DataImportException>, SealDetail> GetCsvRowData(
            DateTime effectiveDate,
            string[] headers,
            string[] values,
            Dictionary<string, Action<string, SealRawData>> parseDict,
            int productRowNumber)
        {
            //Process string csv values to Raw Seal object
            var rawData = new SealRawData();
            for (int i = 0; i < headers.Length; i++)
            {
                if (parseDict.ContainsKey(headers[i]))
                {
                    parseDict[headers[i]](values[i], rawData);
                }
            }
            var sealDetail = new SealDetail();
            List<DataImportException> e = new List<DataImportException>();
            ;
            //Get SealDetails object from raw Seal clas
            try
            {
                //Check data types validity
                List<DataImportException> fe = rawData.CheckRawData();

                if (fe.Count > 0)
                {
                    fe.ForEach(ee => e.Add(new DataImportException(ee.Message, productRowNumber, ee.ErrorLevel)));
                }

                //If balmo set to Day datetype
                if (rawData.Expiry.HasValue)
                {
                    if (rawData.Expiry.Value.Month == effectiveDate.Month &&
                        rawData.Expiry.Value.Year == effectiveDate.Year)
                    {
                        sealDetail.DateType = (int)ProductDateType.Day;
                    }
                    else
                    {
                        sealDetail.DateType = (int)ProductDateType.MonthYear;
                    }
                }

                sealDetail.BusinessStateName = rawData.Raw_BusinessStateName;
                sealDetail.TradeId = rawData.Raw_TradeId;
                sealDetail.ExchangeId = rawData.Raw_ExchangeId;
                sealDetail.Volume = rawData.Raw_Volume;
                sealDetail.Market_Expiry = rawData.Raw_Market_Expiry;
                sealDetail.Market_Product = rawData.Raw_Market_Product;
                sealDetail.BuySell = rawData.Raw_BuySell;
                sealDetail.OptionType = rawData.Raw_OptionType;
                sealDetail.Strike = rawData.Raw_Strike;
                sealDetail.Price = rawData.Raw_Price;
                sealDetail.Expr1 = rawData.Raw_Expr1;
                sealDetail.Reference1 = rawData.Raw_Reference1;
                sealDetail.ToMemberId = rawData.Raw_ToMemberId;
                sealDetail.ToMember = rawData.Raw_ToMember;
                sealDetail.ExchangeOrderNumber = rawData.Raw_ExchangeOrderNumber;
                sealDetail.ExchangeTradeNumber = rawData.Raw_ExchangeTradeNumber;
                sealDetail.Reference2 = rawData.Raw_Reference2;
                sealDetail.Reference3 = rawData.Raw_Reference3;
                sealDetail.GiveupRef1 = rawData.Raw_GiveupRef1;
                sealDetail.GiveupRef2 = rawData.Raw_GiveupRef2;
                sealDetail.GiveupRef3 = rawData.Raw_GiveupRef3;
                sealDetail.Trader = rawData.Raw_Trader;
                sealDetail.AllocationId = rawData.Raw_AllocationId;
                sealDetail.ExchangeMember = rawData.Raw_ExchangeMember;
                sealDetail.TradingDay = rawData.Raw_TradingDay;
                sealDetail.ClearingDay = rawData.Raw_ClearingDay;
                sealDetail.CounterPartyMember = rawData.Raw_CounterPartyMember;
                sealDetail.OpenClose = rawData.Raw_OpenClose;
                sealDetail.PositionAccount = rawData.Raw_PositionAccount;
                sealDetail.BOAccount = rawData.Raw_BOAccount;
                sealDetail.BOReference = rawData.Raw_BOReference;
                sealDetail.OriginTransferType = rawData.Raw_OriginTransferType;
                sealDetail.OriginFromMember = rawData.Raw_OriginFromMember;
                sealDetail.TradingTradeType = rawData.Raw_TradingTradeType;
                sealDetail.TradePriceType = rawData.Raw_TradePriceType;
                sealDetail.Venue = rawData.Raw_Venue;
                sealDetail.ComboType = rawData.Raw_ComboType;
                sealDetail.OriginalClearingDay = rawData.Raw_OriginalClearingDay;
                sealDetail.Note = rawData.Raw_Note;
                sealDetail.TradeGroupId = rawData.Raw_TradeGroupId;
                sealDetail.ClientId = rawData.Raw_ClientId;
                sealDetail.ClientName = rawData.Raw_ClientName;

                //Check if product exists in DB
                if (_productManager != null)
                {
                    if (sealDetail.IsICE && sealDetail.DateType == (int)ProductDateType.Day)
                    {
                        int datePart = 0;
                        var product = _productManager.GetProductByBalmoSealCode(
                            sealDetail.Market_Product,
                            out datePart,
                            false);

                        if (product != null)
                        {
                            //if product found check whether we may got valid datetime for balmo
                            try
                            {
                                new DateTime(effectiveDate.Year, effectiveDate.Month, datePart);
                            }
                            catch
                            {
                                //Set  product to null if failed to build valid datetime
                                product = null;
                                //throw new FormatException(
                                //    String.Format(
                                //        "The alias {0} does not map to a product",
                                //        sealDetail.Market_Product));
                            }
                        }
                        if (product == null)
                        {
                            //if we failed to map product by balmo code we try to map by exchange code
                            product = _productManager.GetProductByEchangeCode(sealDetail.Market_Product);
                            //if success we consider detail as regular not balmo
                            sealDetail.DateType = (int)ProductDateType.MonthYear;
                        }
                    }
                    else
                    {
                        var product = _productManager.GetProductByEchangeCode(sealDetail.Market_Product);
                    }
                }

            }
            catch (FormatException ex)
            {
                _logger.Error(ex, "SourceDataImport.GetCsvRowData for seals - exception reading row.");
                e.Add(new DataImportException(ex.Message, productRowNumber));
            }

            return new Tuple<List<DataImportException>, SealDetail>(e, sealDetail);
        }

        /// <summary>
        /// Save list of string arrays of csv data to Seals table in db
        /// </summary>
        /// <param name="values">List of array representing single csv reacord</param>
        /// <param name="effectiveDate">Date of data actual for</param>
        /// <param name="sourceType">Type of source data. Allways Seals</param>
        /// <returns>Collection of Seals objects along with list of import errors for each csv record</returns>
        public List<Tuple<List<DataImportException>, SealDetail>> ImportSealSourceDataFromArray(
            List<string[]> values,
            DateTime effectiveDate,
            SourceDataType sourceType)
        {
            //Check if there is source data entry for date and source type provided
            SourceData sourceData = _storage.GetSourceData(effectiveDate, sourceType);
            if (sourceData != null)
            {
                //If there is source data then update import date
                sourceData.ImportedDateTime = SystemTime.Now();
            }
            else
            {
                //If not create new one.
                sourceData = new SourceData
                {
                    SourceDataId = -1,
                    Date = effectiveDate,
                    ImportedDateTime = SystemTime.Now(),
                    Type = sourceType
                };
            }

            //Transform csv value to Seals object
            Dictionary<int, Tuple<List<DataImportException>, SealDetail>> results = LoadSealFromArray(
                effectiveDate,
                values);
            //Retrieve SealDetails
            List<SealDetail> sourceDetails = results.Values.Select(v => v.Item2).ToList();
            //Save Seal details to DB
            _storage.SaveSealData(sourceData, sourceDetails);
            //return object and errors for further processing
            return results.Select(r => new Tuple<List<DataImportException>, SealDetail>(r.Value.Item1, r.Value.Item2))
                          .ToList();
        }

        /// <summary>
        /// Import Seals data from mail service
        /// </summary>
        /// <param name="filename">Filename of csv file.</param>
        /// <param name="effectiveDate">Date of data import to (taken from filename)</param>
        /// <returns></returns>
        public List<Tuple<List<DataImportException>, SealDetail>> ImportSealsDetailsDataFromCsvFile(
            string filename,
            DateTime effectiveDate)
        {
            List<string[]> lines = new List<string[]>();
            using (CsvReader csv = new CsvReader(new System.IO.StreamReader(filename), CultureInfo.InvariantCulture))
            {
                //Get headers
                string[] headers = csv.ReadHeader()
                        ? csv.HeaderRecord
                        : new string[0];
                
                int n = 1;
                //Prepare dump array with row number marker
                string[] nHeaders = new string[headers.Length + 1];

                nHeaders[0] = "0";
                Array.Copy(headers, 0, nHeaders, 1, headers.Length);
                lines.Add(nHeaders);

                //Read rest lines
                while (csv.Read())
                {
                    string[] raw = new string[nHeaders.Length];
                    //Setting row number
                    raw[0] = n++.ToString();

                    for (int i = 1; i < nHeaders.Length; i++)
                    {
                        raw[i] = csv[nHeaders[i]];
                    }

                    lines.Add(raw);
                }

            }

            //call common procedure
            return ImportSealSourceDataFromArray(lines, effectiveDate, SourceDataType.Seals);

        }
    }
}
