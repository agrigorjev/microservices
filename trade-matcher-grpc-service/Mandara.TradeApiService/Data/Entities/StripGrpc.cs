using Mandara.GrpcDefinitions.Extensions;
using Mandara.TradeApiService;
using Mandara.TradeApiService.Data;

namespace Mandara.GrpcDefinitions.Extensions
{
    public partial class StripGrpc
    {
        public static readonly string DailySwapStripName = "Custom CFD";
        public static readonly string DailyDiffStripName = "Custom CFD (Month)";

        public static StripGrpc FromTradeCapture(TradeCapture tradeCapture, Func<string[], string> selectStripName)
        {
            SecurityDefinition secDef = tradeCapture.SecurityDefinition;

            return secDef.Product.Type.IsDaily()
                ? GetDailyProductStrip(tradeCapture, secDef.Product.Type)
                : GetNonDailyProductStrip(tradeCapture, secDef, selectStripName);
        }

        private static StripGrpc GetDailyProductStrip(TradeCapture trade, ProductType tradedType)
        {
            StripGrpc dailyStrip = new StripGrpc
            {
                StringValue = ProductType.DailySwap == tradedType
                ? DailySwapStripName
                : DailyDiffStripName,
                StartDate = trade.TradeStartDate.toProtoTimestamp(),
                EndDate = trade.TradeEndDate.toProtoTimestamp()
            };
            return dailyStrip;
        }

        private static StripGrpc GetNonDailyProductStrip(
            TradeCapture trade,
            SecurityDefinition security,
            Func<string[], string> selectStripName)
        {
            StripGrpc strip = new StripGrpc();
            string[] stripParts = security.StripName.Split('/');
            string stripName = selectStripName(stripParts);

            Tuple<DateTime, ProductDateType> liveTradeDate = StripHelper.ParseStripDate(
                stripName,
                trade.TradeStartDate ?? DateTime.MinValue,
                trade.TransactTime);

            strip.StringValue = stripName;
            strip.StartDate = liveTradeDate.Item1.toProtoTimestamp();

            if (liveTradeDate.Item2 == ProductDateType.Day)
            {
                strip.StringValue = "Balmo";
                strip.EndDate = trade.TradeEndDate.toProtoTimestamp();
                strip.IsBalmoStrip = true;
            }

            if (liveTradeDate.Item2 == ProductDateType.Custom)
            {
                strip.EndDate = trade.TradeEndDate.toProtoTimestamp();
            }

            return strip;
        }

        public static StripGrpc FromTradeCapture(TradePieces tradeAndSecDef, Func<string[], string> selectStripName)
        {
            ProductType tradedType = tradeAndSecDef.Security.Product.Type;
            TradeCapture trade = tradeAndSecDef.Trade;
            SecurityDefinition security = tradeAndSecDef.Security.SecurityDef;

            return tradedType.IsDaily()
                ? GetDailyProductStrip(trade, tradedType)
                : GetNonDailyProductStrip(trade, security, selectStripName);
        }

        public static Func<string[], string> DefaultStripNameSelector(bool useFirstStrip)
        {
            return (stripParts) => useFirstStrip ? stripParts[0] : stripParts[1];
        }
    }
}
