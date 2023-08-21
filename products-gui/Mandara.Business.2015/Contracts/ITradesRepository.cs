using Mandara.Business.Audit;
using Mandara.Business.Model;
using Mandara.Business.TradeAdd;
using Mandara.Date;
using Mandara.Entities;
using Mandara.Entities.EntityPieces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mandara.Business.Contracts
{
    public class TradeReadFilterParameters
    {
        public List<int> PortfolioIds { get; set; }
        public DateTime? FromTransactTime { get; set; }
        public DateTime? TillTransactTime { get; set; }
        public DateTime? RiskDate { get; set; }
        public bool SetTransaction { get; set; }
        public bool ReadTimeSpreadLegs { get; set; }

        public TradeReadFilterParameters()
        {
            PortfolioIds = new List<int>();
            FromTransactTime = null;
            TillTransactTime = null;
            RiskDate = null;
            SetTransaction = false;
            ReadTimeSpreadLegs = false;
        }
    }

    /// <summary>
    /// Provides methods to read / write TradeCaptures entities in different ways.
    /// ReadQuantities - gets an aggregated trades view we use to calculate positions duing start up or rolloff.
    /// Other methods works directly with TradeCapture entity, and are used to create new manual/transfer
    /// trades, assign/cancel trades, or read trades based on different criterias (e.g. from the same trade group,
    /// from a datetimes period, by trade identifier).
    /// </summary>
    public interface ITradesRepository
    {
        TradesQuantityData ReadQuantities(TradeReadFilterParameters filterParams);

        //TradesQuantityData ReadQuantities(
        //    DateTime? fromTransactTime = null,
        //    DateTime? tillTransactTime = null,
        //    DateTime? riskDate = null,
        //    bool setTransaction = false,
        //    bool readTimeSpreadLegs = false);

        /// <summary>
        /// Inserts manual or transfer trades (trade captures and FX trades if provided) in a database.
        /// If added trades are belong to an existing trade group, all the existing filled trades
        /// from a group will be cancelled first.
        /// An audit is written for every operation (cancellation/creation) on trades.
        /// </summary>
        /// <param name="tradeCaptures">Trade captures to add.</param>
        /// <param name="fxTrades">FX trades to add.</param>
        /// <param name="auditContext">Parameters for trades audit entries.</param>
        /// <param name="tradeAddDetails">Original TradeAddDetails entry based on which trade captures
        /// and FX trades were created, contains the metadata to perform the operation.</param>
        /// <returns>List of identifiers of just inserted trade captures.</returns>
        List<int> CreateTrades(
            List<TradeCapture> tradeCaptures,
            List<FxTrade> fxTrades,
            AuditContext auditContext,
            TradeAddDetails tradeAddDetails);

        void InsertTradeCaptures(
            List<TradeCapture> tradeCaptures,
            List<FxTrade> fxTrades,
            AuditContext auditContext,
            int? portfolioId = null,
            int? groupId = null,
            List<int> tradeCapturesToCancel = null,
            string editCancelReason = null,
            TradeAddDetails tradeAddDetails = null);

        (bool cancellationsCreated, List<TradeCapture> cancelledTrades) CancelTradeCaptures(
            MandaraEntities productsDb,
            HashSet<int> tradeCaptures,
            string editCancelReason,
            TradeGroup tradeGroup = null);

        (bool cancellationsCreated, List<TradeCapture> cancelledTrades) CancelTradeCaptures(
            AuditContext auditContext,
            MandaraEntities productsDb,
            HashSet<int> tradeCaptures,
            string editCancelReason,
            TradeGroup tradeGroup = null);

        List<TradePieces> AddNonFxTradesToContext(
            MandaraEntities cxt,
            List<TradeCapture> tradeCaptures,
            Portfolio portfolio,
            Portfolio sellPortfolio,
            Portfolio buyPortfolio);

        List<TradeCapture> GetTradesWithSameGroup(int groupId, string ordStatus = null);

        List<TradeCapture> GetFullSpreadTrades(TradeCapture spreadTrade);
        TradeCapture GetTradeWithSdById(int tradeId);
        List<TradeCapture> GetTradesById(List<int> tradesIds);

        void AssignTrades(
            AuditContext auditContext,
            int toPortfolioId,
            string toPortfolioName,
            Dictionary<string, Tuple<List<DateTime?>, List<string>>> execIdsOfTradesToAssign,
            string userName);

        List<int> AddTransferTrades(
            int? buyBookId,
            int? sellBookId,
            List<TradeCapture> tradeCaptures,
            string userName,
            AuditContext auditContext,
            int? groupId,
            List<int> tradeIdsForCancel,
            string editCancelReason,
            TradeAddDetails tradeAddDetails = null);

        /// <summary>
        /// Gets trade captures that match a portfolio defined by portfoliosConstraint, whose security definition
        /// identifier is in the set defined by sdIds or trade identifier is in the set defined by tradesIds,
        /// and all trades TransactTimes (taking into account trade's exchange timezone) are within datetime period
        /// between fromDatetime and tillDatetime.
        /// </summary>
        /// <param name="portfoliosConstraint">Constraint against trades portfolios.</param>
        /// <param name="sdIds">Security Definitions identifiers whose trades should be included into output, if
        /// they match a portfolio defined by portfoliosConstraint and fill within datetime period defined by
        /// fromDatetime and tillDatetime.</param>
        /// <param name="tradesIds">Trades identifiers which should be included into output, if they match a portfolio
        /// defined by portfoliosConstraint and fill within datetime period defined by fromDatetime and tillDatetime.</param>
        /// <param name="tillDatetime">An end day till which trades are needed.</param>
        /// <param name="fromDatetime">A start day from when trades are needed.</param>
        /// <returns>A list of trades.</returns>
        List<TradeCapture> ReadParentTradeCaptures(
            Expression<Func<TradeCapture, bool>> portfoliosConstraint,
            List<int> sdIds,
            List<int> tradesIds,
            DateTime? tillDatetime,
            DateTime? fromDatetime);

        void CalculatePreCalcDetails(MandaraEntities cxt, List<TradePieces> trades);

        List<SdQuantityModel> ReadAllQuantities(MandaraEntities productsDb, DateRange dateRange, List<int> portfolioIds);

        List<SdQuantityModel> ReadNonFuturesQuantities(
            MandaraEntities productsDb,
            DateRange dateRange,
            List<int> portfolioIds);
    }
}