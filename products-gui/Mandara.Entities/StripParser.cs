using Mandara.Entities.EntitiesCustomization;
using Mandara.Entities.EntityPieces;
using Mandara.Entities.Extensions;
using System;
using Mandara.Date;

namespace Mandara.Entities
{
    public static class StripParser
    {
        public static Strip Parse(string stripName, DateTime startDate)
        {
            if (String.IsNullOrWhiteSpace(stripName))
            {
                return Strip.Default;
            }

            string[] parts = stripName.Split('/');

            Tuple<DateTime, ProductDateType> part1 = StripHelper.ParseStripDate(parts[0], startDate);

            Tuple<DateTime, ProductDateType> part2 = null;
            if (parts.Length == 2)
                part2 = StripHelper.ParseStripDate(parts[1], startDate);

            return new Strip(part1, part2);
        }

        public static Strip Parse(TradePieces trade)
        {
            return Parse(trade.Trade, trade.Security.SecurityDef, trade.Security.Product);
        }

        public static Strip Parse(TradeCapture trade, SecurityDefinition secDef, Product product)
        {
            if (product.IsProductDaily)
            {
                if (trade.TradeStartDate == null)
                {
                    return Strip.Default;
                }

                return new Strip(
                    new Tuple<DateTime, ProductDateType>(trade.TradeStartDate.Value, ProductDateType.Daily));
            }

            if (string.IsNullOrEmpty(secDef.StripName))
            {
                return Strip.Default;
            }

            Tuple<DateTime, ProductDateType> part1 = GetProductDateAndType(true, trade, product, secDef);
            Tuple<DateTime, ProductDateType> part2 = GetProductDateAndType(false, trade, product, secDef);

            if (trade.TradeStartDate == null)
            {
                trade.TradeStartDate = part1.Item1;
            }

            return new Strip(part1, part2);
        }

        public static Strip Parse(TradeCapture trade)
        {
            if (trade.SecurityDefinition == null || trade.SecurityDefinition.Product == null)
            {
                return Strip.Default;
            }

            return Parse(trade, trade.SecurityDefinition, trade.SecurityDefinition.Product);
        }

        public static Strip Parse(TradeCapture trade, Product product)
        {
            return Parse(trade, trade.SecurityDefinition, product);
        }

        private static Tuple<DateTime, ProductDateType> GetProductDateAndType(
            bool firstStrip,
            TradeCapture trade,
            Product product,
            SecurityDefinition securityDefinition)
        {
            securityDefinition = securityDefinition ?? trade.SecurityDefinition;

            string[] strips = securityDefinition.StripName.Split('/');
            DateTime startDate = trade.TradeStartDate.HasValue ? trade.TradeStartDate.Value : DateTime.MinValue;

            if (firstStrip)
            {
                return ParseFirstStrip(trade, product, securityDefinition, strips, startDate);
            }

            if (strips.Length == 2)
            {
                return StripHelper.ParseStripDate(strips[1], startDate, trade.TransactTime);
            }

            return new Tuple<DateTime, ProductDateType>(StripPart.Default.StartDate, StripPart.Default.DateType);
        }

        private static Tuple<DateTime, ProductDateType> ParseFirstStrip(
            TradeCapture trade,
            Product product,
            SecurityDefinition securityDefinition,
            string[] strips,
            DateTime startDate)
        {
            bool nymexBalmoTrade = false;
            DateTime nymexBalmoDate = DateTime.MinValue;
            string securityDefinitionExchange = securityDefinition.Exchange ?? string.Empty;

            if (securityDefinitionExchange.EqualTrimmed(
                Exchange.NymexExchangeName,
                StringComparison.InvariantCultureIgnoreCase))
            {
                string startDateString = securityDefinition.StartDate ?? string.Empty;

                if (startDateString.Trim().Length == 10)
                {
                    DateTime? sdStartDate = StripHelper.ParseDate(
                        startDateString.Substring(0, 8),
                        Formats.SortableShortDate);

                    if (sdStartDate.HasValue && trade.TradeDate.HasValue)
                    {
                        if (sdStartDate.Value.Year == trade.TradeDate.Value.Year
                            && sdStartDate.Value.Month == trade.TradeDate.Value.Month
                            && sdStartDate.Value.Day != 1)
                        {
                            nymexBalmoTrade = true;
                            nymexBalmoDate = sdStartDate.Value;
                        }
                    }
                }
            }

            if (nymexBalmoTrade)
            {
                securityDefinition.StripName = "Bal Month";
                return Tuple.Create(nymexBalmoDate, ProductDateType.Day);
            }

            if (product.Type == ProductType.Balmo)
            {
                ProductDateType type = ProductDateType.Day;
                DateTime prDate = trade.TradeStartDate ?? trade.TransactTime.Value.Date;

                return Tuple.Create(prDate, type);
            }

            return StripHelper.ParseStripDate(strips[0], startDate, trade.TransactTime);
        }

        public static Strip Parse(SourceDetail sourceDetail)
        {
            ProductType productType = sourceDetail.Product.Type;

            if (productType.IsDaily())
            {
                return new Strip(new Tuple<DateTime, ProductDateType>(sourceDetail.ProductDate, ProductDateType.Daily));
            }

            Tuple<DateTime, ProductDateType> part1 = GetProductDateAndType(true, sourceDetail);
            Tuple<DateTime, ProductDateType> part2 = GetProductDateAndType(false, sourceDetail);

            return new Strip(part1, part2);
        }

        private static Tuple<DateTime, ProductDateType> GetProductDateAndType(bool firstStrip, SourceDetail sourceDetail)
        {
            if (firstStrip)
            {
                return Tuple.Create(sourceDetail.ProductDate, sourceDetail.DateType);
            }

            return null;
        }
    }
}