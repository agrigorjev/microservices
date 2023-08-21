using AutoMapper.QueryableExtensions;
using EntityFramework.Extensions;
using EntityFramework.Future;
using Mandara.Business.Audit;
using Mandara.Business.Contracts;
using Mandara.Business.Dates;
using Mandara.Business.Extensions;
using Mandara.Business.Model;
using Mandara.Business.TradeAdd;
using Mandara.Date;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Entities.EntityPieces;
using Mandara.Extensions.Option;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Mandara.Business.Services
{
    public class LegTradesRepository : ITradesRepository
    {
        private readonly IProductsStorage _productsStorage;
        private readonly ISecurityDefinitionsStorage _securityDefinitionsStorage;
        private readonly ITradeSplitService _tradeSplitService;
        private readonly IEndOfDayDateTimeProvider _endOfDayDateTimes;

        public LegTradesRepository(
            IProductsStorage productsStorage,
            ISecurityDefinitionsStorage securityDefinitionsStorage,
            ITradeSplitService tradeSplitService,
            IEndOfDayDateTimeProvider eodDateTimes)
        {
            _productsStorage = productsStorage;
            _securityDefinitionsStorage = securityDefinitionsStorage;
            _tradeSplitService = tradeSplitService;
            _endOfDayDateTimes = eodDateTimes;
        }

        public TradesQuantityData ReadQuantities(TradeReadFilterParameters filterParams)
        {
            List<SdQuantityModel> sdQuantities;
            int latestTradeId;
            int latestSecDefId;
            int latestTradeChangeId;
            List<TradeModel> trades;
            List<SdQuantityModel> sdLegsQuantities;
            DateTime calculationDay = (filterParams.RiskDate ?? SystemTime.Today()).Date;
            List<TradeModel> todayTasTrades;

            using (MandaraEntities cxt = new MandaraEntities(
                MandaraEntities.DefaultConnStrName,
                nameof(LegTradesRepository)))
            {
                cxt.Database.CommandTimeout = 0;

                FutureQuery<SdQuantityModel> futureSdQuantities = ConstructSecurityDefModelQuery(
                    cxt,
                    filterParams.PortfolioIds,
                    filterParams.TillTransactTime);

                FutureQuery<TradeModel> futureTrades = ConstructTradeModelQuery(
                    cxt,
                    filterParams.PortfolioIds,
                    filterParams.TillTransactTime);

                FutureQuery<TradeModel> futureTodayTasTrades = ConstructFutureTasTradesQuery(
                    cxt,
                    filterParams.PortfolioIds,
                    calculationDay,
                    filterParams.TillTransactTime);
                FutureQuery<TradeCapture> futureLatestTrade =
                    cxt.TradeCaptures.OrderByDescending(tc => tc.TradeId).Take(1).Future();
                FutureQuery<SecurityDefinition> futureLatestSd =
                    cxt.SecurityDefinitions.OrderByDescending(tc => tc.SecurityDefinitionId).Take(1).Future();
                FutureQuery<TradeChange> futureLatestTradeChange =
                    cxt.TradeChanges.OrderByDescending(tc => tc.ChangeId).Take(1).Future();
                DbContextTransaction trx = null;

                try
                {
                    if (filterParams.SetTransaction)
                    {
                        trx = cxt.Database.BeginTransaction(IsolationLevel.RepeatableRead);
                    }

                    TradeCapture latestTrade = futureLatestTrade.FirstOrDefault();
                    SecurityDefinition latestSd = futureLatestSd.FirstOrDefault();
                    TradeChange latestTradeChange = futureLatestTradeChange.FirstOrDefault();

                    latestTradeId = latestTrade?.TradeId ?? 0;
                    latestSecDefId = latestSd?.SecurityDefinitionId ?? 0;
                    latestTradeChangeId = latestTradeChange?.ChangeId ?? 0;

                    trades = futureTrades.ToList();

                    todayTasTrades = futureTodayTasTrades.ToList();
                    sdQuantities = futureSdQuantities.ToList();
                    trades.ForEach(trade => trade.IsParentTrade = true);

                    List<TradeModel> historicTrades =
                        _tradeSplitService.SplitHistoricCustomPeriodTrades(
                            GetHistoricTrades(filterParams, trades).ToList());
                    List<TradeModel> liveTrades =
                        _tradeSplitService.SplitLiveCustomPeriodTrades(GetLiveTrades(filterParams, trades).ToList());

                    sdQuantities = _tradeSplitService.SplitQCalTimeSpreadIntoPerMonth(sdQuantities);
                    trades = historicTrades.Concat(liveTrades).OrderBy(trade => trade.TradeId).ToList();
                    sdLegsQuantities = new List<SdQuantityModel>();
                }
                finally
                {
                    if (filterParams.SetTransaction)
                    {
                        trx?.Commit();
                    }
                }
            }

            RemoveTasTrades(todayTasTrades, sdQuantities, filterParams.RiskDate);

            return new TradesQuantityData(
                sdQuantities,
                sdLegsQuantities,
                trades,
                latestTradeId,
                latestSecDefId,
                latestTradeChangeId);
        }

        private IEnumerable<TradeModel> GetHistoricTrades(
            TradeReadFilterParameters filterParams,
            List<TradeModel> trades)
        {
            DateTime liveTradeBoundary = GetLiveTradeBoundary(filterParams);

            return trades.Where(trade => trade.TradeDate < liveTradeBoundary);
        }

        private DateTime GetLiveTradeBoundary(TradeReadFilterParameters filterParams)
        {
            DateTime riskDate = filterParams.RiskDate ?? SystemTime.Now();
            TryGetResult<DateTime> prevDayEoD = _endOfDayDateTimes.PrevBusinessDayEndOfDay(riskDate);
            DateTime liveTradeBoundary = prevDayEoD.HasValue ? prevDayEoD.Value : riskDate;

            return liveTradeBoundary;
        }

        private IEnumerable<TradeModel> GetLiveTrades(
            TradeReadFilterParameters filterParams,
            List<TradeModel> trades)
        {
            DateTime liveTradeBoundary = GetLiveTradeBoundary(filterParams);

            return trades.Where(trade => trade.TradeDate >= liveTradeBoundary);
        }

        private FutureQuery<SdQuantityModel> ConstructSecurityDefModelQuery(
            MandaraEntities cxt,
            List<int> portfolioIds,
            DateTime? tillTransactTime = null)
        {
            IQueryable<TradeCapture> sdQuery = TradesQueryProvider.GetSecDefBasedLegsOrSingleParentsTradesQuery(
                cxt,
                new DateRange(DateRange.DefaultStart, tillTransactTime),
                _endOfDayDateTimes,
                _productsStorage,
                portfolioIds);

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

            return futureSdQuantities;
        }

        private FutureQuery<TradeModel> ConstructTradeModelQuery(
            MandaraEntities cxt,
            List<int> portfolioIds,
            DateTime? tillTransactTime = null)
        {
            IQueryable<TradeCapture> tradesQuery = TradesQueryProvider.GetCustomPeriodLegsTradesQuery(
                cxt,
                new DateRange(DateRange.DefaultStart, tillTransactTime),
                _endOfDayDateTimes,
                _productsStorage,
                portfolioIds);

            FutureQuery<TradeModel> futureTrades =
                tradesQuery.AsNoTracking().ProjectTo<TradeModel>().OrderBy(tc => tc.TradeId).Future();

            return futureTrades;
        }

        private FutureQuery<TradeModel> ConstructFutureTasTradesQuery(
            MandaraEntities cxt,
            List<int> portfolioIds,
            DateTime today,
            DateTime? cutoffDateTime)
        {
            IQueryable<TradeCapture> query = TradesQueryProvider.GetTasTradesForDayQuery(
                cxt,
                today,
                cutoffDateTime,
                _endOfDayDateTimes,
                _productsStorage,
                portfolioIds);

            FutureQuery<TradeModel> futureTodayTasTrades =
                query.AsNoTracking().ProjectTo<TradeModel>().OrderBy(tc => tc.TradeId).Future();

            return futureTodayTasTrades;
        }

        private void RemoveTasTrades(
            List<TradeModel> todayTasTrades,
            List<SdQuantityModel> sdQuantities,
            DateTime? riskDate)
        {
            foreach (TradeModel tasTrade in todayTasTrades)
            {
                SecurityDefinition securityDefinition =
                    _securityDefinitionsStorage.TryGetSecurityDefinition(tasTrade.SecurityDefinitionId).Value;
                Product product = _productsStorage.TryGetProduct(securityDefinition.product_id.Value).Value;
                DateTime tasActivationDate = _productsStorage.GetTasActivationDate(product, tasTrade.TradeDate.Value);

                if (tasActivationDate > product.GetTasActivationTime(riskDate))
                {
                    // remove tas trade from positions
                    SdQuantityModel sdQuantity =
                        sdQuantities.FirstOrDefault(
                            x =>
                                (x.PortfolioId == tasTrade.PortfolioId)
                                && (x.SecurityDefinitionId == tasTrade.SecurityDefinitionId));

                    if (sdQuantity != null)
                    {
                        sdQuantity.TradesQuantity -= tasTrade.Quantity.Value;
                    }
                }
            }
        }

        public List<TradeCapture> GetTradesWithSameGroup(int groupId, string ordStatus = null)
        {
            ThrowInvalidOperationException("GetTradesWithSameGroup");
            return null;
        }

        public List<TradeCapture> GetFullSpreadTrades(TradeCapture spreadTrade)
        {
            ThrowInvalidOperationException("GetFullSpreadTrades");
            return null;
        }

        public TradeCapture GetTradeWithSdById(int tradeId)
        {
            ThrowInvalidOperationException("GetTradeWithSdById");
            return null;
        }

        public List<TradeCapture> GetTradesById(List<int> tradesIds)
        {
            ThrowInvalidOperationException("GetTradesById");
            return null;
        }

        public void AssignTrades(
            AuditContext auditContext,
            int toPortfolioId,
            string toPortfolioName,
            Dictionary<string, List<DateTime?>> execIdsOfTradesToAssign,
            string userName)
        {
            ThrowInvalidOperationException("AssignTrades");
        }

        private void ThrowInvalidOperationException(string invalidOperationName)
        {
            throw new InvalidOperationException(
                $"{invalidOperationName}: LegTradeRepository is not for use as a replacement for TradeRepository");
        }

        public List<int> CreateTrades(
            List<TradeCapture> tradeCaptures,
            List<FxTrade> fxTrades,
            AuditContext auditContext,
            TradeAddDetails tradeAddDetails)
        {
            ThrowInvalidOperationException("CreateTrades");
            return null;
        }

        public List<int> AddTransferTrades(
            int? buyBookId,
            int? sellBookId,
            List<TradeCapture> tradeCaptures,
            string userName,
            AuditContext auditContext,
            int? groupId = null,
            List<int> tradeIdsForCancel = null,
            string editCancelReason = null,
            TradeAddDetails tradeAddDetails = null)
        {
            ThrowInvalidOperationException("AddTransferTrades");
            return null;
        }

        public List<TradeCapture> ReadParentTradeCaptures(
            Expression<Func<TradeCapture, bool>> portfoliosConstraint,
            List<int> sdIds,
            List<int> tradesIds,
            DateTime? tillDatetime,
            DateTime? fromDatetime)
        {
            ThrowInvalidOperationException("ReadParentTradeCaptures");
            return null;
        }

        public void InsertTradeCaptures(
            List<TradeCapture> tradeCaptures,
            List<FxTrade> fxTrades,
            AuditContext auditContext,
            int? portfolioId = null,
            int? groupId = null,
            List<int> tradeCapturesToCancel = null,
            string editCancelReason = null,
            TradeAddDetails tradeAddDetails = null)
        {
            ThrowInvalidOperationException("InsertTradeCaptures");
        }

        public (bool cancellationsCreated, List<TradeCapture> cancelledTrades) CancelTradeCaptures(
            MandaraEntities cxt,
            HashSet<int> tradeCaptures,
            string editCancelReason,
            TradeGroup tradeGroup = null)
        {
            ThrowInvalidOperationException("CancelTradeCaptures");
            return (false, new List<TradeCapture>());
        }

        public (bool cancellationsCreated, List<TradeCapture> cancelledTrades) CancelTradeCaptures(
            AuditContext auditContext,
            MandaraEntities cxt,
            HashSet<int> tradeCaptures,
            string editCancelReason,
            TradeGroup tradeGroup = null)
        {
            ThrowInvalidOperationException("CancelTradeCaptures");
            return (false, new List<TradeCapture>());
        }

        public List<TradePieces> AddNonFxTradesToContext(
            MandaraEntities cxt,
            List<TradeCapture> tradeCaptures,
            Portfolio portfolio,
            Portfolio sellPortfolio,
            Portfolio buyPortfolio)
        {
            ThrowInvalidOperationException("AddTradeCapturesToContext");
            // This line is never reached because the method above throws an exception.  However, the compiler is unable
            // to tell and insists on a return value.
            return new List<TradePieces>();
        }

        public void AssignTrades(
            AuditContext auditContext,
            int toPortfolioId,
            string toPortfolioName,
            Dictionary<string, Tuple<List<DateTime?>, List<string>>> execIdsOfTradesToAssign,
            string userName)
        {
            throw new NotImplementedException("This method is not implemented on the LegTradesRepository");
        }

        public void CalculatePreCalcDetails(MandaraEntities cxt, List<TradePieces> trades)
        {
            throw new NotImplementedException();
        }

        public List<SdQuantityModel> ReadAllQuantities(MandaraEntities productsDb, DateRange dateRange, List<int> portfolioIds)
        {
            throw new NotImplementedException();
        }

        public List<SdQuantityModel> ReadNonFuturesQuantities(
            MandaraEntities productsDb,
            DateRange dateRange,
            List<int> portfolioIds)
        {
            throw new NotImplementedException();
        }
    }
}