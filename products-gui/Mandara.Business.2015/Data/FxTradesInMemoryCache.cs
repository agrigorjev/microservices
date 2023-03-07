using Mandara.Business.DataInterface;
using Mandara.Business.Dates;
using Mandara.Business.Services;
using Mandara.Date;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Entities.Enums;
using Mandara.Entities.Trades;
using Mandara.Extensions.Option;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Mandara.Extensions.Collections;

namespace Mandara.Business.Data
{
    /// <summary>
    /// Reads FX Trades from <see cref="IEntityCache"/> for all methods except
    /// <see cref="IFxTradesDataProvider.ReadLastFxTrades"/>.
    /// </summary>
    public class FxTradesInMemoryCache : IFxTradesCache
    {
        private readonly IEndOfDayDateTimeProvider _endOfDayDateTimeProvider;
        private readonly ConcurrentDictionary<int, FxTrade> _fxTrades;

        public FxTradesInMemoryCache(IEndOfDayDateTimeProvider endOfDayDateTimeProvider)
        {
            _endOfDayDateTimeProvider = endOfDayDateTimeProvider;
            _fxTrades = new ConcurrentDictionary<int, FxTrade>();
        }

        public void AddOrUpdateEntities(IEnumerable<FxTrade> fxTrades)
        {
            fxTrades.ForEach(AddOrUpdateEntity);
        }

        public void AddOrUpdateEntity(FxTrade fxTrade)
        {
            SetTradeGroup(fxTrade);

            _fxTrades.AddOrUpdate(fxTrade.FxTradeId, id => fxTrade, (id, existingFxTrade) => fxTrade);
        }

        private void SetTradeGroup(FxTrade fxTrade)
        {
            int? tradeGroupId = fxTrade.TradeCapture?.GroupId;

            if (tradeGroupId.HasValue)
            {
                fxTrade.TradeCapture.TradeGroup = new TradeGroup { GroupId = tradeGroupId.Value };
            }
        }

        public TryGetResult<FxTrade> TryGetById(int fxTradeId)
        {
            FxTrade fxTrade;
            bool success = _fxTrades.TryGetValue(fxTradeId, out fxTrade);

            return new TryGetRef<FxTrade>(val => !success) { Value = fxTrade };
        }

        public FxTradesData ReadLastFxTrades(
            int lastFxTradeIdRead,
            int lastTradeChangeIdRead,
            List<int> skippedFxTradesIds,
            int maxFxTradesReadPackageSize = 100)
        {
            List<FxTrade> fxTradesAfterId =
                GetQueryableTrades()
                    .Where(fxTrade => fxTrade.FxTradeId > lastFxTradeIdRead)
                    .Take(maxFxTradesReadPackageSize)
                    .ToList();

            return new FxTradesData(fxTradesAfterId, new List<FxTradeChange>());
        }

        public IEnumerable<FxTrade> GetLiveFxTrades()
        {
            IEnumerable<FxTrade> fxTradesQuery =
                FxTradesQueryProvider.ApplyDateRangeConstraints(
                    new DateRange(SystemTime.Today(), SystemTime.Today().AddDays(1)),
                    _endOfDayDateTimeProvider,
                    GetQueryableTrades());

            return fxTradesQuery.ToList();
        }

        private IQueryable<FxTrade> GetQueryableTrades()
        {
            return _fxTrades.Values.AsQueryable();
        }

        public List<FxTrade> ReadFilledFxTradesForCurrency(int currencyId)
        {
            IQueryable<FxTrade> allFxTradesForCurrency =
                _fxTrades.Values.Where(
                    fxTrade => currencyId == fxTrade.AgainstCurrencyId || currencyId == fxTrade.SpecifiedCurrencyId)
                         .AsQueryable();

            return FxTradesQueryProvider.FilterForFilledTradesOnly(allFxTradesForCurrency).ToList();
        }

        public List<FxTrade> ReadFxTradesForPeriod(DateTime startDate, DateTime endDate)
        {
            DateTime endOfEndDate = endDate.GetBeginningOfNextDay();
            DateRange dateRange = new DateRange(startDate, endOfEndDate);

            IEnumerable<FxTrade> fxTrades = FxTradesQueryProvider.ApplyDateRangeConstraints(
                dateRange,
                _endOfDayDateTimeProvider,
                GetQueryableTrades());

            return fxTrades.ToList();
        }

        public List<FxTrade> ReadFilledSpotFxTradesForPeriod(DateTime startDate, DateTime endDate)
        {
            IQueryable<FxTrade> filledFxTradesPeriodQuery = GetFilledFxTradesForPeriodQuery(startDate, endDate);

            return filledFxTradesPeriodQuery.Where(fxTrade => fxTrade.ProductType == FxProductTypes.Spot).ToList();
        }

        private IQueryable<FxTrade> GetFilledFxTradesForPeriodQuery(DateTime startDate, DateTime endDate)
        {
            IQueryable<FxTrade> fxTradesPeriodQuery =
                FxTradesQueryProvider.ApplyDateRangeConstraints(
                    new DateRange(startDate, endDate),
                    _endOfDayDateTimeProvider,
                    GetQueryableTrades());

            return FxTradesQueryProvider.FilterForFilledTradesOnly(fxTradesPeriodQuery);
        }

        public List<FxTrade> ReadFilledFxTradesHistoric(int currencyId, DateTime riskDate)
        {
            return
                GetFilledFxTradesForPeriodQuery(DateRange.DefaultStart, riskDate)
                    .Where(
                        fxTrade => currencyId == fxTrade.AgainstCurrencyId || currencyId == fxTrade.SpecifiedCurrencyId)
                    .ToList();
        }

        public TryGetResult<FxTrade> ReadFxTradeByTradeId(MandaraEntities dbContext, int tradeId)
        {
            return new TryGetRef<FxTrade>()
            {
                Value = GetQueryableTrades().SingleOrDefault(fxTrade => fxTrade.TradeCaptureId == tradeId)
            };
        }

        public List<FxTrade> ReadFxTradesByPortfolio(int currencyId, int portfolioId)
        {
            return
                GetQueryableTrades()
                    .Where(
                        fx =>
                            fx.TradeCapture.PortfolioId == portfolioId
                            && (currencyId == fx.AgainstCurrencyId || currencyId == fx.SpecifiedCurrencyId))
                    .ToList();
        }
    }
}