using Mandara.Business.DataInterface;
using Mandara.Entities;
using Mandara.Entities.Trades;
using Mandara.Extensions.Option;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.Data
{
    public class FxTradesDataProvider : IFxTradesDataProvider
    {
        private readonly IFxTradesCache _fxTradesCache;
        private readonly IFxTradesDataProvider _fxTradesLoader;
        private readonly HashSet<int> _hasReadFilledTradesForCurrency = new HashSet<int>();
        private readonly Dictionary<int, DateTime> _mostRecentHistoricalDateByCurrency = new Dictionary<int, DateTime>();
        private readonly HashSet<int> _hasReadForPortfolio = new HashSet<int>();
        private bool _hasLoadedLiveTrades = false;

        public FxTradesDataProvider(IFxTradesCache cache, [Named("Loader")]IFxTradesDataProvider loader)
        {
            _fxTradesCache = cache;
            _fxTradesLoader = loader;
        }

        public FxTradesData ReadLastFxTrades(
            int lastFxTradeIdRead,
            int lastTradeChangeIdRead,
            List<int> skippedFxTradesIds,
            int maxFxTradesReadPackageSize = 100)
        {
            FxTradesData tradesAndChanges = _fxTradesLoader.ReadLastFxTrades(
                lastFxTradeIdRead,
                lastTradeChangeIdRead,
                skippedFxTradesIds,
                maxFxTradesReadPackageSize);

            _fxTradesCache.AddOrUpdateEntities(tradesAndChanges.Trades);
            return tradesAndChanges;
        }

        public List<FxTrade> ReadFilledFxTradesForCurrency(int currencyId)
        {
            List<FxTrade> tradesForCurrency =
                ReadFxTradesByIdentifier(
                    _hasReadFilledTradesForCurrency.Contains(currencyId),
                    () => _fxTradesLoader.ReadFilledFxTradesForCurrency(currencyId),
                    () => _fxTradesCache.ReadFilledFxTradesForCurrency(currencyId));

            _hasReadFilledTradesForCurrency.Add(currencyId);
            return tradesForCurrency;
        }

        public List<FxTrade> ReadFxTradesByIdentifier(
            bool isIdentifierAlreadySeen,
            Func<List<FxTrade>> tradesLoad,
            Func<List<FxTrade>> tradesInMemoryRead)
        {
            List<FxTrade> tradesForIdentifier;

            if (!isIdentifierAlreadySeen)
            {
                tradesForIdentifier = tradesLoad();
                _fxTradesCache.AddOrUpdateEntities(tradesForIdentifier);
            }
            else
            {
                tradesForIdentifier = tradesInMemoryRead();
            }

            return tradesForIdentifier;
        }

        public List<FxTrade> ReadFxTradesForPeriod(DateTime startDate, DateTime endDate)
        {
            return _fxTradesLoader.ReadFxTradesForPeriod(startDate, endDate);
        }

        public List<FxTrade> ReadFilledSpotFxTradesForPeriod(DateTime startDate, DateTime endDate)
        {
            return _fxTradesLoader.ReadFilledSpotFxTradesForPeriod(startDate, endDate);
        }

        public List<FxTrade> ReadFilledFxTradesHistoric(int currencyId, DateTime riskDate)
        {
            DateTime mostRecentRiskDateForCurrency;
            List<FxTrade> historicalTrades;

            if (!_mostRecentHistoricalDateByCurrency.TryGetValue(currencyId, out mostRecentRiskDateForCurrency))
            {
                historicalTrades = ReadFilledHistoricTradesFromStore(currencyId, riskDate);
            }
            else if (riskDate > mostRecentRiskDateForCurrency)
            {
                historicalTrades = ReadFilledHistoricTradesFromCacheAndStore(
                    currencyId,
                    riskDate,
                    mostRecentRiskDateForCurrency);
            }
            else
            {
                historicalTrades = _fxTradesCache.ReadFilledFxTradesHistoric(currencyId, riskDate);
            }

            return historicalTrades;
        }

        private List<FxTrade> ReadFilledHistoricTradesFromStore(int currencyId, DateTime riskDate)
        {
            List<FxTrade> historicalTrades;
            historicalTrades = _fxTradesLoader.ReadFilledFxTradesHistoric(currencyId, riskDate);
            _mostRecentHistoricalDateByCurrency[currencyId] = riskDate;
            _fxTradesCache.AddOrUpdateEntities(historicalTrades);
            return historicalTrades;
        }

        private List<FxTrade> ReadFilledHistoricTradesFromCacheAndStore(
            int currencyId,
            DateTime riskDate,
            DateTime mostRecentRiskDateForCurrency)
        {
            List<FxTrade> tradesFromGap = _fxTradesLoader.ReadFxTradesForPeriod(mostRecentRiskDateForCurrency, riskDate);
            List<FxTrade> historicalTradesInMemory = _fxTradesCache.ReadFilledFxTradesHistoric(
                currencyId,
                mostRecentRiskDateForCurrency);

            _fxTradesCache.AddOrUpdateEntities(tradesFromGap);
            return tradesFromGap.Concat(historicalTradesInMemory).ToList();
        }

        public TryGetResult<FxTrade> ReadFxTradeByTradeId(MandaraEntities dbContext, int tradeId)
        {
            TryGetResult<FxTrade> trade = _fxTradesCache.ReadFxTradeByTradeId(dbContext, tradeId);

            if (!trade.HasValue)
            {
                trade = _fxTradesLoader.ReadFxTradeByTradeId(dbContext, tradeId);

                if (trade.HasValue)
                {
                    _fxTradesCache.AddOrUpdateEntity(trade.Value);
                }
            }

            return trade;
        }

        public List<FxTrade> ReadFxTradesByPortfolio(int currencyId, int portfolioId)
        {
            List<FxTrade> tradesForIdentifier = ReadFxTradesByIdentifier(
                _hasReadForPortfolio.Contains(portfolioId),
                () => _fxTradesLoader.ReadFxTradesByPortfolio(currencyId, portfolioId),
                () => _fxTradesCache.ReadFxTradesByPortfolio(currencyId, portfolioId));

            _hasReadForPortfolio.Add(portfolioId);
            return tradesForIdentifier;
        }

        public IEnumerable<FxTrade> GetLiveFxTrades()
        {
            List<FxTrade> trades = ReadFxTradesByIdentifier(
                _hasLoadedLiveTrades,
                () => _fxTradesLoader.GetLiveFxTrades().ToList(),
                () => _fxTradesCache.GetLiveFxTrades().ToList());

            _hasLoadedLiveTrades = true;
            return trades;
        }
    }
}
