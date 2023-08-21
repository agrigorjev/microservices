using Mandara.Business.Managers;
using Mandara.Business.Model;
using Mandara.Business.Services.Prices;
using Mandara.Entities;
using Mandara.Entities.EntitiesCustomization;
using Mandara.Extensions.Option;
using Mandara.IRM.Server.Services;
using System;
using System.Collections.Generic;

namespace Mandara.Business.Extensions
{
    public static class PricesStorageExtensions
    {
        public static LivePrice GetTradePrice(this IPricesProvider pricesProvider, TradeCapture trade)
        {
            List<Money> liveLegPrices = new List<Money>();
            Product product = trade.SecurityDefinition.Product;

            liveLegPrices = GetTradePrices(trade, (i) => pricesProvider);

            return CalculateLiveTradePrice(liveLegPrices, product);
        }

        private static LivePrice CalculateLiveTradePrice(List<Money> liveLegPrices, Product product)
        {
            LivePrice livePrice = new LivePrice();

            if (liveLegPrices.Count == 2)
            {
                livePrice.Leg1LivePrice = liveLegPrices[0];
                livePrice.Leg2LivePrice = liveLegPrices[1];

                livePrice.TradeLivePrice = livePrice.Leg1LivePrice - livePrice.Leg2LivePrice;
            }

            if (liveLegPrices.Count == 1)
            {
                livePrice.TradeLivePrice =
                    livePrice.Leg1LivePrice = new Money(liveLegPrices[0], product.OfficialProduct.Currency.IsoName);
            }

            return livePrice;
        }

        private static List<Money> GetTradePrices(
            TradeCapture trade,
            Func<int, IPricesProvider> stripPricesProviderSelector)
        {
            List<Money> tradeStripPrices = new List<Money>();
            Product product = trade.SecurityDefinition.Product;
            StripPart[] stripParts = new[] { trade.Strip.Part1, trade.Strip.Part2 };

            for (int i = 0; i < stripParts.Length; i++)
            {
                if (stripParts[i].IsDefault())
                {
                    continue;
                }

                IPricesProvider pricesProvider = stripPricesProviderSelector(i);
                DateTime productDate = stripParts[i].StartDate;
                ProductDateType productDateType = stripParts[i].DateType;

                TryGetResult<Money> price = pricesProvider.TryGetProductPrice(
                    product.ProductId,
                    productDate,
                    productDateType,
                    product.OfficialProduct.MappingColumn,
                    product.OfficialProductId,
                    trade.TradeStartDate,
                    trade.TradeEndDate);

                if (price.HasValue)
                {
                    tradeStripPrices.Add(price.Value);
                }
            }

            return tradeStripPrices;
        }

        public static LivePrice GetSnapshotTradePrice(
            this IPricesProvider snapshotPricesProvider,
            TradeCapture trade,
            IPricesProvider livePricesProvider)
        {
            // The first strip uses the given snapshot prices provider, but the second strip uses the live prices 
            // provider.
            List<Money> liveLegPrices = GetTradePrices(trade, (i) => i == 0 ? snapshotPricesProvider : livePricesProvider);
            Product product = trade.SecurityDefinition.Product;

            return CalculateLiveTradePrice(liveLegPrices, product);
        }

        public static LivePrice GetTradePrice(this IPricesStorage pricesStorage, TradeCapture trade)
        {
            Product product = trade.SecurityDefinition.Product;
            List<Money> liveLegPrices = PricesStorageExtensions.GetTradePrices(trade, product, (i) => pricesStorage);

            return CalculateLiveTradePrice(liveLegPrices, product);
        }

        private static List<Money> GetTradePrices(
            TradeCapture trade,
            Product product,
            Func<int, IPricesStorage> stripPricesStorageSelector)
        {
            List<Money> livePrice = new List<Money>();
            StripPart[] stripParts = new[] { trade.Strip.Part1, trade.Strip.Part2 };

            for (int i = 0; i < stripParts.Length; i++)
            {
                if (stripParts[i].IsDefault() || trade.SecurityDefinitionId == SecurityDefinitionsManager.FxSecDefId)
                {
                    continue;
                }

                DateTime productDate = stripParts[i].StartDate;
                ProductDateType productDateType = stripParts[i].DateType;
                IPricesStorage pricesStorage = stripPricesStorageSelector(i);

                TryGetResult<Money> price = pricesStorage.GetProductPrice(
                    product.ProductId,
                    productDate,
                    productDateType,
                    product.OfficialProduct.MappingColumn,
                    product.OfficialProductId,
                    trade.TradeStartDate,
                    trade.TradeEndDate);

                if (price.HasValue)
                {
                    livePrice.Add(price.Value);
                }
            }

            return livePrice;
        }
    }
}