using Mandara.Entities.Trades;

namespace Mandara.Business.DataInterface
{
    public interface IFxTradesDataLoader
    {
        /// <summary>
        /// Read up to 100 FX trades with IDs greater than the given FX trade ID and all trade changes with IDs
        /// greater than the given FX trade change ID.
        /// </summary>
        /// <param name="lastFxTradeIdRead"></param>
        /// <param name="lastTradeChangeIdRead"></param>
        /// <param name="maxFxTradesReadPackageSize"></param>
        /// <returns></returns>
        FxTradesData ReadLastFxTrades(
            int lastFxTradeIdRead,
            int lastTradeChangeIdRead,
            int maxFxTradesReadPackageSize = 100);
    }
}