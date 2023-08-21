using Mandara.Entities;
using Mandara.Extensions.Option;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Mandara.Extensions.Nullable;

namespace Mandara.Business.Trades
{
    public static class TradesIndexer
    {
        /// <summary>
        /// Index a trade by TradeId (into a dictionary of trades based on TradeId),
        /// and by TradeKey (into a dictionary of trades identifiers by TradeKey used to identify spread trades).
        /// </summary>
        /// <param name="tradesByTradeId">Trades dictionary with TradeId as a key</param>
        /// <param name="tradesByTradeKey">TradeKey to trades ids dictionary</param>
        /// <param name="trade">Trade to add to dictionaries</param>
        /// <returns>If trade is a part of a spread and is a leg and is not a first trade being added to a list
        /// of spread trades, then we should check already added trades and mark the parent with IsParentTimeSpread
        /// flag. If successful, then spread parent will be returned as it was just marked, otherwise an empty
        /// value will be returned.</returns>
        public static TryGetResult<TradeCapture> IndexTradeByTradeIdAndTradeKey(
            ConcurrentDictionary<int, TradeCapture> tradesByTradeId,
            Dictionary<string, List<int>> tradesByTradeKey,
            TradeCapture trade)
        {
            TryGetResult<TradeCapture> spreadParent = new TryGetRef<TradeCapture>();
            List<int> spreadTradesIds;

            if (tradesByTradeKey.TryGetValue(trade.Key, out spreadTradesIds))
            {
                spreadParent = TryMarkParentTimeSpreadTrade(tradesByTradeId, trade, spreadTradesIds);
            }
            else
            {
                tradesByTradeKey.Add(trade.Key, new List<int> { trade.TradeId });
            }

            tradesByTradeId.AddOrUpdate(
                trade.TradeId,
                trade,
                (tradeId, existingTrade) => SetUpdatedTradeReplacedTimestampIfNull(existingTrade, trade));

            return spreadParent;
        }

        private static TradeCapture SetUpdatedTradeReplacedTimestampIfNull(
            TradeCapture existingTrade,
            TradeCapture updatedTrade)
        {
            if (updatedTrade.LastReplacedTimestamp == null)
            {
                updatedTrade.LastReplacedTimestamp = existingTrade.LastReplacedTimestamp;
            }

            return updatedTrade;
        }

        private static TryGetResult<TradeCapture> TryMarkParentTimeSpreadTrade(
            ConcurrentDictionary<int, TradeCapture> tradesByTradeId,
            TradeCapture trade,
            List<int> spreadTradesIds)
        {
            TryGetResult<TradeCapture> spreadParent = new TryGetRef<TradeCapture>();

            if (spreadTradesIds.Count == 2)
            {
                if (trade.IsParentTrade.True())
                {
                    trade.IsParentTimeSpread = true;
                }
                else
                {
                    spreadParent = GetSpreadParent(tradesByTradeId, spreadTradesIds);

                    if (spreadParent.HasValue)
                    {
                        spreadParent.Value.IsParentTimeSpread = true;
                    }
                }
            }

            if (spreadTradesIds.Count == 3)
            {
                trade.IsParentTimeSpread = trade.IsParentTrade.True();
                SetLegTrades(tradesByTradeId, trade, spreadTradesIds);
            }

            if (!spreadTradesIds.Contains(trade.TradeId))
            {
                spreadTradesIds.Add(trade.TradeId);
            }

            return spreadParent;
        }

        private static TryGetResult<TradeCapture> GetSpreadParent(
            ConcurrentDictionary<int, TradeCapture> tradesByTradeId,
            List<int> spreadTradesIds)
        {
            TradeCapture tradeInSpread;
            TryGetResult<TradeCapture> spreadParentResult = null;

            if (tradesByTradeId.TryGetValue(spreadTradesIds[0], out tradeInSpread))
            {
                spreadParentResult = GetTradeIfParent(tradeInSpread);

                if (!spreadParentResult.HasValue)
                {
                    spreadParentResult = TryGetSpreadParent(tradesByTradeId, spreadTradesIds[1]);
                }
            }

            return spreadParentResult ?? new TryGetRef<TradeCapture>();
        }

        private static TryGetResult<TradeCapture> GetTradeIfParent(TradeCapture tradeInSpread)
        {
            TradeCapture spreadParent = null;

            if (tradeInSpread.IsParentTrade.True())
            {
                spreadParent = tradeInSpread;
            }

            return new TryGetRef<TradeCapture>(spreadParent);
        }

        private static TryGetResult<TradeCapture> TryGetSpreadParent(
            ConcurrentDictionary<int, TradeCapture> tradesByTradeId,
            int spreadTradesId)
        {
            TradeCapture tradeInSpread;

            if (tradesByTradeId.TryGetValue(spreadTradesId, out tradeInSpread))
            {
                return GetTradeIfParent(tradeInSpread);
            }

            return new TryGetRef<TradeCapture>();
        }

        private static void SetLegTrades(
            ConcurrentDictionary<int, TradeCapture> tradesByTradeId,
            TradeCapture parentTrade,
            List<int> spreadTradesIds)
        {
            foreach (int tradeId in spreadTradesIds)
            {
                TradeCapture trade;
                if (!tradesByTradeId.TryGetValue(tradeId, out trade))
                {
                    continue;
                }

                if (trade.Strip.IsTimeSpread)
                {
                    continue;
                }

                if (trade.Strip.Part1.StartDate == parentTrade.Strip.Part1.StartDate)
                {
                    parentTrade.Leg1Trade = trade;
                }
                else
                {
                    parentTrade.Leg2Trade = trade;
                }
            }
        }
    }
}