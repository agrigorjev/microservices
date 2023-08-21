using Mandara.Business.Contracts;
using Mandara.Business.Dates;
using Mandara.Business.Managers;
using Mandara.Date;
using Mandara.Entities;
using Mandara.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Mandara.Business.Services
{
    public class TradesQueryProvider
    {
        public static int?[] sdVars =
        {
            (int)ProductDateType.MonthYear,
            (int)ProductDateType.Quarter,
            (int)ProductDateType.Year
        };

        private static readonly Expression<Func<TradeCapture, bool>> _legsOrSingleParentsFunc =
            tc =>
                ((tc.Exchange == Exchange.IceExchangeName) && tc.IsParentTrade.HasValue && tc.IsParentTrade.Value
                  && (!(tc.LegRefID == null ^ tc.ClOrdID == null)))
                || (tc.IsParentTrade.HasValue && !tc.IsParentTrade.Value) || (tc.Exchange != Exchange.IceExchangeName);

        public static IQueryable<TradeCapture> GetSecDefBasedParentTradesQuery(
            MandaraEntities databaseContext,
            DateRange dateRange,
            IEndOfDayDateTimeProvider endOfDayDateTimes,
            IProductsStorage productStorage)
        {
            return GetSecDefBasedTradesQuery(
                databaseContext,
                tc => tc.IsParentTrade == true,
                dateRange,
                endOfDayDateTimes,
                productStorage,
                new List<int>());
        }

        public static IQueryable<TradeCapture> GetSecDefBasedOnlyLegsTradesQuery(
            MandaraEntities databaseContext,
            IEndOfDayDateTimeProvider endOfDayDateTimes,
            IProductsStorage productStorage)
        {
            return GetSecDefBasedTradesQuery(
                databaseContext,
                tc => tc.IsParentTrade == false,
                DateRange.Default,
                endOfDayDateTimes,
                productStorage,
                new List<int>());
        }

        public static IQueryable<TradeCapture> GetSecDefBasedLegsOrSingleParentsTradesQuery(
            MandaraEntities databaseContext,
            DateRange dateRange,
            IEndOfDayDateTimeProvider endOfDayDateTimes,
            IProductsStorage productsStorage)
        {
            return GetSecDefBasedLegsOrSingleParentsTradesQuery(
                databaseContext,
                dateRange,
                endOfDayDateTimes,
                productsStorage,
                new List<int>());
        }

        public static IQueryable<TradeCapture> GetSecDefBasedLegsOrSingleParentsTradesQuery(
            MandaraEntities databaseContext,
            DateRange dateRange,
            IEndOfDayDateTimeProvider endOfDayDateTimes,
            IProductsStorage productsStorage,
            List<int> portfolioIds)
        {
            return GetSecDefBasedTradesQuery(
                databaseContext,
                _legsOrSingleParentsFunc,
                dateRange,
                endOfDayDateTimes,
                productsStorage,
                portfolioIds);
        }

        public static IQueryable<TradeCapture> GetBaseQuery(MandaraEntities databaseContext)
        {
            return
                databaseContext.TradeCaptures
                               .Include(trade => trade.SecurityDefinition)
                               .Include(trade => trade.SecurityDefinition.Product)
                               .Include(trade => trade.SecurityDefinition.Product.Exchange);
        }

        public static IQueryable<TradeCapture> GetBaseQueryWithoutFx(MandaraEntities databaseContext)
        {
            return
                GetBaseQuery(databaseContext)
                    .Where(trade => trade.SecurityDefinitionId != SecurityDefinitionsManager.FxSecDefId);
        }

        public static IQueryable<TradeCapture> GetFilledNonFxTradesQuery(MandaraEntities databaseContext)
        {
            return GetBaseQueryWithoutFx(databaseContext).Where(trade => trade.OrdStatus == TradeOrderStatus.Filled);
        }

        private static IQueryable<TradeCapture> GetSecDefBasedTradesQuery(
            MandaraEntities databaseContext,
            Expression<Func<TradeCapture, bool>> parentOrLegsFunc,
            DateRange dateRange,
            IEndOfDayDateTimeProvider endOfDayDateTimes,
            IProductsStorage productsStorage,
            List<int> portfolioIds)
        {
            IQueryable<TradeCapture> sdQuery = GetFilledNonFxTradesQuery(databaseContext)
                .Where(parentOrLegsFunc)
                .Where(
                    tc => sdVars.Contains(tc.SecurityDefinition.Strip1DateTypeDb)
                          && ((tc.SecurityDefinition.Strip2DateTypeDb == null)
                              || sdVars.Contains(tc.SecurityDefinition.Strip2DateTypeDb)));

            sdQuery = ApplyPortfoliosConstraint(sdQuery, portfolioIds);
            sdQuery = ApplyDateRangeConstraints(dateRange, endOfDayDateTimes, sdQuery, productsStorage);
            return sdQuery;
        }

        private static IQueryable<TradeCapture> ApplyPortfoliosConstraint(IQueryable<TradeCapture> sdQuery, List<int> portfolioIds)
        {
            if (portfolioIds.Any())
            {
                sdQuery = sdQuery.Where(tc => portfolioIds.Contains(tc.PortfolioId.Value));
            }
            return sdQuery;
        }

        private static Expression<Func<TradeCapture, bool>> GetIsAfterStartOrNymexTasOnHolidayExpression(
            DateRange eodUtcDateRange,
            List<int> nymexTasesWithPrevDayHolidays)
        {
            return
                (tc) =>
                    eodUtcDateRange.Start <= tc.UtcTransactTime
                    || (tc.UtcTransactTime >= eodUtcDateRange.Start.Date
                        && nymexTasesWithPrevDayHolidays.Contains(tc.SecurityDefinition.Product.ProductId));
        }

        public static IQueryable<TradeCapture> ApplyDateRangeConstraints(
            DateRange dateRange,
            IEndOfDayDateTimeProvider endOfDayDateTimes,
            IQueryable<TradeCapture> query,
            IProductsStorage productStorage)
        {
            DateRange eodUtcDateRange = endOfDayDateTimes.GetUtcDateRangeAccordingToEodTimes(dateRange);

            if (eodUtcDateRange.HasStartDate())
            {
                List<int> nymexTasesProductIdsWithPrevDayHolidays =
                    productStorage.GetNymexTasProductIdsWithGivenHoliday(eodUtcDateRange.Start.Date);

                Expression<Func<TradeCapture, bool>> isAfterStartOrNymexTasOnHoliday =
                    GetIsAfterStartOrNymexTasOnHolidayExpression(eodUtcDateRange, nymexTasesProductIdsWithPrevDayHolidays);

                query = query.Where(isAfterStartOrNymexTasOnHoliday);
            }

            if (eodUtcDateRange.HasStartDate() || eodUtcDateRange.HasEndDate())
            {
                // Only keep trades which are not expired by the start or the end yet
                // There is multiple use this method with either start or end missing
                // We trying to cater for both usecases here and load not expired trades
                DateTime expiry = (eodUtcDateRange.HasStartDate() ? eodUtcDateRange.Start : eodUtcDateRange.End)
                    .Date;
                query = query.Where(tc => tc.ExpiryDate == null || tc.ExpiryDate >= expiry);

            }

            if (eodUtcDateRange.HasEndDate())
            {
                query = query.Where(tc => tc.UtcTransactTime < eodUtcDateRange.End);
            }

            return query;
        }

        public static IQueryable<TradeCapture> GetCustomPeriodTradesQuery(
            MandaraEntities databaseContext,
            DateRange dateRange,
            IEndOfDayDateTimeProvider endOfDayDateTimes,
            IProductsStorage productsStorage)
        {
            return GetCustomPeriodTradesQueryBase(
                databaseContext,
                tc => true,
                dateRange,
                endOfDayDateTimes,
                productsStorage,
                new List<int>());
        }

        public static IQueryable<TradeCapture> GetCustomPeriodLegsTradesQuery(
            MandaraEntities databaseContext,
            DateRange dateRange,
            IEndOfDayDateTimeProvider endOfDayDateTimes,
            IProductsStorage productsStorage,
            List<int> portfolioIds)
        {
            return GetCustomPeriodTradesQueryBase(
                databaseContext,
                _legsOrSingleParentsFunc,
                dateRange,
                endOfDayDateTimes,
                productsStorage,
                portfolioIds);
        }

        private static IQueryable<TradeCapture> GetCustomPeriodTradesQueryBase(
            MandaraEntities databaseContext,
            Expression<Func<TradeCapture, bool>> parentOrLegsFunc,
            DateRange dateRange,
            IEndOfDayDateTimeProvider endOfDayDateTimes,
            IProductsStorage productsStorage,
            List<int> portfolioIds)
        {
            IQueryable<TradeCapture> tradesQuery = GetFilledNonFxTradesQuery(databaseContext)
                .Include(x => x.PrecalcDetails)
                .Where(parentOrLegsFunc)
                .Where(tc => tc.PrecalcDetails.Any());

            tradesQuery = ApplyPortfoliosConstraint(tradesQuery, portfolioIds);

            tradesQuery = ApplyDateRangeConstraints(dateRange, endOfDayDateTimes, tradesQuery, productsStorage);

            return tradesQuery;
        }

        public static IQueryable<TradeCapture> GetTasTradesForDayQuery(
            MandaraEntities databaseContext,
            DateTime riskDate,
            DateTime? cutOffDateTime,
            IEndOfDayDateTimeProvider endOfDayDateTimes,
            IProductsStorage productsStorage)
        {
            return GetTasTradesForDayQuery(
                databaseContext,
                riskDate,
                cutOffDateTime,
                endOfDayDateTimes,
                productsStorage,
                new List<int>());
        }

        public static IQueryable<TradeCapture> GetTasTradesForDayQuery(
            MandaraEntities databaseContext,
            DateTime riskDate,
            DateTime? cutOffDateTime,
            IEndOfDayDateTimeProvider endOfDayDateTimes,
            IProductsStorage productsStorage,
            List<int> portfolioIds)
        {
            if (cutOffDateTime.HasValue && cutOffDateTime < riskDate)
            {
                riskDate = cutOffDateTime.Value;
            }

            DateTime dayAfterRiskDate = riskDate.Date.AddDays(1);
            DateTime endDate = (cutOffDateTime != null && cutOffDateTime < dayAfterRiskDate)
                ? cutOffDateTime.Value
                : dayAfterRiskDate;
            DateRange dateRange = new DateRange(riskDate.Date, endDate);

            IQueryable<TradeCapture> query = GetFilledNonFxTradesQuery(databaseContext);

            query = ApplyPortfoliosConstraint(query, portfolioIds);

            query = ApplyDateRangeConstraints(dateRange, endOfDayDateTimes, query, productsStorage);

            return query.Where(
                tc => (tc.IsParentTrade == true) && ((tc.SecurityDefinition.Product.IsTasDb == true)
                                                     || (tc.SecurityDefinition.Product.IsMopsDb == true)
                                                     || (tc.SecurityDefinition.Product.IsMmDb == true)));
        }

        public static IQueryable<TradeCapture> ApplyPortfolioConstraints(
            List<int> portfolioIds,
            IQueryable<TradeCapture> query)
        {
            Expression<Func<TradeCapture, bool>> isInTargetedPortfolio =
            (trade) =>
                trade.PortfolioId.HasValue && portfolioIds.Contains(trade.PortfolioId.Value);

            return query.Where(isInTargetedPortfolio);
        }
    }
}