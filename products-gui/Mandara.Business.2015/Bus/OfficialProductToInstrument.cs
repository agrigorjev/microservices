using Mandara.Business.Bus.Messages.TradeAdd;
using Mandara.Entities;
using Mandara.Entities.Enums;
using Mandara.Extensions.Option;
using System;
using System.Collections.Generic;
using System.Linq;
using Mandara.Date.Time;

namespace Mandara.Business.Bus
{
    public class OfficialProductToInstrument : IOfficialProductToInstrument
    {
        public Instrument ConvertOfficialProductToInstrument(OfficialProduct officialProduct)
        {
            List<Product> allowedProducts = GetAllowedProductsValidNow(officialProduct);

            allowedProducts = FilterForProductsAllowedInManualMode(allowedProducts);

            List<Exchange> exchangeObjects = GetExchangeObjectsForProducts(allowedProducts);
            List<string> exchanges = exchangeObjects.Select(x => x.Name).OrderBy(x => x).ToList();
            Dictionary<string, ExchangeUnits> unitsPerExchange =
                GetUnitsPerExchange(officialProduct, exchangeObjects);

            Instrument instrument = ConstructInstrument(
                officialProduct,
                exchanges,
                unitsPerExchange,
                allowedProducts,
                exchangeObjects);

            return instrument;
        }

        public Instrument ConvertOfficialProductToInstrumentForMasterToolMode(OfficialProduct officialProduct)
        {
            List<Product> allowedProducts = GetAllowedProductsValidNow(officialProduct);
            List<Exchange> exchangeObjects = GetExchangeObjectsForProducts(allowedProducts);
            List<string> exchanges = exchangeObjects.Select(x => x.Name).OrderBy(x => x).ToList();
            Dictionary<string, ExchangeUnits> unitsPerExchange =
                GetUnitsPerExchangeForMasterToolMode(officialProduct, exchangeObjects);

            Instrument instrument = ConstructInstrument(
                officialProduct,
                exchanges,
                unitsPerExchange,
                allowedProducts,
                exchangeObjects);

            return instrument;
        }

        private static List<Product> GetAllowedProductsValidNow(OfficialProduct officialProduct)
        {
            DateTime now = SystemTime.Now();
            List<Product> allowedProducts =
                officialProduct.Products.Where(
                    x => ((x.ValidFrom == null) || (x.ValidFrom <= now)) && ((x.ValidTo == null) || (now <= x.ValidTo)))
                    .ToList();
            return allowedProducts;
        }

        private static List<Exchange> GetExchangeObjectsForProducts(List<Product> products)
        {
            List<Exchange> exchangeObjects =
                products.Where(x => x.Exchange != null).Select(x => x.Exchange).Distinct().ToList();

            if (products.Any(x => x.IsInternalTransferProduct))
            {
                exchangeObjects.Insert(0, new Exchange { Name = "Internal", });
            }
            return exchangeObjects;
        }

        private static List<Product> FilterForProductsAllowedInManualMode(List<Product> allowedProducts)
        {
            allowedProducts = allowedProducts.Where(x => x.IsAllowedForManualTrades).ToList();
            return allowedProducts;
        }

        private static Instrument ConstructInstrument(
            OfficialProduct officialProduct,
            List<string> exchanges,
            Dictionary<string, ExchangeUnits> unitsPerExchange,
            List<Product> products,
            List<Exchange> exchangeObjects)
        {
            Instrument instrument = ConstructBaseInstrument(
                officialProduct,
                exchanges,
                unitsPerExchange,
                products,
                exchangeObjects);

            SetInstrumentFxProductData(products, instrument);

            SetInstrumentInternalTransferProductData(products, instrument);
            return instrument;
        }

        private static Instrument ConstructBaseInstrument(
            OfficialProduct officialProduct,
            List<string> exchanges,
            Dictionary<string, ExchangeUnits> unitsPerExchange,
            List<Product> products,
            List<Exchange> exchangeObjects)
        {
            Instrument instrument = new Instrument
            {
                Id = officialProduct.OfficialProductId,
                Name = officialProduct.DisplayName,
                Exchanges = exchanges,
                ExchangeUnits = unitsPerExchange,
                IsCalcPnlFromLegs = products.All(product => product.CalculatePnlFromLegs),
                HasFutures = products.Any(product => ProductType.Futures == product.Type),
                HasDailySwaps = products.Any(product => ProductType.DailySwap == product.Type),
                HasDailyDiffs = AreDailyDiffsPresent(products),
                HasNonTas = products.Any(product => TasType.NotTas == product.TasType),
                HasTas = products.Any(product => TasType.Tas == product.TasType && ProductType.Swap != product.Type),
                HasMops = products.Any(product => TasType.Mops == product.TasType),
                HasMm = products.Any(product => TasType.Mm == product.TasType),
                HasMoc = products.Any(product => TasType.Tas == product.TasType && ProductType.Swap == product.Type),
                DailySwapUnits = GetDailyUnits(officialProduct, exchangeObjects, ProductType.DailySwap),
                // TODO: Include DayVsMonthCustom?
                DailyDiffUnits = GetDailyUnits(officialProduct, exchangeObjects, ProductType.DayVsMonthFullWeek),
                Currency = officialProduct.Currency.IsoName
            };
            return instrument;
        }

        private static bool AreDailyDiffsPresent(List<Product> products)
        {
            return products.Any(product => (product.Type.IsDailyOrWeeklyDiff()));
        }

        private static void SetInstrumentFxProductData(List<Product> products, Instrument instrument)
        {
            List<Product> fxProducts = products.Where(it => it.Type == ProductType.Spot).ToList();

            instrument.FxTradesExchanges =
                fxProducts.Where(it => it.Exchange != null).Select(it => it.Exchange.Name).Distinct().ToList();

            if (fxProducts.Count > 0)
            {
                instrument.FxSpecifiedCurrency = fxProducts[0].Currency1.IsoName;
                instrument.FxAgainstCurrency = fxProducts[0].Currency2.IsoName;
            }
        }

        private static void SetInstrumentInternalTransferProductData(List<Product> products, Instrument instrument)
        {
            List<Product> internalProducts = products.Where(x => x.IsInternalTransferProductDb == true).ToList();

            if (internalProducts.Count > 0)
            {
                instrument.ExpiryExchanges =
                    internalProducts.Where(x => x.Exchange != null)
                        .Select(x => x.Exchange.Name)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList();
            }
        }

        private static Dictionary<string, ExchangeUnits> GetUnitsPerExchange(
            OfficialProduct officialProduct,
            List<Exchange> exchangeObjects)
        {
            ICollection<Product> allowedProducts = GetAllowedProducts(officialProduct);

            return GetUnitsPerExchangeForProducts(exchangeObjects, allowedProducts);
        }

        private static Dictionary<string, ExchangeUnits> GetUnitsPerExchangeForMasterToolMode(
            OfficialProduct officialProduct,
            List<Exchange> exchangeObjects)
        {
            ICollection<Product> allowedProducts = GetAllowedProductsForMasterToolMode(officialProduct);

            return GetUnitsPerExchangeForProducts(exchangeObjects, allowedProducts);
        }

        private static Dictionary<string, ExchangeUnits> GetUnitsPerExchangeForProducts(
            List<Exchange> exchangeObjects,
            ICollection<Product> products)
        {
            Dictionary<string, ExchangeUnits> unitsPerExchange = new Dictionary<string, ExchangeUnits>();

            foreach (Exchange exchangeObject in exchangeObjects)
            {
                List<Product> exchangeProducts = GetExchangeProducts(exchangeObject, products);
                TryGetResult<ExchangeUnits> exchUnitsResult = GetUnitsForProducts(exchangeProducts);

                if (exchUnitsResult.HasValue)
                {
                    unitsPerExchange.Add(exchangeObject.Name, exchUnitsResult.Value);
                }
            }

            return unitsPerExchange;
        }

        private static List<Product> GetExchangeProducts(Exchange exchangeObject, ICollection<Product> products)
        {
            List<Product> exchangeProducts;

            if (exchangeObject.Name.Equals("internal", StringComparison.InvariantCultureIgnoreCase))
            {
                exchangeProducts = products.Where(x => x.IsInternalTransferProductDb == true).ToList();
            }
            else
            {
                exchangeProducts =
                    products.Where(
                        x => (x.Exchange != null) && (x.Exchange.ExchangeId == exchangeObject.ExchangeId)).ToList();
            }
            return exchangeProducts;
        }

        private static TryGetResult<ExchangeUnits> GetUnitsForProducts(List<Product> products)
        {

            List<int> availableUnits = GetUnitsByProductType(
                products,
                (product) => product.Type != ProductType.Balmo);

            List<int> balmoUnits = GetUnitsByProductType(
                products,
                (product) => (product.Type == ProductType.Balmo) || (product.Type == ProductType.Swap));

            TryGetRef<ExchangeUnits> exchangeUnits = new TryGetRef<ExchangeUnits>();

            if ((availableUnits.Count > 0) || (balmoUnits.Count > 0))
            {
                exchangeUnits.Value = new ExchangeUnits()
                {
                    AvailableUnits = availableUnits,
                    BalmoUnits = balmoUnits,
                    HasBalmo = balmoUnits.Count > 0,
                };
            }

            return exchangeUnits;
        }

        private static List<int> GetUnitsByProductType(List<Product> products, Predicate<Product> productTypeFilter)
        {
            List<int> availableUnits =
                products.Where(product => productTypeFilter(product))
                    .Select(x => x.Unit.UnitId)
                    .Distinct()
                    .ToList();
            return availableUnits;
        }

        private static ICollection<Product> GetAllowedProducts(OfficialProduct officialProduct)
        {
            return officialProduct.Products.Where(x => x.IsAllowedForManualTrades).ToList();
        }

        private static ICollection<Product> GetAllowedProductsForMasterToolMode(OfficialProduct officialProduct)
        {
            return officialProduct.Products;
        }

        private static Dictionary<string, List<int>> GetDailyUnits(
            OfficialProduct officialProduct,
            List<Exchange> exchangeObjects,
            ProductType dailyType)
        {
            Dictionary<string, List<int>> dailySwapUnits = new Dictionary<string, List<int>>();

            foreach (Exchange exchangeObject in exchangeObjects)
            {
                List<Product> exchangeProducts;
                if (exchangeObject.Name.Equals("internal", StringComparison.InvariantCultureIgnoreCase))
                {
                    exchangeProducts =
                        officialProduct.Products.Where(x => x.IsInternalTransferProductDb == true).ToList();
                }
                else
                {
                    exchangeProducts =
                        officialProduct.Products.Where(
                            x => (x.Exchange != null) && (x.Exchange.ExchangeId == exchangeObject.ExchangeId)).ToList();
                }

                List<int> availableUnits =
                    exchangeProducts.Where(x => x.Type == dailyType).Select(x => x.Unit.UnitId).Distinct().ToList();

                dailySwapUnits.Add(exchangeObject.Name, availableUnits);
            }

            return dailySwapUnits;
        }
    }
}
