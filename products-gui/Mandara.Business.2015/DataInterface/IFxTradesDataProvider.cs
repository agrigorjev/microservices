using Mandara.Entities;
using Mandara.Entities.Trades;
using Mandara.Extensions.Option;
using System;
using System.Collections.Generic;

namespace Mandara.Business.DataInterface
{
    /// <summary>
    /// FX trades need to be filtered in a range of common ways - trades for today (live trades), trades up to a given
    /// date (historic), trades for a particular currency, trades executed between two dates and current trades and
    /// trade changes.  This interface provides methods exposing these common behaviours.
    /// </summary>
    public interface IFxTradesDataProvider
    {
        /// <summary>
        /// Read up to a given number of FX trades (defaulting to 100) with IDs greater than the given FX trade ID and 
        /// all trade changes with IDs greater than the given FX trade change ID.
        /// </summary>
        /// <param name="lastFxTradeIdRead"></param>
        /// <param name="lastTradeChangeIdRead"></param>
        /// <param name="skippedFxTradesIds"></param>
        /// <param name="maxFxTradesReadPackageSize"></param>
        /// <returns></returns>
        FxTradesData ReadLastFxTrades(
            int lastFxTradeIdRead,
            int lastTradeChangeIdRead,
            List<int> skippedFxTradesIds,
            int maxFxTradesReadPackageSize = 100);
        List<FxTrade> ReadFilledFxTradesForCurrency(int currencyId);
        List<FxTrade> ReadFxTradesForPeriod(DateTime startDate, DateTime endDate);
        List<FxTrade> ReadFilledSpotFxTradesForPeriod(DateTime startDate, DateTime endDate);
        /// <summary>
        /// Read all FX trades that are marked as filled, that were for the given currency and were executed on or 
        /// before the given date.
        /// </summary>
        /// <param name="currencyId"></param>
        /// <param name="riskDate"></param>
        /// <returns></returns>
        List<FxTrade> ReadFilledFxTradesHistoric(int currencyId, DateTime riskDate);
        TryGetResult<FxTrade> ReadFxTradeByTradeId(MandaraEntities dbContext, int tradeId);
        List<FxTrade> ReadFxTradesByPortfolio(int currencyId, int portfolioId);
        IEnumerable<FxTrade> GetLiveFxTrades();
    }
}
