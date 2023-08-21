using Mandara.Entities;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Mandara.Business.Trades
{
    /// <summary>
    /// Class handles trades for Products marked with CalculatePnlOnLegs flag.
    /// If we have a spread with a parent with the flag set we will remove
    /// parent from the list and mark legs as parents.
    /// </summary>
    public interface ICalcPnlFromLegsParentReplacer
    {
        /// <summary>
        /// Replaces parent trade with it's legs in a list of trades provided.
        /// Assumes trades have <see cref="SecurityDefinition"/> and <see cref="Product"/> set.
        /// </summary>
        /// <param name="trades">Trades list where CalculatePnlOnLegs parent will be removed and it's
        /// legs will be marked marked as parent trades.
        /// If CalculatePnlOnLegs parent will not be found the list won't be changed.</param>
        /// <returns>The legs marked as parents <see cref="TradeCaptures"/>.  If none of the trades in the collection
        /// is on a 'Calculate PnL on Legs' product then an empty collection is returned.</returns>
        List<TradeCapture> GetLegsAsParents(List<TradeCapture> trades);

        /// <summary>
        /// Process one trade assuming we already have a list of trades in a dictionary as well
        /// as a mapping for trades by TradeKey to identify spread the provided trade participate in.
        /// If the assumption doesn't hold an empty result should be returned.
        /// </summary>
        /// <param name="tradesByTradeId">Dictionary of all the trades with a TradeId as a key.</param>
        /// <param name="tradesByTradeKey">Dictionary to map a TradeKey to a list of spread trades identifiers.</param>
        /// <param name="trade">Trade we need to process. If it's a part of a spread where parent trade product has
        /// CalculatePnlOnLegs flag set and legs are not marked as parents yet, we will marke them as parents and
        /// return as a result.</param>
        /// <returns>If trade is a related to a spread with CalcualtePnlOnLegs parent trade, 
        /// then non parent legs should be marked as parents and returned.
        /// If all the legs are already parents we should return empty result as a valid value.
        /// If passed trade is not a part of a spread or spread does not have parent product marked as 
        /// CalculatePnlOnLegs then empty result will be returned.</returns>
        List<TradeCapture> GetLegsAsParents(
            ConcurrentDictionary<int, TradeCapture> tradesByTradeId,
            Dictionary<string, List<int>> tradesByTradeKey,
            TradeCapture trade);
    }
}