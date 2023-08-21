using System.Collections.Generic;

namespace Mandara.Entities.Trades
{
    public class UnprocessedTradesDataPack : TradesDataPackBase<TradeCapture, TradeChange>
    {
        public UnprocessedTradesDataPack(List<TradeCapture> trades, List<TradeChange> tradeChanges, int retryCounter)
            : base(trades, tradeChanges, retryCounter)
        {
        }

        public UnprocessedTradesDataPack(List<TradeCapture> trades, List<TradeChange> tradeChanges)
            : this(trades, tradeChanges, 0)
        {
        }

        public UnprocessedTradesDataPack(List<TradeChange> tradeChanges, int retryCounter)
            : this(new List<TradeCapture>(), tradeChanges, retryCounter)
        {
        }

        public UnprocessedTradesDataPack(int retryCounter)
            : this(new List<TradeCapture>(), new List<TradeChange>(), retryCounter)
        {
        }

        protected override int GetTradeId(TradeCapture trade)
        {
            return trade.TradeId;
        }

        protected override int GetTradeChangeId(TradeChange tradeChg)
        {
            return tradeChg.ChangeId;
        }

        public override string ToString()
        {
            return ToString(
                "Delayed processing - Trade IDs: [{0}], Trade Change IDs: [{1}], Retry Counter: [{2}]",
                DefaultMaxDisplayableIdentifiers);
        }
    }
}