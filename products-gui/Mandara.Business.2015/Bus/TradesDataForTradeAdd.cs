using Mandara.Entities;
using Mandara.Extensions.Option;
using System.Collections.Generic;

namespace Mandara.Business.Bus
{
    public class TradeDataForTradeAdd
    {
        public List<TradeCapture> TradeCaptures { get; set; }
        public List<TradeCapture> TransferTradeCaptures { get; set; }
        public TryGetResult<string> Warnings { get; set; }
        public List<FxTrade> FxTrades { get; set; }

        public TradeDataForTradeAdd()
        {
            TradeCaptures = new List<TradeCapture>();
            TransferTradeCaptures = new List<TradeCapture>();
            FxTrades = new List<FxTrade>();
            Warnings = new TryGetRef<string>(val => true);
        }
    }
}
