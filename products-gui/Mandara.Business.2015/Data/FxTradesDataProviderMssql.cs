using Mandara.Business.Contracts;
using Mandara.Business.DataInterface;
using Mandara.Business.Dates;
using Mandara.Business.Services;
using Mandara.Date;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Entities.Enums;
using Mandara.Entities.Trades;
using Mandara.Extensions.Option;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Mandara.Extensions.Collections;

namespace Mandara.Business.Data
{
    public class FxTradesDataProviderMssql : IFxTradesDataProvider
    {
        private readonly IEndOfDayDateTimeProvider _endOfDayDateTimeProvider;
        private readonly IProductsStorage _productsStore;
        private ILogger _log = new NLogLoggerFactory().GetCurrentClassLogger();

        public FxTradesDataProviderMssql(IEndOfDayDateTimeProvider endOfDayDateTimeProvider, IProductsStorage prodStore)
        {
            _endOfDayDateTimeProvider = endOfDayDateTimeProvider;
            _productsStore = prodStore;
        }

        public FxTradesData ReadLastFxTrades(
            int lastFxTradeIdRead,
            int lastTradeChangeIdRead,
            List<int> skippedFxTradesIds,
            int maxFxTradesReadPackageSize = 100)
        {
            using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
            {
                dbContext.Database.CommandTimeout = 0;

                List<FxTrade> fxTrades = SetProducts(
                    ReadFXTradesFromLastFXTradeRead(
                        lastFxTradeIdRead,
                        skippedFxTradesIds,
                        maxFxTradesReadPackageSize,
                        dbContext),
                    dbContext);
                List<FxTradeChange> fxTradeChanges = ReadFXTradeChanges(lastTradeChangeIdRead, dbContext, fxTrades);

                _log.Trace("FxTradesDataProviderMssql::ReadLastFxTrades: read {0} FX Trades", fxTrades.Count);
                return new FxTradesData(fxTrades, fxTradeChanges);
            }
        }

        private static MandaraEntities CreateMandaraProductsDbContext()
        {
            return new MandaraEntities(MandaraEntities.DefaultConnStrName, nameof(FxTradesDataProviderMssql));
        }

        private List<FxTrade> SetProducts(List<FxTrade> trades, MandaraEntities productsContext)
        {
            return trades.ForEachFluent(trade => SetProducts(trade, productsContext)).ToList();
        }

        private FxTrade SetProducts(FxTrade trade, MandaraEntities productsContext)
        {
            SetProduct(trade.ProductId, (product) => trade.Product = product, productsContext);
            SetProduct(
                trade.TradeCapture.SecurityDefinition.MandaraProductId,
                (product) => trade.TradeCapture.SecurityDefinition.Product = product, productsContext);

            return trade;
        }

        private void SetProduct(int productId, Action<Product> setProdOnEntity, MandaraEntities productsContext)
        {
            TryGetResult<Product> product = _productsStore.TryGetProduct(productId, productsContext);

            if (product.HasValue)
            {
                setProdOnEntity(product.Value);
            }
        }

        private List<FxTrade> ReadFXTradesFromLastFXTradeRead(
            int lastFxTradeIdRead,
            List<int> skippedFxTradesIds,
            int maxFxTradesReadPackageSize,
            MandaraEntities dbContext)
        {
            IQueryable<FxTrade> fxTradesLoad = GetFxQuery(dbContext);
            List<FxTrade> trades =
                fxTradesLoad.Where(
                    fxTrade => fxTrade.FxTradeId > lastFxTradeIdRead || skippedFxTradesIds.Contains(fxTrade.FxTradeId))
                            .OrderBy(fxTrade => fxTrade.FxTradeId)
                            .Take(maxFxTradesReadPackageSize)
                            .ToList();

            return SetProducts(trades, dbContext);
        }

        private static IQueryable<FxTrade> GetFxQuery(MandaraEntities context)
        {
            IQueryable<FxTrade> query =
                context.FxTrades.Include(fxTrade => fxTrade.AgainstCurrency)
                       .Include(fxTrade => fxTrade.SpecifiedCurrency)
                       .Include(fxTrade => fxTrade.TradeCapture)
                       .Include(fxTrade => fxTrade.TradeCapture.SecurityDefinition)
                       .Include(fxTrade => fxTrade.TradeCapture.SecurityDefinition.Product)
                       .Include(fxTrade => fxTrade.TradeCapture.SecurityDefinition.Product.Exchange)
                       .Include(fxTrade => fxTrade.Product)
                       .Include(fxTrade => fxTrade.Product.Exchange)
                       .Include(fxTrade => fxTrade.TradeCapture.Portfolio);

            return query;
        }

        private List<FxTradeChange> ReadFXTradeChanges(
            int lastTradeChangeIdRead,
            MandaraEntities dbContext,
            List<FxTrade> fxTrades)
        {
            List<TradeChange> tradeChanges = ReadTradeChangesForFXTrades(lastTradeChangeIdRead, dbContext);
            List<FxTrade> changedFxTrades = ReadChangedFXTrades(dbContext, fxTrades, tradeChanges);

            return BuildFXTradeChangesCollection(tradeChanges, changedFxTrades);
        }

        private List<TradeChange> ReadTradeChangesForFXTrades(int lastTradeChangeIdRead, MandaraEntities dbContext)
        {
            int fxEntityType = (int)TradeChangeEntityType.FxTrade;

            return
                dbContext.TradeChanges.Include(tradeChg => tradeChg.TradeCapture)
                   .Include(tradeChg => tradeChg.TradeCapture.Portfolio)
                   .Include(tradeChg => tradeChg.TradeCapture.SecurityDefinition)
                   .Include(tradeChg => tradeChg.TradeCapture.SecurityDefinition.Product)
                   .Where(
                       tradeChg => tradeChg.ChangeId > lastTradeChangeIdRead && tradeChg.EntityTypeDb == fxEntityType)
                   .ToList();
        }

        private List<FxTrade> ReadChangedFXTrades(
            MandaraEntities dbContext,
            List<FxTrade> fxTrades,
            List<TradeChange> tradeChanges)
        {
            List<int> tradeIdsRead = fxTrades.Select(x => x.TradeCaptureId).ToList();
            List<int> changedTradesIds = tradeChanges.Select(tradeChg => tradeChg.TradeId).ToList();
            List<FxTrade> changedFxTrades =
                fxTrades.Where(fxTrade => changedTradesIds.Contains(fxTrade.TradeCaptureId)).ToList();
            List<int> tradeIdsToRead = changedTradesIds.Except(tradeIdsRead).ToList();

            if (tradeIdsToRead.Count > 0)
            {
                List<FxTrade> fxTradesByTradeId =
                    GetFxQuery(dbContext)
                                         .Where(fxTrade => tradeIdsToRead.Contains(fxTrade.TradeCaptureId))
                                         .OrderBy(fxTrade => fxTrade.FxTradeId)
                                         .ToList();

                changedFxTrades.AddRange(SetProducts(fxTradesByTradeId, dbContext));
            }

            return changedFxTrades;
        }

        private static List<FxTradeChange> BuildFXTradeChangesCollection(
            List<TradeChange> tradeChanges,
            List<FxTrade> changedFxTrades)
        {
            return
                tradeChanges.Select(
                    tradeChg =>
                        new FxTradeChange
                        {
                            FxTrade = changedFxTrades.FirstOrDefault(fx => fx.TradeCaptureId == tradeChg.TradeId),
                            TradeCapture = tradeChg.TradeCapture,
                            TradeId = tradeChg.TradeId,
                            ChangeId = tradeChg.ChangeId,
                            ChangeDate = tradeChg.ChangeDate,
                            FromPortfolioId = tradeChg.FromPortfolioId,
                            OldQuantity = tradeChg.OldQuantity,
                            TradeChangeTypeDb = tradeChg.TradeChangeTypeDb
                        }).ToList();
        }

        public virtual List<FxTrade> ReadFilledFxTradesForCurrency(int currencyId)
        {
            using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
            {
                dbContext.Database.CommandTimeout = 0;

                List<FxTrade> trades =
                    FxTradesQueryProvider.FilterForFilledTradesOnly(GetFxQuery(dbContext))
                                         .Where(
                                             fxTrade =>
                                                 currencyId == fxTrade.AgainstCurrencyId
                                                 || currencyId == fxTrade.SpecifiedCurrencyId)
                                         .ToList();

                return SetProducts(trades, dbContext);
            }
        }

        public virtual List<FxTrade> ReadFxTradesByPortfolio(int currencyId, int portfolioId)
        {
            using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
            {
                dbContext.Database.CommandTimeout = 0;

                List<FxTrade> trades =
                    GetFxQuery(dbContext)
                                         .Where(
                                             fx =>
                                                 fx.TradeCapture.PortfolioId == portfolioId
                                                 && (currencyId == fx.AgainstCurrencyId
                                                     || currencyId == fx.SpecifiedCurrencyId))
                                         .ToList();

                return SetProducts(trades, dbContext);
            }
        }


        public List<FxTrade> ReadFxTradesForPeriod(DateTime startDate, DateTime endDate)
        {
            DateTime endOfEndDate = endDate.GetBeginningOfNextDay();
            DateRange dateRange = new DateRange(startDate, endOfEndDate);

            using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
            {
                dbContext.Database.CommandTimeout = 0;

                IQueryable<FxTrade> fxTradesLoad = GetFxQuery(dbContext);
                List<FxTrade> trades =
                    FxTradesQueryProvider.ApplyDateRangeConstraints(dateRange, _endOfDayDateTimeProvider, fxTradesLoad)
                                         .AsNoTracking()
                                         .ToList();

                return SetProducts(trades, dbContext);
            }
        }

        public virtual List<FxTrade> ReadFilledFxTradesHistoric(int currencyId, DateTime riskDate)
        {
            DateRange dateRange = new DateRange(DateRange.DefaultStart, riskDate);

            using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
            {
                dbContext.Database.CommandTimeout = 0;

                IQueryable<FxTrade> fxTradesLoad =
                    FxTradesQueryProvider.FilterForFilledTradesOnly(GetFxQuery(dbContext));
                List<FxTrade> trades =
                    FxTradesQueryProvider.ApplyDateRangeConstraints(dateRange, _endOfDayDateTimeProvider, fxTradesLoad)
                                         .Where(
                                             fxTrade =>
                                                 currencyId == fxTrade.AgainstCurrencyId
                                                 || currencyId == fxTrade.SpecifiedCurrencyId)
                                         .AsNoTracking()
                                         .ToList();

                return SetProducts(trades, dbContext);
            }
        }

        public List<FxTrade> ReadFilledSpotFxTradesForPeriod(DateTime startDate, DateTime endDate)
        {
            DateTime endOfEndDate = endDate.GetBeginningOfNextDay();

            using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
            {
                dbContext.Database.CommandTimeout = 0;

                IQueryable<FxTrade> fxTradesLoad = GetFxQuery(dbContext);

                fxTradesLoad = FxTradesQueryProvider.ApplyDateRangeConstraints(
                    new DateRange(startDate, endOfEndDate),
                    _endOfDayDateTimeProvider,
                    fxTradesLoad);

                List<FxTrade> trades = FxTradesQueryProvider
                    .FilterForFilledTradesOnly(fxTradesLoad)
                    .Where(fxTrade => fxTrade.ProductType == FxProductTypes.Spot)
                    .AsNoTracking()
                    .ToList();

                return SetProducts(trades, dbContext); ;
            }
        }

        public void SetProduct(FxTrade trade, MandaraEntities dbContext)
        {
            TryGetResult<Product> product = _productsStore.TryGetProduct(trade.ProductId);

            if (product.HasValue)
            {
                dbContext.Products.Attach(product.Value);
                trade.Product = product.Value;
            }
        }

        public TryGetResult<FxTrade> ReadFxTradeByTradeId(MandaraEntities dbContext, int tradeId)
        {
            _log.Trace("FxTradesDataProviderMssql::ReadFxTradeByTradeId: reading FX Trade");

            FxTrade trade = dbContext.FxTrades.SingleOrDefault(fxTrade => fxTrade.TradeCaptureId == tradeId);
            SetProduct(trade, dbContext);

            return new TryGetRef<FxTrade>() { Value = trade };
        }

        public IEnumerable<FxTrade> GetLiveFxTrades()
        {
            DateRange dateRange = new DateRange(SystemTime.Today(), SystemTime.Today().AddDays(1));
            List<FxTrade> result;

            using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
            {
                dbContext.Database.CommandTimeout = 0;

                IQueryable<FxTrade> query = GetFxQuery(dbContext);

                query = FxTradesQueryProvider.ApplyDateRangeConstraints(dateRange, _endOfDayDateTimeProvider, query);
                result = SetProducts(query.AsNoTracking().ToList(), dbContext);
            }

            _log.Trace("FxTradesDataProviderMssql::GetLiveFxTrades: read {0} live FX Trades", result.Count);
            return result;
        }
    }
}

