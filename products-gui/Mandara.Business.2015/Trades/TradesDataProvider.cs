using Mandara.Business.Contracts;
using Mandara.Business.Dates;
using Mandara.Business.Services;
using Mandara.Date;
using Mandara.Entities;
using Mandara.Extensions.Option;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Mandara.Business.Trades
{
    public class TradesDataProvider : ITradesDataProvider
    {
        private readonly ISecurityDefinitionsStorage _securityDefinitionsStorage;
        private readonly IProductsStorage _productsStorage;
        private readonly IEndOfDayDateTimeProvider _eodDateTimes;
        private readonly ICalcPnlFromLegsParentReplacer _calcPnlFromLegsParentReplacer;

        public TradesDataProvider(
            ISecurityDefinitionsStorage securityDefinitionsStorage,
            IProductsStorage productsStorage,
            IEndOfDayDateTimeProvider eodDateTimes,
            ICalcPnlFromLegsParentReplacer calcPnlFromLegsParentReplacer)
        {
            _securityDefinitionsStorage = securityDefinitionsStorage;
            _productsStorage = productsStorage;
            _eodDateTimes = eodDateTimes;
            _calcPnlFromLegsParentReplacer = calcPnlFromLegsParentReplacer;
        }

        public List<TradeCapture> GetFilledTrades(DateTime startDate, DateTime endDate)
        {
            return _calcPnlFromLegsParentReplacer.GetLegsAsParents(
                SetSecDefAndProduct(ReadFilledTrades(startDate, endDate)));
        }

        private List<TradeCapture> ReadFilledTrades(DateTime startDate, DateTime endDate)
        {
            using (MandaraEntities cxt = new MandaraEntities(
                MandaraEntities.DefaultConnStrName,
                nameof(TradesDataProvider)))
            {
                IQueryable<TradeCapture> tradesQuery =
                    TradesQueryProvider.GetFilledNonFxTradesQuery(cxt)
                                       .WithBooks()
                                       .Include(tc => tc.TradeGroup)
                                       .Include(tc => tc.PrecalcDetails);

                tradesQuery = TradesQueryProvider.ApplyDateRangeConstraints(
                    new DateRange(startDate, endDate),
                    _eodDateTimes,
                    tradesQuery,
                    _productsStorage);

                return tradesQuery.ToList();
            }
        }

        private List<TradeCapture> SetSecDefAndProduct(List<TradeCapture> tradeCaptures)
        {
            foreach (TradeCapture tradeCapture in tradeCaptures)
            {
                TryGetResult<SecurityDefinition> secDef =
                    _securityDefinitionsStorage.TryGetSecurityDefinition(tradeCapture.SecurityDefinitionId);

                if (!secDef.HasValue)
                {
                    continue;
                }

                tradeCapture.SecurityDefinition = secDef.Value;

                if (secDef.Value.Product != null)
                {
                    continue;
                }

                TryGetResult<Product> product = _productsStorage.TryGetProduct(secDef.Value.product_id.Value);
                if (product.HasValue)
                {
                    tradeCapture.SecurityDefinition.Product = product.Value;
                }
            }

            return tradeCaptures;
        }

        public List<TradeCapture> GetFilledTrades(DateTime startDate, DateTime endDate, List<int> portfolioIds)
        {
            return _calcPnlFromLegsParentReplacer.GetLegsAsParents(
                SetSecDefAndProduct(ReadFilledTrades(startDate, endDate, portfolioIds)));
        }

        private List<TradeCapture> ReadFilledTrades(DateTime startDate, DateTime endDate, List<int> portfolioIds)
        {
            using (MandaraEntities cxt = new MandaraEntities(
                MandaraEntities.DefaultConnStrName,
                nameof(TradesDataProvider)))
            {
                IQueryable<TradeCapture> tradesQuery = TradesQueryProvider
                                                       .GetFilledNonFxTradesQuery(cxt)
                                                       .WithBooks()
                                                       .Include(tc => tc.TradeGroup)
                                                       .Include(tc => tc.PrecalcDetails);

                return TradesQueryProvider.ApplyPortfolioConstraints(
                                              portfolioIds,
                                              TradesQueryProvider.ApplyDateRangeConstraints(
                                                  new DateRange(startDate, endDate),
                                                  _eodDateTimes,
                                                  tradesQuery,
                                                  _productsStorage))
                                          .ToList();
            }
        }
    }
}