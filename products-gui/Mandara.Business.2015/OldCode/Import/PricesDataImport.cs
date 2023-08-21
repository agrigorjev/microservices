using CsvHelper;
using Mandara.Business;
using Mandara.Entities;
using Mandara.Entities.Exceptions;
using Mandara.Entities.Import;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Mandara.Import
{
    public class PricesDataImport
    {
        private readonly ProductManager _productManager = new ProductManager();
        private readonly ILogger _logger = new NLogLoggerFactory().GetCurrentClassLogger();

        public List<Tuple<DataImportException, ProductPriceDetail>> LoadFromCsv(string filename)
        {
            List<Tuple<DataImportException, ProductPriceDetail>> lines = new List<Tuple<DataImportException, ProductPriceDetail>>();
            CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                Dictionary<string, Action<string, PricesRawData>> parseDict = new Dictionary<string, Action<string, PricesRawData>>();

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
                        lines.Add(GetCsvRow(headers, csv, parseDict, productRowNumber));

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

        private Tuple<DataImportException, ProductPriceDetail> GetCsvRow(string[] headers,
                                                                               CsvReader reader,
                                                                               Dictionary<string, Action<string, PricesRawData>> parseDict,
                                                                               int productRowNumber)
        {
            var rawData = new PricesRawData();

            foreach (string header in headers)
            {
                if (parseDict.ContainsKey(header))
                    parseDict[header](reader[header], rawData);
            }

            DataImportException e = null;
            ProductPriceDetail productPriceDetail = null;

            try
            {
                productPriceDetail = new ProductPriceDetail
                {
                    Price = rawData.Price,
                    Date = rawData.Date
                };

                if (_productManager != null)
                {
                    OfficialProduct officialProduct = _productManager.GetOfficialProduct(rawData.RawProductDisplayName);

                    if (officialProduct == null)
                        throw new DataImportException(
                            string.Format("Could not map product with display name [{0}].", rawData.RawProductDisplayName),
                            productRowNumber);

                    productPriceDetail.OfficialProduct = officialProduct;
                }
            }
            catch (DataImportException ex)
            {
                _logger.Error(ex, "PricesDataImport.GetCsvRow - Data import exception reading row.");
                e = ex;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "PricesDataImport.GetCsvRow - General exception reading row.");
                e = new DataImportException(ex.Message, productRowNumber);
            }

            return new Tuple<DataImportException, ProductPriceDetail>(e, productPriceDetail);
        }

        private IEnumerable<Tuple<string, Action<string, PricesRawData>>> GetParseDictionary()
        {
            foreach (PropertyInfo propertyLoop in typeof(PricesRawData).GetProperties())
            {
                PropertyInfo property = propertyLoop;
                object[] attributeInfo = property.GetCustomAttributes(true);
                object attribute = attributeInfo.SingleOrDefault(a => typeof(RawDataAttribute)
                                                                          .IsAssignableFrom(a.GetType()));

                if (attribute != null)
                {
                    var rawDataAttribute = (RawDataAttribute)attribute;

                    yield return
                        new Tuple<string, Action<string, PricesRawData>>(rawDataAttribute.FieldName,
                                                                         (v, obj) => property.SetValue(obj, v, null));
                }
            }
        }
    }
}