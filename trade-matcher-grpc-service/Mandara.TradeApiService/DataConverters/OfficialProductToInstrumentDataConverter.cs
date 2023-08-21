using Mandara.GrpcDefinitions.Extensions;
using Mandara.TradeApiService.Data;
using Mandara.TradeApiService.Contracts;

namespace Mandara.TradeApiService.DataConverters
{
    public class OfficialProductToInstrumentDataConverter : IDataConverter<OfficialProduct, InstrumentGrpc>
    {
        public InstrumentGrpc Convert(OfficialProduct officialProduct)
        {
            List<Product> allowedProducts = GetAllowedProductsValidNow(officialProduct);

            allowedProducts = FilterForProductsAllowedInManualMode(allowedProducts);

            List<Exchange> exchangeObjects = GetExchangeObjectsForProducts(allowedProducts);
            List<string> exchanges = exchangeObjects.Select(x => x.Name).OrderBy(x => x).ToList();
            Dictionary<string, ExchangeUnitsGrpc> unitsPerExchange =
                GetUnitsPerExchange(officialProduct, exchangeObjects);

            InstrumentGrpc instrument = ConstructInstrument(
                officialProduct,
                exchanges,
                unitsPerExchange,
                allowedProducts,
                exchangeObjects);

            return instrument;
        }

        public InstrumentGrpc ConvertOfficialProductToInstrumentForMasterToolMode(OfficialProduct officialProduct)
        {
            List<Product> allowedProducts = GetAllowedProductsValidNow(officialProduct);
            List<Exchange> exchangeObjects = GetExchangeObjectsForProducts(allowedProducts);
            List<string> exchanges = exchangeObjects.Select(x => x.Name).OrderBy(x => x).ToList();
            Dictionary<string, ExchangeUnitsGrpc> unitsPerExchange =
                GetUnitsPerExchangeForMasterToolMode(officialProduct, exchangeObjects);

            InstrumentGrpc instrument = ConstructInstrument(
                officialProduct,
                exchanges,
                unitsPerExchange,
                allowedProducts,
                exchangeObjects);

            return instrument;
        }

        private static List<Product> GetAllowedProductsValidNow(OfficialProduct officialProduct)
        {
            DateTime now = DateTime.Now;
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

        private static InstrumentGrpc ConstructInstrument(
            OfficialProduct officialProduct,
            List<string> exchanges,
            Dictionary<string, ExchangeUnitsGrpc> unitsPerExchange,
            List<Product> products,
            List<Exchange> exchangeObjects)
        {
            InstrumentGrpc instrument = ConstructBaseInstrument(
                officialProduct,
                exchanges,
                unitsPerExchange,
                products,
                exchangeObjects);

            SetInstrumentFxProductData(products, instrument);

            SetInstrumentInternalTransferProductData(products, instrument);
            return instrument;
        }

        private static InstrumentGrpc ConstructBaseInstrument(
            OfficialProduct officialProduct,
            List<string> exchanges,
            Dictionary<string, ExchangeUnitsGrpc> unitsPerExchange,
            List<Product> products,
            List<Exchange> exchangeObjects)
        {
            InstrumentGrpc instrument = new InstrumentGrpc
            {
                Id = officialProduct.OfficialProductId,
                Name = officialProduct.DisplayName,
                IsCalcPnlFromLegs = products.All(product => product.CalculatePnlFromLegs),
                HasFutures = products.Any(product => ProductType.Futures == product.Type),
                HasDailySwaps = products.Any(product => ProductType.DailySwap == product.Type),
                HasDailyDiffs = AreDailyDiffsPresent(products),
                HasNoTas = products.Any(product => TasType.NotTas == product.TasType),
                HasTas = products.Any(product => TasType.Tas == product.TasType && ProductType.Swap != product.Type),
                HasMops = products.Any(product => TasType.Mops == product.TasType),
                HasMm = products.Any(product => TasType.Mm == product.TasType),
                HasMoc = products.Any(product => TasType.Tas == product.TasType && ProductType.Swap == product.Type),
                //DailySwapUnits = GetDailyUnits(officialProduct, exchangeObjects, ProductType.DailySwap),
                // TODO: Include DayVsMonthCustom?
                //DailyDiffUnits = GetDailyUnits(officialProduct, exchangeObjects, ProductType.DayVsMonthFullWeek),
                Currency = officialProduct.Currency.IsoName
            };
            instrument.Exchanges.AddRange(exchanges);
            instrument.ExchangeUnits.Add(unitsPerExchange);

            return instrument;
        }

        private static bool AreDailyDiffsPresent(List<Product> products)
        {
            return products.Any(product => (product.Type.IsDailyOrWeeklyDiff()));
        }

        private static void SetInstrumentFxProductData(List<Product> products, InstrumentGrpc instrument)
        {
            List<Product> fxProducts = products.Where(it => it.Type == ProductType.Spot).ToList();

            instrument.FxTradesExchanges.AddRange(
                fxProducts.Where(it => it.Exchange != null).Select(it => it.Exchange.Name).Distinct().ToList());

            if (fxProducts.Count > 0)
            {
                instrument.FxSpecifiedCurrency = fxProducts[0].Currency1.IsoName;
                instrument.FxAgainstCurrency = fxProducts[0].Currency2.IsoName;
            }
        }

        private static void SetInstrumentInternalTransferProductData(List<Product> products, InstrumentGrpc instrument)
        {
            List<Product> internalProducts = products.Where(x => x.IsInternalTransferProductDb == true).ToList();

            if (internalProducts.Count > 0)
            {
                instrument.ExpiryExchanges.AddRange(
                    internalProducts.Where(x => x.Exchange != null)
                        .Select(x => x.Exchange.Name)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList()
                        );
            }
        }

        private static Dictionary<string, ExchangeUnitsGrpc> GetUnitsPerExchange(
            OfficialProduct officialProduct,
            List<Exchange> exchangeObjects)
        {
            ICollection<Product> allowedProducts = GetAllowedProducts(officialProduct);

            return GetUnitsPerExchangeForProducts(exchangeObjects, allowedProducts);
        }

        private static Dictionary<string, ExchangeUnitsGrpc> GetUnitsPerExchangeForMasterToolMode(
            OfficialProduct officialProduct,
            List<Exchange> exchangeObjects)
        {
            ICollection<Product> allowedProducts = GetAllowedProductsForMasterToolMode(officialProduct);

            return GetUnitsPerExchangeForProducts(exchangeObjects, allowedProducts);
        }

        private static Dictionary<string, ExchangeUnitsGrpc> GetUnitsPerExchangeForProducts(
            List<Exchange> exchangeObjects,
            ICollection<Product> products)
        {
            Dictionary<string, ExchangeUnitsGrpc> unitsPerExchange = new Dictionary<string, ExchangeUnitsGrpc>();

            foreach (Exchange exchangeObject in exchangeObjects)
            {
                List<Product> exchangeProducts = GetExchangeProducts(exchangeObject, products);
                ExchangeUnitsGrpc? exchUnitsResult = GetUnitsForProducts(exchangeProducts);

                if (exchUnitsResult != null)
                {
                    unitsPerExchange.Add(exchangeObject.Name, exchUnitsResult);
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

        private static ExchangeUnitsGrpc? GetUnitsForProducts(List<Product> products)
        {

            List<int> availableUnits = GetUnitsByProductType(
                products,
                (product) => product.Type != ProductType.Balmo);

            List<int> balmoUnits = GetUnitsByProductType(
                products,
                (product) => (product.Type == ProductType.Balmo) || (product.Type == ProductType.Swap));

            ExchangeUnitsGrpc? exchangeUnits = null;

            if ((availableUnits.Count > 0) || (balmoUnits.Count > 0))
            {
                var t = new ExchangeUnitsGrpc();

                t.AvailableUnits.AddRange(availableUnits);
                t.BalmoUnits.AddRange(balmoUnits);
                t.HasBalmo = balmoUnits.Count > 0;

                exchangeUnits = t;
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
