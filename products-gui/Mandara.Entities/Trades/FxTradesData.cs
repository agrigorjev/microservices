using System.Collections.Generic;

namespace Mandara.Entities.Trades
{
    public class FxTradesData : TradesDataPackBase<FxTrade, FxTradeChange>
    {
        public FxTradesData(List<FxTrade> trades, List<FxTradeChange> tradeChanges, int retryCounter)
            : base(trades, tradeChanges, retryCounter)
        {
        }

        public FxTradesData(int retryCounter) : this(new List<FxTrade>(), new List<FxTradeChange>(), retryCounter)
        {
        }

        public FxTradesData(List<FxTrade> trades, List<FxTradeChange> tradeChanges) : this(trades, tradeChanges, 0)
        {
        }

        public FxTradesData(List<FxTradeChange> tradeChanges, int retryCount)
            : this(new List<FxTrade>(), tradeChanges, retryCount)
        {
        }

        protected override int GetTradeId(FxTrade trade)
        {
            return trade.FxTradeId;
        }

        protected override int GetTradeChangeId(FxTradeChange tradeChg)
        {
            return tradeChg.ChangeId;
        }

        public override string ToString()
        {
            return ToString(
                "Delayed processing - FX Trade IDs: [{0}], Trade Change IDs: [{1}], Retry Counter: [{2}]",
                DefaultMaxDisplayableIdentifiers);
        }
    }
}
