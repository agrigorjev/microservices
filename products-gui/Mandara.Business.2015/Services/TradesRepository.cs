using AutoMapper.QueryableExtensions;
using EntityFramework.Future;
using Mandara.Business.Audit;
using Mandara.Business.Bus;
using Mandara.Business.Calculators;
using Mandara.Business.Contracts;
using Mandara.Business.Data;
using Mandara.Business.DataInterface;
using Mandara.Business.Dates;
using Mandara.Business.Extensions;
using Mandara.Business.Managers;
using Mandara.Business.Model;
using Mandara.Business.TradeAdd;
using Mandara.Business.Trades;
using Mandara.Data;
using Mandara.Date;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Entities.EntityPieces;
using Mandara.Entities.Enums;
using Mandara.Entities.ErrorReporting;
using Mandara.Extensions.Option;
using Ninject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mandara.Extensions.Collections;
using Mandara.Extensions.Guids;
using Mandara.Extensions.Nullable;
using Mandara.Proto.Encoder;
using Ninject.Extensions.Logging;

namespace Mandara.Business.Services
{
    public class TradesRepository : ReplaceableDbContext, ITradesRepository
    {
        private readonly ILogger _logger;
        private readonly IAuditService _auditService;
        private readonly PrecalcPositionsCalculator _precalcPositionsCalculator;
        private readonly IProductsStorage _productsStorage;
        private readonly ISecurityDefinitionsStorage _securityDefinitionsStorage;
        private Dictionary<string, string> _execIdDictionary;
        private readonly ITradeAddHandlerConverter _tradeAddHandlerConverter;
        private TradeBrokerageCalculationManager _tradeBrokerageCalculationManager;
        private readonly IFxTradesDataProvider _fxTradesDataProvider;
        private readonly IEndOfDayDateTimeProvider _endOfDayDateTimes;

        private static readonly Type[] PrecalcSaveModifiableTypes =
        {
            typeof(PrecalcSdDetail),
            typeof(PrecalcTcDetail),
            typeof(AuditMessage)
        };
        private static readonly Type[] TradeSaveModifiableTypes =
        {
            typeof(SecurityDefinition),
            typeof(TradeCapture),
            typeof(FxTrade),
            typeof(TradeChange),
            typeof(PrecalcSdDetail),
            typeof(PrecalcTcDetail),
            typeof(AuditMessage)
        };


        public TradesRepository(
            ISecurityDefinitionsStorage securityDefinitionsStorage,
            IProductsStorage productsStorage,
            PrecalcPositionsCalculator precalcPositionsCalculator,
            ITradeAddHandlerConverter tradeAddHandlerConverter,
            IAuditService auditService,
            [Named("Loader")]IFxTradesDataProvider fxTradesDataProv,
            IEndOfDayDateTimeProvider eodDateTimes,
            ILogger logger)
        {
            _securityDefinitionsStorage = securityDefinitionsStorage;
            _productsStorage = productsStorage;
            _precalcPositionsCalculator = precalcPositionsCalculator;
            _auditService = auditService;
            _tradeAddHandlerConverter = tradeAddHandlerConverter;
            _fxTradesDataProvider = fxTradesDataProv;
            _endOfDayDateTimes = eodDateTimes;

            _execIdDictionary = new Dictionary<string, string>();
            _logger = logger;
        }


        public TradesQuantityData ReadQuantities(TradeReadFilterParameters filterParams)
        {
            List<SdQuantityModel> sdQuantities = null;
            List<SdQuantityModel> sdLegsQuantities = null;
            List<TradeModel> trades = null;
            int latestTradeId = 0;
            int latestSdId = 0;
            int latestTradeChangeId = 0;

            DateTime today = (filterParams.RiskDate ?? SystemTime.Today()).Date;
            List<TradeModel> todayTasTrades;

            using (MandaraEntities dbContext = _dbContextCreator())
            {
                dbContext.Database.CommandTimeout = 0;

                IQueryable<TradeCapture> sdQuery = TradesQueryProvider.GetSecDefBasedParentTradesQuery(
                    dbContext,
                    new DateRange(filterParams.FromTransactTime, filterParams.TillTransactTime),
                    _endOfDayDateTimes,
                    _productsStorage);

                // TODO: Separate loaded data into single currency and multi-currency.  This is likely to require
                // fitlering after loading.  So the conversion to SdQuantityModel collection is likely to have to be
                // executed on the loaded data.  Speed will probably remain much the same, but the memory footprint will
                // be affected.
                FutureQuery<SdQuantityModel> futureSdQuantities =
                    sdQuery.GroupBy(x => new { x.SecurityDefinitionId, x.PortfolioId })
                           .AsNoTracking()
                           .Select(
                               x =>
                                   new SdQuantityModel
                                   {
                                       SecurityDefinitionId = x.Key.SecurityDefinitionId,
                                       PortfolioId = x.Key.PortfolioId ?? 0,
                                       TradesQuantity = x.Sum(tc => tc.Quantity ?? 0),
                                       TradesFxExposure = x.Sum(tc => tc.Quantity * tc.Price) ?? 0,
                                   })
                           .OrderBy(sd => sd.SecurityDefinitionId)
                           .Future();

                IQueryable<TradeCapture> sdLegsQuery = TradesQueryProvider.GetSecDefBasedOnlyLegsTradesQuery(
                    dbContext,
                    _endOfDayDateTimes,
                    _productsStorage);

                FutureQuery<SdQuantityModel> futureSdLegsQuantities =
                    sdLegsQuery.GroupBy(x => new { x.SecurityDefinitionId, x.PortfolioId })
                               .AsNoTracking()
                               .Select(
                                   x =>
                                       new SdQuantityModel
                                       {
                                           SecurityDefinitionId = x.Key.SecurityDefinitionId,
                                           PortfolioId = x.Key.PortfolioId ?? 0,
                                           TradesQuantity = x.Sum(tc => tc.Quantity ?? 0),
                                           TradesFxExposure = x.Sum(tc => tc.Quantity * tc.Price) ?? 0,
                                       })
                               .OrderBy(sd => sd.SecurityDefinitionId)
                               .Future();

                IQueryable<TradeCapture> tradesQuery = TradesQueryProvider.GetCustomPeriodTradesQuery(
                    dbContext,
                    new DateRange(filterParams.FromTransactTime, filterParams.TillTransactTime),
                    _endOfDayDateTimes,
                    _productsStorage);

                FutureQuery<TradeModel> futureTrades =
                    tradesQuery.AsNoTracking().OrderBy(tc => tc.TradeId).ProjectTo<TradeModel>().Future();

                IQueryable<TradeCapture> tasQuery = TradesQueryProvider.GetTasTradesForDayQuery(
                    dbContext,
                    today,
                    filterParams.TillTransactTime,
                    _endOfDayDateTimes,
                    _productsStorage);
                FutureQuery<TradeModel> futureTodayTasTrades =
                    tasQuery.AsNoTracking().OrderBy(tc => tc.TradeId).ProjectTo<TradeModel>().Future();

                FutureQuery<TradeCapture> futureLatestTrade =
                    dbContext.TradeCaptures.OrderByDescending(tc => tc.TradeId).Take(1).Future();
                FutureQuery<SecurityDefinition> futureLatestSd =
                    dbContext.SecurityDefinitions.OrderByDescending(tc => tc.SecurityDefinitionId).Take(1).Future();
                FutureQuery<TradeChange> futureLatestTradeChange =
                    dbContext.TradeChanges.OrderByDescending(tc => tc.ChangeId).Take(1).Future();

                DbContextTransaction trx = null;
                try
                {
                    if (filterParams.SetTransaction)
                    {
                        trx = dbContext.Database.BeginTransaction(IsolationLevel.RepeatableRead);
                    }
                    TradeCapture latestTrade = futureLatestTrade.FirstOrDefault();
                    SecurityDefinition latestSd = futureLatestSd.FirstOrDefault();
                    TradeChange latestTradeChange = futureLatestTradeChange.FirstOrDefault();

                    latestTradeId = latestTrade?.TradeId ?? 0;
                    latestSdId = latestSd?.SecurityDefinitionId ?? 0;
                    latestTradeChangeId = latestTradeChange?.ChangeId ?? 0;

                    trades = futureTrades.ToList();
                    todayTasTrades = futureTodayTasTrades.ToList();

                    List<SdQuantityModel> sdQuantitiesData = futureSdQuantities.ToList();

                    sdQuantities = sdQuantitiesData.ToList();

                    if (filterParams.ReadTimeSpreadLegs)
                    {
                        List<SdQuantityModel> sdLegsQuantitiesData = futureSdLegsQuantities.ToList();

                        sdLegsQuantities = sdLegsQuantitiesData.ToList();
                    }
                }
                finally
                {
                    if (filterParams.SetTransaction)
                    {
                        trx?.Commit();
                    }
                }
            }

            Action<TradeModel> tasTradesFilterAction = tasTrade =>
            {
                SdQuantityModel sdQuantity =
                    sdQuantities.FirstOrDefault(
                       x =>
                           x.PortfolioId == tasTrade.PortfolioId
                           && x.SecurityDefinitionId == tasTrade.SecurityDefinitionId);
                if (sdQuantity != null)
                {
                    sdQuantity.TradesQuantity -= tasTrade.Quantity.Value;
                }
            };

            FilterOutTasTrades(filterParams.RiskDate, todayTasTrades, tasTradesFilterAction);


            return new TradesQuantityData(
                sdQuantities,
                sdLegsQuantities,
                trades,
                latestTradeId,
                latestSdId,
                latestTradeChangeId);
        }

        public List<TradeCapture> ReadParentTradeCaptures(
            Expression<Func<TradeCapture, bool>> portfoliosConstraint,
            List<int> sdIds,
            List<int> tradesIds,
            DateTime? tillDatetime,
            DateTime? fromDatetime)
        {
            DateTime today = (tillDatetime ?? SystemTime.Today()).Date;

            tillDatetime = tillDatetime?.Date.AddDays(1);

            List<TradeModel> todayTasTrades;
            List<TradeCapture> tradeCaptures;

            using (var dbContext = CreateMandaraProductsDbContext())
            {
                IQueryable<TradeCapture> baseQuery = GetBaseQueryForReadParentTradeCaptures(
                    portfoliosConstraint,
                    tillDatetime,
                    fromDatetime,
                    dbContext);

                FutureQuery<TradeCapture> futureTradesFromSds =
                    baseQuery.Where(trade => sdIds.Contains(trade.SecurityDefinitionId)).Future();

                FutureQuery<TradeCapture> futureTrades =
                    baseQuery.Include(x => x.PrecalcDetails).Where(trade => tradesIds.Contains(trade.TradeId)).Future();

                FutureQuery<TradeModel> futureTasTrades =
                    TradesQueryProvider.GetTasTradesForDayQuery(dbContext, today, tillDatetime, _endOfDayDateTimes, _productsStorage)
                                       .AsNoTracking()
                                       .OrderBy(tc => tc.TradeId)
                                       .ProjectTo<TradeModel>()
                                       .Future();

                tradeCaptures = futureTradesFromSds.ToList();
                tradeCaptures.AddRange(futureTrades.ToList());
                todayTasTrades = futureTasTrades.ToList();
            }

            Action<TradeModel> tasTradeFilterAction = GetRemoveTasTradesFilterAction(tradeCaptures);
            FilterOutTasTrades(tillDatetime, todayTasTrades, tasTradeFilterAction);

            return tradeCaptures;
        }

        private IQueryable<TradeCapture> GetBaseQueryForReadParentTradeCaptures(
            Expression<Func<TradeCapture, bool>> portfoliosConstraint,
            DateTime? tillDatetime,
            DateTime? fromDatetime,
            MandaraEntities dbContext)
        {
            DateRange dateRange = new DateRange(fromDatetime, tillDatetime);

            IQueryable<TradeCapture> baseQuery =
                dbContext.TradeCaptures
                   .Where(portfoliosConstraint)
                   .Where(tc => tc.OrdStatus == TradeOrderStatus.Filled && tc.IsParentTrade == true);
            baseQuery = TradesQueryProvider.ApplyDateRangeConstraints(
                dateRange,
                _endOfDayDateTimes,
                baseQuery,
                _productsStorage);

            return baseQuery;
        }

        private Action<TradeModel> GetRemoveTasTradesFilterAction(List<TradeCapture> tradeCaptures)
        {
            return tasTrade =>
            {
                TradeCapture trade =
                    tradeCaptures.FirstOrDefault(
                        x => x.PortfolioId == tasTrade.PortfolioId && x.TradeId == tasTrade.TradeId);

                if (trade != null)
                {
                    tradeCaptures.Remove(trade);
                }
            };
        }

        private void FilterOutTasTrades(
            DateTime? riskDate,
            List<TradeModel> todayTasTrades,
            Action<TradeModel> tasTradeFilterAction)
        {
            foreach (TradeModel tasTrade in todayTasTrades)
            {
                TryGetResult<SecurityDefinition> secDef =
                    _securityDefinitionsStorage.TryGetSecurityDefinition(tasTrade.SecurityDefinitionId);

                if (!secDef.HasValue)
                {
                    continue;
                }

                TryGetResult<Product> product = _productsStorage.TryGetProduct(secDef.Value.product_id.Value);

                if (!product.HasValue)
                {
                    continue;
                }

                DateTime tasActivationDate =
                    _productsStorage.GetTasActivationDate(product.Value, tasTrade.TradeDate.Value);

                if (tasActivationDate > product.Value.GetTasActivationTime(riskDate))
                {
                    tasTradeFilterAction(tasTrade);
                }
            }
        }

        public List<TradeCapture> GetTradesWithSameGroup(int groupId, string ordStatus = null)
        {
            List<TradeCapture> groupTrades;
            using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
            {
                IQueryable<TradeCapture> query =
                    dbContext.TradeCaptures.Include(x => x.Portfolio)
                       .Include(x => x.SellBook)
                       .Include(x => x.BuyBook)
                       .Include(x => x.TradeGroup)
                       .Include(x => x.SecurityDefinition)
                       .Include(x => x.SecurityDefinition.Product)
                       .Where(x => x.GroupId == groupId);

                if (ordStatus != null)
                {
                    query = query.Where(x => x.OrdStatus == ordStatus);
                }

                groupTrades = query.ToList();
            }

            return groupTrades;
        }

        public List<TradeCapture> GetFullSpreadTrades(TradeCapture spreadTrade)
        {
            List<TradeCapture> spreadGroup;

            using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
            {
                spreadGroup =
                    dbContext.TradeCaptures.Include(x => x.Portfolio)
                       .Include(x => x.SellBook)
                       .Include(x => x.BuyBook)
                       .Include(x => x.TradeGroup)
                       .Include(x => x.SecurityDefinition)
                       .Include(x => x.SecurityDefinition.Product)
                       .Where(
                           x =>
                               (x.ExecID == spreadTrade.ExecID) && (x.TradeDate == spreadTrade.TradeDate)
                               && ((x.ClOrdID == spreadTrade.ClOrdID) || (x.ClOrdID == null)))
                       .ToList();
            }

            return spreadGroup;
        }

        public TradeCapture GetTradeWithSdById(int tradeId)
        {
            using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
            {
                return
                    dbContext.TradeCaptures.Include(x => x.Portfolio)
                       .Include(x => x.SellBook)
                       .Include(x => x.BuyBook)
                       .Include(x => x.TradeGroup)
                       .Include(x => x.SecurityDefinition)
                       .Include(x => x.SecurityDefinition.Product)
                       .FirstOrDefault(x => x.TradeId == tradeId);
            }
        }

        public List<TradeCapture> GetTradesById(List<int> tradesIds)
        {
            if (tradesIds.Count == 0)
            {
                return new List<TradeCapture>();
            }

            using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
            {
                return
                    dbContext.TradeCaptures.Include(x => x.Portfolio)
                       .Include(x => x.SellBook)
                       .Include(x => x.BuyBook)
                       .Include(x => x.TradeGroup)
                       .Include(x => x.SecurityDefinition)
                       .Include(x => x.SecurityDefinition.Product)
                       .Where(x => tradesIds.Contains(x.TradeId))
                       .ToList();
            }
        }

        public void AssignTrades(
            AuditContext auditContext,
            int toPortfolioId,
            string toPortfolioName,
            Dictionary<string, Tuple<List<DateTime?>, List<string>>> execIdsOfTradesToAssign,
            string userName)
        {
            if (execIdsOfTradesToAssign == null)
            {
                return;
            }

            if (execIdsOfTradesToAssign.Count == 0)
            {
                return;
            }

            List<string> execIdsList = execIdsOfTradesToAssign.Keys.ToList();

            try
            {
                using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
                {
                    using (DbContextTransaction trx = dbContext.Database.BeginTransaction(IsolationLevel.RepeatableRead))
                    {
                        List<TradeCapture> tradeCapturesToAssign =
                            dbContext.TradeCaptures.Include(x => x.Portfolio)
                               .Include(x => x.SecurityDefinition)
                               .Include(x => x.SecurityDefinition.Product)
                               .Include(x => x.SecurityDefinition.Product.Exchange)
                               .Where(tc => execIdsList.Contains(tc.ExecID))
                               .ToList();

                        //now we need to remove from list trades, that from diffrent date then execID been sent
                        //because ExecID is unique just withhin a day, and we could get some trades from other date, 
                        //that have no connection to requested trades
                        for (int i = tradeCapturesToAssign.Count - 1; i >= 0; i--)
                        {
                            TradeCapture tradeCapture = tradeCapturesToAssign[i];
                            string execID = tradeCapture.ExecID;

                            if (
                                !(execIdsOfTradesToAssign.ContainsKey(execID)
                                  && ((List<DateTime?>)execIdsOfTradesToAssign[execID].Item1).Contains(
                                      tradeCapture.TradeDate)
                                  && ((List<string>)execIdsOfTradesToAssign[execID].Item2).Contains(
                                      tradeCapture.OrderID ?? "")))
                            {
                                tradeCapturesToAssign.RemoveAt(i);
                            }
                        }

                        Portfolio destinationPortfolio =
                            dbContext.Portfolios.FirstOrDefault(x => x.PortfolioId == toPortfolioId);

                        if (destinationPortfolio == null)
                        {
                            return;
                        }

                        DateTime timestamp = SystemTime.Now();
                        bool destinationPortfolioUpdated = false;

                        List<AuditMessage> auditMessages = _auditService.CreateAuditMessages(
                            auditContext,
                            "Modify",
                            tradeCapturesToAssign,
                            null);

                        foreach (TradeCapture tradeCapture in tradeCapturesToAssign)
                        {
                            // if trade is already in the destination portfolio we skip the assign, 
                            // it doesn't make sense and would lead to incorrect results
                            if (tradeCapture.Portfolio.PortfolioId == destinationPortfolio.PortfolioId)
                            {
                                continue;
                            }

                            TradeChange assign = TradeChange.Create(
                                tradeCapture,
                                timestamp,
                                TradeChangeType.Assigned,
                                SecurityDefinitionsManager.GetTradeChangeEntityType(tradeCapture.SecurityDefinitionId));

                            dbContext.TradeChanges.Add(assign);
                            tradeCapture.Portfolio = destinationPortfolio;
                            destinationPortfolioUpdated = true;
                        }

                        if (destinationPortfolioUpdated)
                        {
                            destinationPortfolio.UpdatedAt = timestamp;
                            destinationPortfolio.UpdatedBy = userName;
                        }

                        auditMessages = _auditService.UpdateAuditMessages(auditMessages, tradeCapturesToAssign);
                        dbContext.AuditMessages.AddRange(auditMessages);

                        dbContext.SaveChanges();

                        trx.Commit();
                    }
                }
            }
            catch (DbUpdateException)
            {
                Error error = new Error(
                    "Assign Trades",
                    ErrorType.Information,
                    "Trades cannot be assigned as they are already being assigned by another user. Please try again "
                    + "later.",
                    null,
                    null,
                    ErrorLevel.Normal);

                ErrorReportingHelper.GlobalQueue.Enqueue(error);
            }
        }

        private static MandaraEntities CreateMandaraProductsDbContext()
        {
            return new MandaraEntities(MandaraEntities.DefaultConnStrName, nameof(TradesRepository));
        }

        public List<int> CreateTrades(
            List<TradeCapture> tradeCaptures,
            List<FxTrade> fxTrades,
            AuditContext auditContext,
            TradeAddDetails tradeAddDetails)
        {
            if (tradeCaptures == null)
            {
                return new List<int>();
            }

            if (tradeAddDetails.TradeType == TradeTypeControl.Transfer)
            {
                return AddTransferTrades(
                    tradeAddDetails.Portfolio2.PortfolioId,
                    tradeAddDetails.Portfolio1.PortfolioId,
                    tradeCaptures,
                    tradeAddDetails.CreatedByUserName,
                    auditContext,
                    tradeAddDetails.GroupId,
                    tradeAddDetails.TradeCaptureIds,
                    tradeAddDetails.EditCancelReason,
                    tradeAddDetails);
            }
            else
            {
                InsertTradeCaptures(
                    tradeCaptures,
                    fxTrades,
                    auditContext,
                    tradeAddDetails.Portfolio1.PortfolioId,
                    tradeAddDetails.GroupId,
                    tradeAddDetails.TradeCaptureIds,
                    tradeAddDetails.EditCancelReason,
                    tradeAddDetails);

                return tradeCaptures.Select(x => x.TradeId).ToList();
            }
        }

        private class TransferTradePortfolios
        {
            public Portfolio BuyBook { get; }
            public Portfolio SellBook { get; }

            public TransferTradePortfolios(Portfolio buyBk, Portfolio sellBk)
            {
                BuyBook = buyBk;
                SellBook = sellBk;
            }

            public bool HasPortfolios()
            {
                return null != BuyBook && null != SellBook;
            }
        }

        public List<int> AddTransferTrades(
            int? buyBookId,
            int? sellBookId,
            List<TradeCapture> tradeCaptures,
            string userName,
            AuditContext auditContext,
            int? groupId,
            List<int> tradeIdsForCancel,
            string editCancelReason,
            TradeAddDetails tradeAddDetails = null)
        {
            List<int> insertedTradeIds = new List<int>();

            if (IsMissingPortfolio(buyBookId, sellBookId) || !HasTradeCaptures(tradeCaptures))
            {
                return insertedTradeIds;
            }

            List<TradeCapture> clonedTrades;

            using (MandaraEntities dbContext = CreateMandaraProductsDbContext())
            {
                using (DbContextTransaction trx = dbContext.Database.BeginTransaction(IsolationLevel.RepeatableRead))
                {
                    TransferTradePortfolios buyAndSellBooks = GetUpdatedBuyAndSellBooks(
                        buyBookId,
                        sellBookId,
                        userName,
                        dbContext);

                    if (!buyAndSellBooks.HasPortfolios())
                    {
                        return insertedTradeIds;
                    }

                    TradeGroup tradeGroup = GetTradeGroup(groupId ?? 0, dbContext);
                    bool editTradesMode = IsEditingTrades(tradeAddDetails, tradeIdsForCancel);
                    List<TradePieces> tradesToCancel = editTradesMode
                        ? GetTradesToCancel(tradeIdsForCancel, dbContext)
                        : new List<TradePieces>();

                    if (tradesToCancel.Any())
                    {
                        SetNewTradesTrader(tradeCaptures, tradesToCancel);
                    }

                    bool cancellationsCreated = editTradesMode
                        ? CancelTradeCaptures(
                                auditContext,
                                dbContext,
                                tradeIdsForCancel.ToHashSet(),
                                editCancelReason,
                                tradeGroup)
                            .cancellationsCreated
                        : CancelTradeCaptures(dbContext, tradeIdsForCancel.ToHashSet(), editCancelReason, tradeGroup)
                            .cancellationsCreated;

                    if (!cancellationsCreated)
                    {
                        // 1) no trades were cancelled - we should return at this point because we are trying
                        // to edit already cancelled trades amd that's not good
                        // 2) if tradeCapturesToCancel is empty we would _not_ be here cause it mean we are
                        // adding new trades
                        return insertedTradeIds;
                    }

                    tradeCaptures.ForEach(tc => tc.TradeGroup = tradeGroup);

                    // add transfer trades for the buy book
                    List<TradePieces> preparedTrades = AddNonFxTradesToContext(
                        dbContext,
                        tradeCaptures,
                        buyAndSellBooks.BuyBook,
                        buyAndSellBooks.SellBook,
                        buyAndSellBooks.BuyBook);

                    // add transfer trades for the sell book
                    clonedTrades = CloneTrades(tradeCaptures, tradeGroup).ToList();

                    preparedTrades.AddRange(AddNonFxTradesToContext(
                        dbContext,
                        clonedTrades,
                        buyAndSellBooks.SellBook,
                        buyAndSellBooks.SellBook,
                        buyAndSellBooks.BuyBook));
                    dbContext.SaveChanges();

                    List<TradeCapture> tradesForSave = preparedTrades.Select(trade => trade.Trade).ToList();

                    if (editTradesMode)
                    {
                        dbContext.AuditMessages.Add(
                            CreateTradeEditAuditMsg(
                                auditContext,
                                tradeAddDetails,
                                dbContext,
                                tradesToCancel,
                                tradesForSave,
                                buyAndSellBooks));
                    }
                    else
                    {
                        List<AuditMessage> auditMessages = _auditService.CreateAuditMessages(
                            auditContext,
                            "Create",
                            null,
                            tradesForSave);

                        dbContext.AuditMessages.AddRange(auditMessages);
                    }

                    CalculatePreCalcDetails(dbContext, preparedTrades);

                    dbContext.SaveChanges();
                    trx.Commit();

                    _productsStorage.SanitiseCircularReferences();
                }
            }

            insertedTradeIds.AddRange(tradeCaptures.Select(x => x.TradeId).ToArray());
            insertedTradeIds.AddRange(clonedTrades.Select(x => x.TradeId).ToArray());

            return insertedTradeIds;
        }

        private static void SetNewTradesTrader(List<TradeCapture> tradeCaptures, List<TradePieces> tradesToCancel)
        {
            string originationTrader = tradesToCancel.First().Trade.OriginationTrader;

            tradeCaptures.ForEach(trade => { trade.OriginationTrader = originationTrader; });
        }

        private static bool IsMissingPortfolio(int? buyBookId, int? sellBookId)
        {
            return (buyBookId == null) || (sellBookId == null);
        }

        private static bool HasTradeCaptures(List<TradeCapture> trades)
        {
            return (trades?.Any()).True();
        }

        private static TransferTradePortfolios GetUpdatedBuyAndSellBooks(
            int? buyBookId,
            int? sellBookId,
            string userName,
            MandaraEntities dbContext)
        {
            Portfolio buyPortfolio = dbContext.Portfolios.SingleOrDefault(p => p.PortfolioId == buyBookId.Value);
            Portfolio sellPortfolio = dbContext.Portfolios.SingleOrDefault(p => p.PortfolioId == sellBookId.Value);

            if ((buyPortfolio == null) || (sellPortfolio == null))
            {
                string error = "";

                error += buyPortfolio == null
                    ? string.Format("Source portfolio not found, portfolio id [{0}], ", buyBookId.Value)
                    : string.Empty;

                error += sellPortfolio == null
                    ? string.Format("Destination portfolio not found, portfolio id [{0}]", sellBookId.Value)
                    : string.Empty;
            }
            else
            {
                DateTime timestamp = SystemTime.Now();

                buyPortfolio.UpdatedAt = timestamp;
                buyPortfolio.UpdatedBy = userName;
                sellPortfolio.UpdatedAt = timestamp;
                sellPortfolio.UpdatedBy = userName;
            }

            return new TransferTradePortfolios(buyPortfolio, sellPortfolio);
        }

        private static TradeGroup GetTradeGroup(int groupId, MandaraEntities dbContext)
        {
            TradeGroup tradeGroup = dbContext.TradeGroups.SingleOrDefault(tg => tg.GroupId == groupId);

            if (tradeGroup == null)
            {
                tradeGroup = new TradeGroup();
                dbContext.TradeGroups.Add(tradeGroup);
                dbContext.SaveChanges();
            }

            return tradeGroup;
        }

        private static bool IsEditingTrades(TradeAddDetails tradeAddDetails, List<int> tradeCapturesToCancel)
        {
            return (tradeAddDetails != null) && (tradeCapturesToCancel?.Any()).True();
        }

        private List<TradePieces> GetTradesToCancel(List<int> tradeIdsToCancel, MandaraEntities dbContext)
        {
            List<TradeCapture> tradesToCancel =
                dbContext.TradeCaptures.Include(x => x.Portfolio)
                   .Include(x => x.BuyBook)
                   .Include(x => x.SellBook)
                   .Include(x => x.TradeGroup)
                   .Where(tc => tradeIdsToCancel.Contains(tc.TradeId))
                   .ToList();

            return tradesToCancel.Aggregate(
                new List<TradePieces>(),
                (secDefsForTrades, trade) => secDefsForTrades.AddF(
                    new TradePieces(trade, GetTradeSecDefAndProduct(trade))));
        }

        private IEnumerable<TradeCapture> CloneTrades(List<TradeCapture> tradeCaptures, TradeGroup tradeGroup)
        {
            return tradeCaptures.Select(trade => CloneTrade(trade, tradeGroup));
        }

        private TradeCapture CloneTrade(TradeCapture trade, TradeGroup tradeGroup)
        {
            TradeCapture clone = SimpleTradeCopy(trade);

            clone.IsTransferSell = clone.IsTransferSell.True() ? (bool?)null : true;

            if (trade.ReferencesNewSecurityDef())
            {
                clone.SecurityDefinition = trade.SecurityDefinition;
            }
            else
            {
                clone.SecurityDefinitionId = trade.SecurityDefinitionId;
            }

            clone.TradeGroup = tradeGroup;
            clone.Quantity *= -1M;
            clone.Side = !clone.Quantity.HasValue || (clone.Quantity < 0M) ? "Sell" : "Buy";
            ChangeExecIdAndLegRefId("d", trade, clone);
            return clone;
        }

        private TradeCapture SimpleTradeCopy(TradeCapture source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source", "TradesRepository.SimpleCopyTo cannot copy from null input");
            }

            TradeCapture destination = new TradeCapture();

            foreach (PropertyInfo propInfo in GetWriteableTradeCaptureProperties(source))
            {
                object value = propInfo.GetValue(source, null);

                propInfo.SetValue(destination, value, null);
            }

            return destination;
        }

        private AuditMessage CreateTradeEditAuditMsg(
            AuditContext auditContext,
            TradeAddDetails tradeAddDetails,
            MandaraEntities dbContext,
            List<TradePieces> tradesToCancel,
            List<TradeCapture> tradesForSave,
            TransferTradePortfolios buyAndSellBooks)
        {
            TradeAddDetails originalTradeDetails = _tradeAddHandlerConverter
                .ConvertTradeCaptureToTradeAddDetails(
                    TradeBackingDataFinder.GetOfficialProductForSecurityDef(
                        dbContext,
                        _securityDefinitionsStorage,
                        _productsStorage),
                    GetFxTrade(dbContext),
                    tradesToCancel,
                    _tradeAddHandlerConverter.GetActionSetTradeDetailsFromParent(false))
                .Value;
            TradeCapture firstTrade = tradesForSave.First();

            originalTradeDetails.Key = tradeAddDetails.Key = firstTrade.TradeId;

            tradeAddDetails.GroupId = firstTrade.GroupId;
            tradeAddDetails.TimestampUtc = firstTrade.TimeStamp;
            tradeAddDetails.Portfolio1 = buyAndSellBooks.SellBook;
            tradeAddDetails.Portfolio2 = buyAndSellBooks.BuyBook;

            AuditMessage tradeEditAudit = _auditService.CreateAuditMessage(
                auditContext,
                "Modify",
                originalTradeDetails,
                tradeAddDetails);

            _auditService.RemoveEqualDetails(tradeEditAudit.Details);
            ProtoEncoder.EncodeDetailsAsGzipProto(tradeEditAudit);

            return tradeEditAudit;
        }

        public void InsertTradeCaptures(
            List<TradeCapture> tradeCaptures,
            List<FxTrade> fxTrades,
            AuditContext auditContext,
            int? portfolioId = null,
            int? groupId = null,
            List<int> tradeIdsForCancel = null,
            string editCancelReason = null,
            TradeAddDetails tradeAddDetails = null)
        {
            if ((tradeCaptures == null) || (tradeCaptures.Count == 0))
            {
                return;
            }

            _productsStorage.SanitiseCircularReferences();

            using (MandaraEntities productsDb = CreateMandaraProductsDbContext())
            {
                using (DbContextTransaction trx = productsDb.Database.BeginTransaction(IsolationLevel.RepeatableRead))
                {
                    TradeGroup tradeGroup = GetTradeGroup(tradeCaptures, groupId, tradeIdsForCancel, productsDb);
                    Portfolio portfolio = GetTargetPortfolio(portfolioId, productsDb);
                    bool editTradesMode = IsEditingTrades(tradeAddDetails, tradeIdsForCancel);
                    List<TradePieces> tradesToCancel = editTradesMode
                        ? GetTradesToCancel(tradeIdsForCancel, productsDb)
                        : new List<TradePieces>();

                    // Exit if:
                    // 1) no trades were cancelled - we should return at this point because we are trying
                    // to edit already cancelled trades
                    // 2) if tradeCapturesToCancel is empty we would _not_ be here cause it mean we are
                    // adding new trades
                    bool tradesMarkedCancelled = editTradesMode
                        ? CancelTrades(
                            tradeCaptures,
                            auditContext,
                            tradesToCancel,
                            editCancelReason,
                            productsDb,
                            tradeGroup)
                        : CancelTrades(
                            tradeCaptures,
                            tradesToCancel,
                            editCancelReason,
                            productsDb,
                            tradeGroup);

                    if (!tradesMarkedCancelled)
                    {
                        return;
                    }

                    tradeCaptures.ForEach(trade => trade.TradeGroup = tradeGroup);

                    List<TradePieces> savedTrades = SaveAddedTrades(tradeCaptures, fxTrades, productsDb, portfolio);

                    if (editTradesMode)
                    {
                        SetAuditTrailForEditedTrades(
                            tradeCaptures,
                            auditContext,
                            tradeAddDetails,
                            productsDb,
                            tradesToCancel);
                    }
                    else
                    {
                        SetAuditTrailForNewTrades(tradeCaptures, auditContext, productsDb);
                    }

                    if (savedTrades.Any())
                    {
                        CalculatePreCalcDetails(productsDb, savedTrades);
                    }
                    _logger.Info("{0} with details - {1}", nameof(InsertTradeCaptures), tradeAddDetails);
                    productsDb.SaveChanges();
                    trx.Commit();

                    _productsStorage.SanitiseCircularReferences();
                }
            }
        }

        private void SetAuditTrailForNewTrades(List<TradeCapture> tradeCaptures, AuditContext auditContext, MandaraEntities productsDb)
        {
            List<AuditMessage> auditMessages = _auditService.CreateAuditMessages(auditContext, "Create", null, tradeCaptures);
            productsDb.AuditMessages.AddRange(auditMessages);
        }

        private void SetAuditTrailForEditedTrades(
            List<TradeCapture> tradeCaptures,
            AuditContext auditContext,
            TradeAddDetails tradeAddDetails,
            MandaraEntities productsDb,
            List<TradePieces> tradesToCancel)
        {
            TradeAddDetails originalTradeDetails = _tradeAddHandlerConverter.ConvertTradeCaptureToTradeAddDetails(
                                                                                TradeBackingDataFinder
                                                                                    .GetOfficialProductForSecurityDef(
                                                                                        productsDb,
                                                                                        _securityDefinitionsStorage,
                                                                                        _productsStorage),
                                                                                GetFxTrade(productsDb),
                                                                                tradesToCancel,
                                                                                _tradeAddHandlerConverter
                                                                                    .GetActionSetTradeDetailsFromParent(false))
                                                                            .Value;

            tradeAddDetails = SetTradeDetailsFromFirstTrade(tradeAddDetails, tradeCaptures.First());
            originalTradeDetails.Key = tradeAddDetails.Key;
            AddTradeModifyAuditMessages(auditContext, tradeAddDetails, originalTradeDetails, productsDb);
        }

        private List<TradePieces> SaveAddedTrades(
            List<TradeCapture> tradeCaptures,
            List<FxTrade> fxTrades,
            MandaraEntities productsDb,
            Portfolio portfolio)
        {
            List<TradeCapture> fxTradesDetail = fxTrades.Select(fx => fx.TradeCapture).ToList();
            List<TradePieces> nonFxTrades = AddNonFxTradesToContext(
                productsDb,
                tradeCaptures.Where(trade => !fxTradesDetail.Contains(trade)).ToList(),
                portfolio,
                null,
                null);

            AddFxTradesToContext(productsDb, fxTrades, portfolio);
            productsDb.SaveChanges();
            return nonFxTrades;
        }

        private static Portfolio GetTargetPortfolio(int? portfolioId, MandaraEntities dbContext)
        {
            return portfolioId.HasValue
                ? dbContext.Portfolios.SingleOrDefault(p => p.PortfolioId == portfolioId.Value)
                : null;
        }

        private static TradeGroup GetTradeGroup(List<TradeCapture> tradeCaptures, int? groupId, List<int> tradeIdsForCancel, MandaraEntities dbContext)
        {
            return tradeCaptures.Count > 1 || (tradeIdsForCancel?.Count > 0)
                ? GetTradeGroup(groupId ?? 0, dbContext)
                : null;
        }

        private bool CancelTrades(
            List<TradeCapture> tradeCaptures,
            AuditContext auditContext,
            List<TradePieces> tradesToCancel,
            string editCancelReason,
            MandaraEntities productsDb,
            TradeGroup tradeGroup)
        {
            if (tradesToCancel.Any())
            {
                SetNewTradesTrader(tradeCaptures, tradesToCancel);
            }

            return CancelTradeCaptures(
                    auditContext,
                    productsDb,
                    tradesToCancel.Select(trade => trade.Trade.TradeId).ToHashSet(),
                    editCancelReason,
                    tradeGroup)
                .cancellationsCreated;
        }

        private bool CancelTrades(
            List<TradeCapture> tradeCaptures,
            List<TradePieces> tradesToCancel,
            string editCancelReason,
            MandaraEntities productsDb,
            TradeGroup tradeGroup)
        {
            if (tradesToCancel.Any())
            {
                SetNewTradesTrader(tradeCaptures, tradesToCancel);
            }

            return CancelTradeCaptures(
                    productsDb,
                    tradesToCancel.Select(trade => trade.Trade.TradeId).ToHashSet(),
                    editCancelReason,
                    tradeGroup)
                .cancellationsCreated;
        }

        private SecurityDefPieces GetTradeSecDefAndProduct(TradeCapture trade)
        {
            TryGetResult<SecurityDefinition> secDef =
                _securityDefinitionsStorage.TryGetSecurityDefinition(trade.SecurityDefinitionId);

            if (secDef.HasValue)
            {
                Product product = GetSecDefProduct(secDef.Value);

                return new SecurityDefPieces(secDef.Value, product);
            }

            return null;
        }

        private Product GetSecDefProduct(SecurityDefinition secDef)
        {
            if (null == secDef.Product)
            {
                TryGetResult<Product> product = _productsStorage.TryGetProduct(secDef.product_id.Value);

                return product.HasValue ? product.Value : null;
            }

            return secDef.Product;
        }

        private TradeAddDetails SetTradeDetailsFromFirstTrade(TradeAddDetails tradeDetails, TradeCapture sourceTrade)
        {
            tradeDetails.Key = sourceTrade.TradeId;
            tradeDetails.GroupId = sourceTrade.GroupId;
            tradeDetails.TimestampUtc = sourceTrade.TimeStamp;
            tradeDetails.Portfolio1 = sourceTrade.Portfolio;
            return tradeDetails;
        }

        private void AddTradeModifyAuditMessages(
            AuditContext auditContext,
            TradeAddDetails tradeAddDetails,
            TradeAddDetails originalTradeDetails,
            MandaraEntities dbContext)
        {
            AuditMessage auditMessages = _auditService.CreateAuditMessage(
                auditContext,
                "Modify",
                originalTradeDetails,
                tradeAddDetails);

            _auditService.RemoveEqualDetails(auditMessages.Details);
            ProtoEncoder.EncodeDetailsAsGzipProto(auditMessages);

            dbContext.AuditMessages.Add(auditMessages);
        }

        private void AddFxTradesToContext(MandaraEntities dbContext, List<FxTrade> fxTrades, Portfolio portfolio)
        {
            SetNewTradeCaptureBackingData(
                dbContext,
                fxTrades.Select(fx => fx.TradeCapture).ToList(),
                portfolio,
                null,
                null);

            foreach (FxTrade fxTrade in fxTrades)
            {
                SetFxTradeProductId(fxTrade, dbContext);
                SetFxTradeCurrencyId(
                    () => fxTrade.AgainstCurrency,
                    (currId) =>
                    {
                        fxTrade.AgainstCurrency = null;
                        fxTrade.AgainstCurrencyId = currId;
                    });
                SetFxTradeCurrencyId(
                    () => fxTrade.SpecifiedCurrency,
                    (currId) =>
                    {
                        fxTrade.SpecifiedCurrency = null;
                        fxTrade.SpecifiedCurrencyId = currId;
                    });
                fxTrade.TradeCapture.Portfolio = portfolio;
            }
            dbContext.FxTrades.AddRange(fxTrades);
        }

        private void SetFxTradeProductId(FxTrade fxTrade, MandaraEntities dbContext)
        {
            Product fxProduct = GetExistingProduct(dbContext, fxTrade.Product.ProductId);

            fxTrade.Product = null;
            fxTrade.ProductId = fxProduct.ProductId;
        }

        private void SetFxTradeCurrencyId(Func<Currency> getCurrentCurrency, Action<int> setCurrId)
        {
            Currency currentCurr = getCurrentCurrency();

            if (null != currentCurr)
            {
                setCurrId(currentCurr.CurrencyId);
            }
        }

        public (bool cancellationsCreated, List<TradeCapture> cancelledTrades) CancelTradeCaptures(
            MandaraEntities productsDb,
            HashSet<int> tradeCaptures,
            string editCancelReason,
            TradeGroup tradeGroup = null)
        {
            if ((tradeCaptures == null) || (tradeCaptures.Count == 0))
            {
                return (true, new List<TradeCapture>());
            }

            bool tradesCancelsCreated = false;
            List<TradeCapture> tradesToCancel =
                productsDb.TradeCaptures.Where(trade => tradeCaptures.Contains(trade.TradeId)).ToList();
            DateTime timestamp = SystemTime.Now();

            foreach (TradeCapture trade in tradesToCancel)
            {
                tradesCancelsCreated = (CreateTradeCancelChange(
                                            trade,
                                            tradeGroup,
                                            timestamp,
                                            editCancelReason,
                                            productsDb)
                                        > 0)
                                       || tradesCancelsCreated;
            }

            return (tradesCancelsCreated, tradesToCancel);
        }

        private int CreateTradeCancelChange(
            TradeCapture trade,
            TradeGroup tradeGroup,
            DateTime tradeChangeTime,
            string editCancelReason,
            MandaraEntities productsDb)
        {
            bool tradeFilled = trade.OrdStatus == TradeOrderStatus.Filled;
            int tradeChangesCreated = 0;

            trade.OrdStatus = TradeOrderStatus.Cancelled;
            trade.TimeStamp = tradeChangeTime;

            if (tradeGroup != null)
            {
                trade.TradeGroup = tradeGroup;
            }

            if (tradeFilled)
            {
                TradeChange cancel = TradeChange.Create(
                    trade,
                    tradeChangeTime,
                    TradeChangeType.Cancelled,
                    SecurityDefinitionsManager.GetTradeChangeEntityType(trade.SecurityDefinitionId));

                productsDb.TradeChanges.Add(cancel);
                tradeChangesCreated = 1;
            }

            trade.EditCancelReason = editCancelReason;
            return tradeChangesCreated;
        }

        public (bool cancellationsCreated, List<TradeCapture> cancelledTrades) CancelTradeCaptures(
            AuditContext auditContext,
            MandaraEntities productsDb,
            HashSet<int> tradeCaptures,
            string editCancelReason,
            TradeGroup tradeGroup = null)
        {
            if (tradeCaptures?.Count == 0)
            {
                return (true, new List<TradeCapture>());
            }

            (bool cancellationsCreated, List<TradeCapture> cancelledTrades) = CancelTradeCaptures(
                productsDb,
                tradeCaptures,
                editCancelReason,
                tradeGroup);
            List<TradeCapture> tradesToCancel =
                productsDb.TradeCaptures.Where(trade => tradeCaptures.Contains(trade.TradeId))
                    .Include(x => x.SecurityDefinition)
                    .ToList();

            CreateTradeCancelAuditTrail(auditContext, productsDb, tradesToCancel);
            return (cancellationsCreated, cancelledTrades);
        }

        private void CreateTradeCancelAuditTrail(
            AuditContext auditContext,
            MandaraEntities productsDb,
            List<TradeCapture> tradesToCancel)
        {
            List<AuditMessage> auditMessages =
                _auditService.CreateAuditMessages(auditContext, "Modify", tradesToCancel, null);

            // TODO: It doesn't make much sense to write the audit messages if skipping audit messages...
            for (int i = 0; i < tradesToCancel.Count; i++)
            {
                AuditMessage message = auditMessages[i];
                message.Details.Property = nameof(TradeCapture.EditCancelReason);
                message.Details.NewValue = tradesToCancel[i].EditCancelReason;
            }
            auditMessages = _auditService.UpdateAuditMessages(auditMessages, tradesToCancel);
            productsDb.AuditMessages.AddRange(auditMessages);
        }

        private static bool IsFxSecurityDefinition(SecurityDefinition secDef)
        {
            return null != secDef && secDef.SecurityDefinitionId == SecurityDefinitionsManager.FxSecDefId;
        }

        public List<TradePieces> AddNonFxTradesToContext(
            MandaraEntities dbContext,
            List<TradeCapture> tradeCaptures,
            Portfolio portfolio,
            Portfolio sellPortfolio,
            Portfolio buyPortfolio)
        {
            List<TradePieces> tradePieces = SetNewTradeCaptureBackingData(
                dbContext,
                tradeCaptures,
                portfolio,
                sellPortfolio,
                buyPortfolio);
            List<TradeCapture> preparedTrades = tradePieces.Select(trade => trade.Trade).ToList();

            preparedTrades.Where(trade => null != trade.PrecalcDetails)
                         .ForEach(
                             trade => trade.PrecalcDetails.ForEach(
                                 precalc =>
                                 {
                                     int productId = precalc.ProductId;

                                     precalc.Product = null;
                                     precalc.ProductId = productId;
                                 }));
            dbContext.TradeCaptures.AddRange(preparedTrades);
            UpdateTotalQtyAndAvgPx(preparedTrades);
            return tradePieces;
        }

        private List<TradePieces> SetNewTradeCaptureBackingData(
            MandaraEntities dbContext,
            List<TradeCapture> tradeCaptures,
            Portfolio portfolio,
            Portfolio sellPortfolio,
            Portfolio buyPortfolio)
        {
            return tradeCaptures.Select(BuildTradePieces).Where(trade => null != trade).ToList();

            TradePieces BuildTradePieces(TradeCapture trade)
            {
                SecurityDefPieces secDef = GetTradeSecurityDefinition(trade);

                if (IsFxSecurityDefinition(secDef.SecurityDef))
                {
                    return null;
                }

                return new TradePieces(
                    SetNewTradeBackingData(
                        dbContext,
                        portfolio,
                        sellPortfolio,
                        buyPortfolio,
                        trade,
                        secDef),
                    secDef);
            }
        }

        private TradeCapture SetNewTradeBackingData(
            MandaraEntities dbContext,
            Portfolio portfolio,
            Portfolio sellPortfolio,
            Portfolio buyPortfolio,
            TradeCapture trade,
            SecurityDefPieces secDef)
        {
            SetTradeSide(trade);
            SetTradePortfolios(dbContext, portfolio, sellPortfolio, buyPortfolio, trade);
            CalculateBrokerage(new TradePieces(trade, secDef));
            trade.ExpiryDate = null;

            if (secDef.SecurityDef.IsNew())
            {
                int? productId = secDef.SecurityDef.product_id;

                secDef.SecurityDef.Product = null;
                secDef.SecurityDef.product_id = productId;
            }
            else
            {
                trade.SecurityDefinitionId = secDef.SecurityDef.SecurityDefinitionId;
            }

            return trade;
        }

        private SecurityDefPieces GetTradeSecurityDefinition(TradeCapture trade)
        {
            SecurityDefinition securityDefinition = trade.SecurityDefinition;
            Product product = securityDefinition?.Product;

            if (null != securityDefinition || trade.ReferencesExistingSecurityDef())
            {
                securityDefinition = GetExistingSecurityDefinition(trade);

                if (null != securityDefinition && !securityDefinition.IsNew())
                {
                    trade.SecurityDefinition = null;
                    trade.SecurityDefinitionId = securityDefinition.SecurityDefinitionId;
                }

                product = GetProductForSecDef(securityDefinition);

                if (!IsFxSecurityDefinition(securityDefinition))
                {
                    trade.Symbol = securityDefinition.UnderlyingSymbol;
                }
            }

            return new SecurityDefPieces(securityDefinition, product);
        }

        private Product GetProductForSecDef(SecurityDefinition securityDefinition)
        {
            Product product = null;

            if (securityDefinition.ReferencesExistingProduct())
            {
                TryGetResult<Product> productOption =
                    _productsStorage.TryGetProduct(securityDefinition.product_id.Value);

                if (productOption.HasValue)
                {
                    product = productOption.Value;
                }
            }
            else
            {
                product = securityDefinition.Product;
            }

            return product;
        }

        private SecurityDefinition GetExistingSecurityDefinition(TradeCapture trade)
        {
            SecurityDefinition tradeSecDef = null;

            if (trade.ReferencesExistingSecurityDef())
            {
                tradeSecDef = _securityDefinitionsStorage.TryGetSecurityDefinition(trade.SecurityDefinitionId).Value;
            }

            if (null == tradeSecDef)
            {
                tradeSecDef = GetSecurityDefFromTradeData(trade);
            }

            if (null != tradeSecDef)
            {
                CleanUpPrecalcDetails(trade, tradeSecDef);
            }

            return tradeSecDef;
        }

        private SecurityDefinition GetSecurityDefFromTradeData(TradeCapture trade)
        {
            if (!trade.SecurityDefinition.IsNew())
            {
                return _securityDefinitionsStorage
                    .TryGetSecurityDefinition(trade.SecurityDefinition.SecurityDefinitionId)
                    .Value;
            }

            return GetClosestMatchingSecDef(trade);
        }

        private static void CleanUpPrecalcDetails(TradeCapture trade, SecurityDefinition tradeSecDef)
        {
            bool isFxSecDef = IsFxSecurityDefinition(tradeSecDef);

            if (isFxSecDef)
            {
                tradeSecDef.PrecalcDetails = new List<PrecalcSdDetail>();
            }

            if (!isFxSecDef && tradeSecDef.HasPrecalcDetails())
            {
                trade.PrecalcDetails = null;
                trade.PrecalcPositions = null;
            }
        }

        private SecurityDefinition GetClosestMatchingSecDef(TradeCapture trade)
        {
            TryGetResult<SecurityDefinition> secDefOption =
                _securityDefinitionsStorage.TryGetSecurityDefinitionByFields(
                    secDef => (secDef.StripName == trade.SecurityDefinition.StripName)
                              && (secDef.product_id == trade.SecurityDefinition.product_id)
                              && (secDef.Exchange == trade.SecurityDefinition.Exchange)
                              && ((secDef.HubAlias == trade.SecurityDefinition.HubAlias)
                                  || (trade.SecurityDefinition.HubAlias == null)));

            return secDefOption.HasValue && secDefOption.Value != trade.SecurityDefinition
                ? secDefOption.Value
                : trade.SecurityDefinition;
        }

        private static void SetTradeSide(TradeCapture trade)
        {
            trade.Side = !trade.Quantity.HasValue || (trade.Quantity < 0M) ? TradeSide.Sell : TradeSide.Buy;
        }

        private static void SetTradePortfolios(
            MandaraEntities dbContext,
            Portfolio portfolio,
            Portfolio sellPortfolio,
            Portfolio buyPortfolio,
            TradeCapture trade)
        {
            trade.Portfolio = portfolio;

            if (sellPortfolio != null)
            {
                trade.SellBook = sellPortfolio;
            }
            else if (trade.SellBook != null)
            {
                trade.SellBook =
                    dbContext.Portfolios.SingleOrDefault(p => p.PortfolioId == trade.SellBook.PortfolioId);
            }

            if (buyPortfolio != null)
            {
                trade.BuyBook = buyPortfolio;
            }
            else if (trade.BuyBook != null)
            {
                trade.BuyBook =
                    dbContext.Portfolios.SingleOrDefault(p => p.PortfolioId == trade.BuyBook.PortfolioId);
            }
        }

        private Func<int, TryGetResult<FxTrade>> GetFxTrade(MandaraEntities dbContext)
        {
            return (tradeId) => _fxTradesDataProvider.ReadFxTradeByTradeId(dbContext, tradeId);
        }

        private static IEnumerable<PropertyInfo> GetWriteableTradeCaptureProperties(TradeCapture source)
        {
            return
                source.GetType()
                      .GetProperties()
                      .Where(
                          p =>
                              p.CanWrite && !p.PropertyType.Namespace.Contains("Entities")
                              && (p.Name != "ChangeTracker"));
        }

        private void ChangeExecIdAndLegRefId(string prefix, TradeCapture origTrade, TradeCapture newTrade)
        {
            if (char.IsLetter(origTrade.ExecID[0]))
            {
                newTrade.ExecID = GetExecIdMap(prefix + origTrade.ExecID);
            }
            else
            {
                newTrade.ExecID = prefix + origTrade.ExecID;
            }

            if (origTrade.LegRefID == null)
            {
                return;
            }

            if (origTrade.ExecID.Trim() == origTrade.LegRefID.Trim())
            {
                newTrade.LegRefID = newTrade.ExecID;
            }
        }

        private string GetExecIdMap(string execId)
        {
            if (_execIdDictionary == null)
            {
                _execIdDictionary = new Dictionary<string, string>();
            }

            if (_execIdDictionary.ContainsKey(execId))
            {
                return _execIdDictionary[execId];
            }

            string newExecId = GetNewExecId();
            _execIdDictionary.Add(execId, newExecId);

            return newExecId;
        }

        private string GetNewExecId()
        {
            return GuidExtensions.NumericGuid(GuidExtensions.HalfGuidLength);
        }

        public void CalculatePreCalcDetails(MandaraEntities dbContext, List<TradePieces> trades)
        {
            List<ICollection<PrecalcSdDetail>> secDefPrecalcDetails = new List<ICollection<PrecalcSdDetail>>();
            List<ICollection<PrecalcTcDetail>> tradeCapturePrecalcDetails = new List<ICollection<PrecalcTcDetail>>();

            foreach (TradePieces trade in trades)
            {
                Tuple<ICollection<PrecalcTcDetail>, ICollection<PrecalcSdDetail>> precalcDetails =
                    _precalcPositionsCalculator.CalculatePrecalcPositions(
                        trade.Trade,
                        trade.Security.SecurityDef,
                        trade.Security.Product);

                tradeCapturePrecalcDetails.Add(GetNewPrecalcDetails(precalcDetails.Item1));
                secDefPrecalcDetails.Add(GetNewPrecalcDetails(precalcDetails.Item2));
            }

            SavePrecalcDetails(dbContext, secDefPrecalcDetails, tradeCapturePrecalcDetails);
        }

        private void SavePrecalcDetails(
            MandaraEntities dbContext,
            List<ICollection<PrecalcSdDetail>> secDefPrecalcDetails,
            List<ICollection<PrecalcTcDetail>> tradeCapturePrecalcDetails)
        {
            secDefPrecalcDetails.Where(ValidPrecalcDetails)
                                .ForEach(
                                    precalc => AddSecurityDefPrecalcDetailsToDbContext(dbContext, precalc.ToList()));
            tradeCapturePrecalcDetails.Where(ValidPrecalcDetails)
                                      .ForEach(
                                          precalc => AddTradeCapturePrecalcDetailsToDbContext(
                                              dbContext,
                                              precalc.ToList()));
        }

        private List<T> GetNewPrecalcDetails<T>(ICollection<T> precalcDetails) where T : INewable
        {
            return precalcDetails.Where(precalc => precalc.IsNew()).ToList();
        }

        private bool ValidPrecalcDetails<T>(ICollection<T> precalcDetails)
        {
            return precalcDetails != null && precalcDetails.Any();
        }

        private void AddSecurityDefPrecalcDetailsToDbContext(MandaraEntities dbContext, List<PrecalcSdDetail> precalcSdDetailsList)
        {
            precalcSdDetailsList.ForEach(
                precalc =>
                {
                    if (IsProductMissingOrNotInDbContext(dbContext, precalc))
                    {
                        precalc.Product = GetProduct(precalc.ProductId, dbContext);
                    }

                    if (IsSecurityDefinitionMissingOrNotInDbContext(dbContext, precalc))
                    {
                        precalc.SecurityDefinition = GetSecurityDefinition(precalc.SecurityDefinitionId, dbContext);
                    }

                    if (IsSecDefProductMissingOrNotInDbContext(dbContext, precalc))
                    {
                        precalc.SecurityDefinition.Product = GetProduct(precalc.SecurityDefinition.product_id.Value, dbContext);
                    }
                });

            dbContext.PrecalcSdDetails.AddRange(precalcSdDetailsList);
        }

        private static bool IsProductMissingOrNotInDbContext(MandaraEntities dbContext, PrecalcSdDetail p)
        {
            return (p.Product == null) || (dbContext.Entry(p.Product).State == EntityState.Detached);
        }

        private static bool IsSecurityDefinitionMissingOrNotInDbContext(MandaraEntities dbContext, PrecalcSdDetail p)
        {
            return (p.SecurityDefinition == null)
                   || (dbContext.Entry(p.SecurityDefinition).State == EntityState.Detached);
        }

        private static bool IsSecDefProductMissingOrNotInDbContext(MandaraEntities dbContext, PrecalcSdDetail p)
        {
            return (p.SecurityDefinition.product_id != null)
                   && ((p.SecurityDefinition.Product == null)
                       || (dbContext.Entry(p.SecurityDefinition).State == EntityState.Detached));
        }

        private Product GetProduct(int productId, MandaraEntities context)
        {
            Product product;
            Product localProduct = context.Products.Local.FirstOrDefault(x => x.ProductId == productId);

            if (localProduct == null)
            {
                product = new Product { ProductId = productId };
                context.Entry(product).State = EntityState.Unchanged;
            }
            else
            {
                product = localProduct;
            }
            return product;
        }

        private SecurityDefinition GetSecurityDefinition(int securityDefinitionId, MandaraEntities context)
        {
            SecurityDefinition securityDefinition;
            SecurityDefinition localSecurityDefinition =
                context.SecurityDefinitions.Local.FirstOrDefault(x => x.SecurityDefinitionId == securityDefinitionId);

            if (localSecurityDefinition == null)
            {
                securityDefinition = new SecurityDefinition { SecurityDefinitionId = securityDefinitionId };
                context.Entry(securityDefinition).State = EntityState.Unchanged;
            }
            else
            {
                securityDefinition = localSecurityDefinition;
            }

            return securityDefinition;
        }

        private void AddTradeCapturePrecalcDetailsToDbContext(MandaraEntities dbContext, List<PrecalcTcDetail> precalcTcDetailsList)
        {
            precalcTcDetailsList.ForEach(
                p =>
                {
                    if ((p.Product == null) || (dbContext.Entry(p.Product).State == EntityState.Detached))
                    {
                        p.Product = GetProduct(p.ProductId, dbContext);
                    }
                });

            dbContext.PrecalcTcDetails.AddRange(precalcTcDetailsList);
        }

        private void CalculateBrokerage(TradePieces trade)
        {
            if (_tradeBrokerageCalculationManager == null)
            {
                _tradeBrokerageCalculationManager = new TradeBrokerageCalculationManager();
            }

            trade.Trade.Brokerage = _tradeBrokerageCalculationManager.CalculateTradeBrokerage(trade);
        }

        private Product GetExistingProduct(MandaraEntities dbContext, int productId)
        {
            TryGetResult<Product> product = _productsStorage.TryGetProduct(productId);

            if (!product.HasValue)
            {
                return dbContext.Products.SingleOrDefault(x => x.ProductId == productId);
            }

            return product.Value;
        }

        /// <summary>
        ///     Updates TotalQty and AveragePx for trades in tradeCaptures
        /// </summary>
        /// <param name="dbContext">context</param>
        /// <param name="tradeCaptures">Trade Captures</param>
        private void UpdateTotalQtyAndAvgPx(List<TradeCapture> tradeCaptures)
        {
            foreach (TradeCapture tc in tradeCaptures)
            {
                tc.TotalQty = tc.Quantity;
                tc.AveragePx = tc.Price;
            }
        }

        private void MarkReadOnlyEntitiesInDbContext(Type[] modifiableTypes, MandaraEntities dbContext)
        {
            IEnumerable<DbEntityEntry> addedOrModified = dbContext
                .ChangeTracker
                .Entries()
                .Where(e => e.State == EntityState.Added);

            addedOrModified.ForEach(
                entity =>
                {
                    if (!IsNewEntity(entity, modifiableTypes))
                    {
                        entity.State = EntityState.Unchanged;
                    }
                });
        }

        private bool IsNewEntity(DbEntityEntry entity, Type[] modifiableTypes)
        {
            bool isModifiable = modifiableTypes.Contains(entity.Entity.GetType());
            INewable newableEntity = entity.Entity as INewable;
            bool isNew = isModifiable && null != newableEntity && newableEntity.IsNew();

            return isNew;
        }

        public List<SdQuantityModel> ReadAllQuantities(
            MandaraEntities productsDb,
            DateRange dateRange,
            List<int> portfolioIds)
        {
            return ReadQuantities(productsDb, String.Empty, dateRange, portfolioIds);
        }

        public List<SdQuantityModel> ReadNonFuturesQuantities(
            MandaraEntities productsDb,
            DateRange dateRange,
            List<int> portfolioIds)
        {
            return ReadQuantities(productsDb, "and p.product_type <> 0", dateRange, portfolioIds);
        }

        private List<SdQuantityModel> ReadQuantities(
            MandaraEntities productsDb,
            string productsFilter,
            DateRange dateRange,
            List<int> portfolioIds)
        {
            var startDateParam = new SqlParameter("@startDate", dateRange.Start);
            var endDateParam = new SqlParameter("@endDate", dateRange.End);
            var secDefQtyQuery = string.Format(PricingReportQuery, productsFilter);
            var dbResult = productsDb.Database.SqlQuery<SdQuantityModel>(secDefQtyQuery, startDateParam, endDateParam)
                                    .AsEnumerable();

            //TODO how to do this efficiently in DB
            if (portfolioIds.Count > 0)
            {
                dbResult = dbResult.Where(x => portfolioIds.Contains(x.PortfolioId));
            }

            return dbResult.ToList();
        }

        /// The reason of the inner join with temp result is that we don't want to get multiple result on trade capture level
        /// as precalc_details_sd is 1:N relationship of trades
        /// In high level we Sum trade quantity of all security definition by id which has position affect
        /// between the date ranges. Query use index on precalc_details_sd to make the internal lookup index scan only
        private static String PricingReportQuery =
            @"select 
                    trade.idSecurityDefinition as SecurityDefinitionId,
                    trade.PortfolioId ,Sum(trade.LastQty) as TradesQuantity,
                    0.0 as TradesFxExposure 
                from trade_captures trade
                join security_definitions secdefs on trade.idSecurityDefinition = secdefs.idSecurityDefinition
                join (select Distinct(SecurityDefinitionId) from precalc_details_sd precalc
                        join products as p on precalc.ProductId = p.product_id
                        where @endDate >= MinDay and @startDate <= MaxDay {0}) 
                    precalc on trade.idSecurityDefinition = precalc.SecurityDefinitionId
                where trade.OrdStatus = 'Filled' and trade.IsParentTrade = 1
                group by trade.idSecurityDefinition, trade.PortfolioId
                having Sum(trade.LastQty) <> 0 ";

    }
}