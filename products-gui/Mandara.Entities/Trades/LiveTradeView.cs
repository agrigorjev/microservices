using System.ComponentModel;

namespace Mandara.Entities.Trades
{
    public class LiveTradeView : TradeView
    {
        [Browsable(false)]
        public TradeCapture TradeCapture { get; set; }

        public decimal? PnL { get; set; }

        [DisplayName("Live PnL Base")]
        public decimal? LivePnLBase { get; set; }

        public decimal? LivePrice { get; set; }

        public decimal? OvernightPrice { get; set; }

        public decimal? PastPnl { get; set; }
    }
}