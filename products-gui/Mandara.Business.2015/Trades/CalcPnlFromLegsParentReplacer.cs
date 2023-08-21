using Mandara.Entities;
using Mandara.Extensions.Option;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Mandara.Extensions.Nullable;

namespace Mandara.Business.Trades
{
    public class CalcPnlFromLegsParentReplacer : ICalcPnlFromLegsParentReplacer
    {
        public List<TradeCapture> GetLegsAsParents(List<TradeCapture> trades)
        {
            List<TradeCapture> tradesForReplace = CopyTrades(trades);

            if (AllNewTrades(tradesForReplace))
            {
                return GetLegsAsParentsForNewTrades(tradesForReplace);
            }

            return ReplaceParentWithLegs(tradesForReplace);
        }

        private List<TradeCapture> CopyTrades(List<TradeCapture> inputTrades)
        {
            return new List<TradeCapture>(inputTrades);
        }

        private static bool AllNewTrades(List<TradeCapture> trades)
        {
            return trades.All(trade => trade.TradeId == 0);
        }

        private List<TradeCapture> GetLegsAsParentsForNewTrades(List<TradeCapture> trades)
        {
            SetTradeIdsManually(trades);

            List<TradeCapture> replacedTrades = ReplaceParentWithLegs(trades);

            ClearManualAssignedIds(replacedTrades);

            return replacedTrades;
        }

        private static void SetTradeIdsManually(List<TradeCapture> trades)
        {
            int fakeId = 1;

            trades.ForEach(trade => trade.TradeId = fakeId++);
        }

        private List<TradeCapture> ReplaceParentWithLegs(List<TradeCapture> trades)
        {
            ConcurrentDictionary<int, TradeCapture> tradesByTradeId = new ConcurrentDictionary<int, TradeCapture>();
            Dictionary<string, List<int>> tradesByTradeKey = new Dictionary<string, List<int>>();

            foreach (TradeCapture trade in trades)
            {
                TradesIndexer.IndexTradeByTradeIdAndTradeKey(tradesByTradeId, tradesByTradeKey, trade);
            }

            return GetLegsAsParents(trades, tradesByTradeKey, tradesByTradeId);
        }

        private List<TradeCapture> GetLegsAsParents(
            List<TradeCapture> trades,
            Dictionary<string, List<int>> tradesByTradeKey,
            ConcurrentDictionary<int, TradeCapture> tradesByTradeId)
        {
            List<TradeCapture> parentTrades =
                trades.Where(tc => tc.SecurityDefinition.Product.CalculatePnlFromLegs && tc.IsParentTrade.True())
                      .ToList();

            foreach (TradeCapture parentTrade in parentTrades)
            {
                trades.Remove(parentTrade);

                List<int> spreadTradesIds = tradesByTradeKey[parentTrade.Key];

                foreach (int legTradeId in spreadTradesIds.Where(id => id != parentTrade.TradeId))
                {
                    TradeCapture legTrade = tradesByTradeId[legTradeId];

                    legTrade.IsParentTrade = true;
                    legTrade.IsParentTimeSpread = null;
                }
            }

            return trades;
        }


        public List<TradeCapture> GetLegsAsParents(
            ConcurrentDictionary<int, TradeCapture> tradesByTradeId,
            Dictionary<string, List<int>> tradesByTradeKey,
            TradeCapture tradeInSpread)
        {
            List<int> tradesIds;

            if (!tradesByTradeKey.TryGetValue(tradeInSpread.Key, out tradesIds))
            {
                return new List<TradeCapture>();
            }

            TryGetResult<List<TradeCapture>> spreadTrades = GetCalcPnlFromLegsSpreadTrades(tradesByTradeId, tradesIds);

            if (!spreadTrades.HasValue)
            {
                return new List<TradeCapture>();
            }

            List<TradeCapture> spreadWithLegsAsParents =
                spreadTrades.Value.Where(tc => tc.IsParentTrade.False()).Select(
                    trade =>
                    {
                        trade.IsParentTrade = true;
                        return trade;
                    }).ToList();

            return spreadWithLegsAsParents;
        }

        private static bool IsCalcPnlFromLegsSpread(List<TradeCapture> spreadTrades)
        {
            TryGetResult<TradeCapture> spreadParent = GetSpreadParent(spreadTrades);

            if (!spreadParent.HasValue)
            {
                return false;
            }

            return spreadParent.Value.SecurityDefinition.Product.CalculatePnlFromLegs;
        }

        private static TryGetResult<TradeCapture> GetSpreadParent(List<TradeCapture> spreadTrades)
        {
            TradeCapture spreadParent =
                spreadTrades.FirstOrDefault(trade => trade.ExecID == trade.LegRefID || trade.LegRefID == null);

            return new TryGetRef<TradeCapture>(spreadParent);
        }

        private static void ClearManualAssignedIds(List<TradeCapture> trades)
        {
            trades.ForEach(trade => trade.TradeId = 0);
        }

        private static TryGetResult<List<TradeCapture>> GetCalcPnlFromLegsSpreadTrades(
            ConcurrentDictionary<int, TradeCapture> tradesByTradeId,
            List<int> spreadTradesIds)
        {
            List<TradeCapture> spreadTrades = new List<TradeCapture>();

            foreach (int tradeId in spreadTradesIds)
            {
                TradeCapture cachedTrade;

                if (tradesByTradeId.TryGetValue(tradeId, out cachedTrade))
                {
                    spreadTrades.Add(cachedTrade);
                }
            }

            if (!IsCalcPnlFromLegsSpread(spreadTrades))
            {
                return new TryGetRef<List<TradeCapture>>();
            }

            return new TryGetRef<List<TradeCapture>>(spreadTrades);
        }
    }
}