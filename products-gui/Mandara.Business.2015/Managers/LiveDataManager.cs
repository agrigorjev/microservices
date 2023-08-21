using Mandara.Entities;
using Mandara.Entities.EntitiesCustomization;
using Ninject.Extensions.Logging;

namespace Mandara.Business
{
    public static class LiveDataManager
    {
        public static string LiveDatabase;
        private static ILogger _log = new NLogLoggerFactory().GetCurrentClassLogger();

        public static SourceDetail ConvertTradeCaptureToSourceDetail(TradeCapture trade, Product product = null)
        {
            if (trade == null)
            {
                _log.Trace("ConvertTradeCaptureToSourceDetail: No trade...");
                return null;
            }

            _log.Trace(
                "ConvertTradeCaptureToSourceDetail: Product passed in is null (so use trade's SecDef product)? {0}",
                product == null);
            product = product ?? trade.SecurityDefinition.Product;

            Strip strip = Strip.Default;

            if (product != null)
            {
                _log.Trace(
                    "ConvertTradeCaptureToSourceDetail: Product is not null, so parsing the strip data from the trade");
                strip = StripParser.Parse(trade, product);
            }

            if (strip.IsDefault())
            {
                LiveDataManager._log.Trace(
                    "ConvertTradeCaptureToSourceDetail: The trade didn't have strip data to parse...");
                return null;
            }

            SourceDetail sourceDetail = new SourceDetail
            {
                SourceDetailId = trade.TradeId,
                StripName = trade.SecurityDefinition.StripName,
                IsTimeSpread = strip.IsTimeSpread,
                InstrumentDescription = trade.SecurityDefinition.UnderlyingSecurityDesc,
                Product = product,
                Quantity = trade.Quantity.Value,
                TradePrice = trade.Price.Value,
                TransactTime = trade.TransactTime,
                MaturityDate = trade.SecurityDefinition.UnderlyingMaturityDateAsDate,
                TradeCapture = trade,
                PortfolioId = trade.Portfolio != null ? trade.Portfolio.PortfolioId : (int?)null,
                TradeCaptureId = trade.TradeId,
                ProductId = product.ProductId,
                UseExpiryCalendar = product.UseExpiryCalendar.HasValue && product.UseExpiryCalendar.Value,
            };

            sourceDetail.ProductDate = sourceDetail.ProductDate1 = strip.Part1.StartDate;
            sourceDetail.DateType = sourceDetail.DateType1 = strip.Part1.DateType;

            if (strip.IsTimeSpread)
            {
                sourceDetail.ProductDate2 = strip.Part2.StartDate;
                sourceDetail.DateType2 = strip.Part2.DateType;
            }

            if (trade.TradeEndDate.HasValue)
                sourceDetail.TradeEndDate = trade.TradeEndDate.Value;

            return sourceDetail;
        }
    }
}
