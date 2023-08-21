using Mandara.Entities.Trades;

namespace Mandara.Entities.TypeMapping
{
    public class TradeToLiveTradeView
    {
        public static T Convert<T>(TradeCapture trade) where T : LiveTradeView, new()
        {
            T tradeView = TradeToTradeView.Convert<T>(trade);

            tradeView.TradeCapture = trade;
            return tradeView;
        }
    }
}