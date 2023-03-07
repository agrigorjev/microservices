using Mandara.Business;
using Mandara.Business.Calculators;
using Mandara.Business.Configuration;
using Mandara.Business.Contracts;
using Mandara.Business.Managers;
using Mandara.Business.OldCode;
using Mandara.Database.Query;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Entities.Enums;
using Mandara.Extensions.Option;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Mandara.ProductGUI
{
    public class PrecalcDetailRecalculator
    {
        private const int LoadSizePerRound = 1000;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private List<Product> _products;
        private int _precalcSourceDetailsMonthsBack;
        private BackgroundWorker _recalcWorker;

        private readonly Expression<Func<TradeCapture, bool>> _isNonFxTrade =
            (trade) => trade.SecurityDefinitionId != SecurityDefinitionsManager.FxSecDefId;

        private readonly Expression<Func<TradeCapture, bool>> _isCustomPeriodTrade =
            (trade) => trade.PrecalcDetails.Any();

        private readonly Expression<Func<TradeCapture, bool>> _isMonthlyTrade = (trade) => !trade.PrecalcDetails.Any();

        private readonly Expression<Func<TradeCapture, bool>> _isManualTrade = (trade) => null != trade.TradeType;

        private readonly Expression<Func<SecurityDefinition, bool>> _isNonFxSecurityDef =
            (secDef) => secDef.SecurityDefinitionId != SecurityDefinitionsManager.FxSecDefId;

        private readonly Expression<Func<SourceDetail, bool>> _isNonFxSrcDetail =
            (srcDetail) => srcDetail.SecurityDefinitionId != SecurityDefinitionsManager.FxSecDefId;

        private class RecalcPrecalcDetailsInput
        {
            public List<TradeCapture> CustomPeriodTrades { get; }
            public List<TradeCapture> MonthlyTrades { get; }
            public List<SourceDetail> AbnSourceDetails { get; }
            public List<SecurityDefinition> SecurityDefinitions { get; }

            public RecalcPrecalcDetailsInput(
                List<TradeCapture> custPeriodTrades,
                List<TradeCapture> monthlyTrades,
                List<SourceDetail> abnSource,
                List<SecurityDefinition> secDefs)
            {
                CustomPeriodTrades = custPeriodTrades ?? new List<TradeCapture>();
                MonthlyTrades = monthlyTrades ?? new List<TradeCapture>();
                AbnSourceDetails = abnSource ?? new List<SourceDetail>();
                SecurityDefinitions = secDefs ?? new List<SecurityDefinition>();
            }

            public bool HasCustomPeriodTrades()
            {
                return CustomPeriodTrades.Any();
            }

            public bool HasMonthlyTrades()
            {
                return MonthlyTrades.Any();
            }

            public bool HasAbnSourceDetails()
            {
                return AbnSourceDetails.Any();
            }

            public bool HasSecurityDefs()
            {
                return SecurityDefinitions.Any();
            }

            public bool HasData()
            {
                return HasCustomPeriodTrades() || HasMonthlyTrades() || HasAbnSourceDetails() || HasSecurityDefs();
            }

            public int TotalEntities()
            {
                return CustomPeriodTrades.Count
                       + MonthlyTrades.Count
                       + AbnSourceDetails.Count
                       + SecurityDefinitions.Count;
            }
        }

        private struct BaseEntityIds
        {
            public int LastCustomPeriodTrade { get; }
            public int LastMonthlySecDef { get; }
            public int LastSourceDetail { get; }
            public int LastSecurityDef { get; }

            public BaseEntityIds(int lastCustPeriodTrade, int lastMonthlySecDef, int lastSrcDetail, int lastSecDef)
            {
                LastCustomPeriodTrade = lastCustPeriodTrade;
                LastMonthlySecDef = lastMonthlySecDef;
                LastSourceDetail = lastSrcDetail;
                LastSecurityDef = lastSecDef;
            }
        }

        private struct CalculationRound
        {
            public int TotalRounds { get; }
            public int CurrentRound { get; }
            public int Progress { get; }

            public CalculationRound(int totalRnds, int rnd, int progress)
            {
                TotalRounds = totalRnds;
                CurrentRound = rnd;
                Progress = progress;
            }
        }

        public PrecalcDetailRecalculator()
        {
            SetPrecalcSrcDetailMonthsRange();
        }

        private void SetPrecalcSrcDetailMonthsRange()
        {
            int months;

            if (!int.TryParse(ConfigurationManager.AppSettings["PrecalcSourceDetails_MonthsBack"], out months))
            {
                months = 6;
            }

            _precalcSourceDetailsMonthsBack = months;
        }

        public void RecalculatePrecalcPositions(object sender, DoWorkEventArgs e)
        {
            RecalcMode recalcMode = (RecalcMode)e.Argument;
            StringBuilder recalcErrorTrades = new StringBuilder();
            int errorCounter = 0;

            _recalcWorker = (BackgroundWorker)sender;

            if (!FxSecDefIdReader.ReadAndValidateFXSecDefID())
            {
                _recalcWorker.ReportProgress(0, "No Fx Security Definition Defined...");
                return;
            }

            _recalcWorker.ReportProgress(0, "Initializing calculators...");

            BaseEntityIds lastEntityIds = new BaseEntityIds(0, 0, 0, 0);
            RecalcPrecalcDetailsInput recalcInput;

            ISecurityDefinitionsStorage securityDefinitionsStorage = IoC.Get<ISecurityDefinitionsStorage>();
            IProductsStorage productsStorage = IoC.Get<IProductsStorage>();

            _recalcWorker.ReportProgress(0, "Loading security definitions...");
            securityDefinitionsStorage.Clean();
            _recalcWorker.ReportProgress(0, "Loading products...");
            productsStorage.Update();

            if (_products == null)
            {
                _products = productsStorage.GetProducts().ToList();
            }

            PricingPrePositionsManager pricingPositionsManager = IoC.Get<PricingPrePositionsManager>();
            PrecalcPositionsCalculator precalcPositionsCalculator =
                new PrecalcPositionsCalculator(productsStorage, pricingPositionsManager);
            DateTime sourceDetailsMinDate = DateTime.Now.AddMonths(-1 * Math.Abs(_precalcSourceDetailsMonthsBack));
            int entitiesProcessed = 0;
            _recalcWorker.ReportProgress(0, "Counting total entities for processing...");
            List<int> entitiesNum = GetTotalEntitiesForRecalc(recalcMode, sourceDetailsMinDate);
            int totalEntitiesToProcess = entitiesNum.Sum();
            int numberOfRounds = entitiesNum.Max() / LoadSizePerRound;
            CalculationRound round = new CalculationRound(numberOfRounds, 1, 0);

            if (0 == totalEntitiesToProcess)
            {
                _recalcWorker.ReportProgress(100, "No data for recalculation");
                return;
            }

            _log.Info("Total entities to process is {0}", totalEntitiesToProcess);
            _recalcWorker.ReportProgress(0, $"Calculating round {round}");

            do
            {
                if (_recalcWorker.CancellationPending)
                {
                    return;
                }

                recalcInput = GetRecalcInput(round, recalcMode, lastEntityIds, sourceDetailsMinDate);
                lastEntityIds = GetCurrentLastEntityIds(recalcInput, lastEntityIds);
                errorCounter = Recalc(
                    round,
                    errorCounter,
                    recalcInput,
                    securityDefinitionsStorage,
                    productsStorage,
                    precalcPositionsCalculator,
                    recalcErrorTrades);

                entitiesProcessed += (recalcInput.TotalEntities());

                round = new CalculationRound(
                    round.TotalRounds,
                    round.CurrentRound + 1,
                    Math.Min(50, entitiesProcessed * 50 / totalEntitiesToProcess));
                _recalcWorker.ReportProgress(round.Progress, $"Calculating round {round.CurrentRound}");
                _log.Info("Processed {0} entities", entitiesProcessed);
            }
            while (recalcInput.HasData());

            _recalcWorker.ReportProgress(50, "Preparing to save positions");

            if (totalEntitiesToProcess != entitiesProcessed)
            {
                _log.Warn(
                    "Expected to process {0} entities, but processed {1}",
                    totalEntitiesToProcess,
                    entitiesProcessed);
            }

            if (!SavePrecalcDetails(precalcPositionsCalculator, _recalcWorker))
            {
                _log.Warn("RecalcPrecalcPositions: Saving recalculated precalc details did not complete.");
                return;
            }

            if (RecalcMode.ChangedProducts == recalcMode || RecalcMode.AllData == recalcMode)
            {
                using (MandaraEntities dbContext = new MandaraEntities())
                {
                    _recalcWorker.ReportProgress(99, "Finalizing...");
                    dbContext.Products.Where(p => p.IsChanged).ToList().ForEach(p => { p.IsChanged = false; });
                    dbContext.SaveChanges();
                }
            }

            ReportRecalcComplete(errorCounter, recalcErrorTrades, _recalcWorker);
        }

        private List<int> GetTotalEntitiesForRecalc(RecalcMode recalcMode, DateTime sourceDetailsMinDate)
        {
            List<int> entitiesNum;

            switch (recalcMode)
            {
                case RecalcMode.ChangedProducts:
                {
                    entitiesNum = CountChangedForRecalc(sourceDetailsMinDate);
                }
                break;

                case RecalcMode.ManualTrades:
                {
                    entitiesNum = CountManualTradesForRecalc();
                }
                break;

                case RecalcMode.NewSourceDetails:
                {
                    entitiesNum = CountNewAbnSrcDetailsForRecalc(sourceDetailsMinDate);
                }
                break;

                default:
                {
                    entitiesNum = CountAllForRecalc(sourceDetailsMinDate);
                }
                break;
            }

            return entitiesNum;
        }

        private List<int> CountChangedForRecalc(DateTime sourceDetailsMinDate)
        {
            _recalcWorker.ReportProgress(0, "Counting custom period trades for recalculation");

            int customPeriodCnt = (int)SqlServerCommandExecution.ExecuteScalarQuery(
                "MandaraEntities",
                @"SELECT 
                    COUNT(1) AS [A1]
                    FROM [dbo].[trade_captures] AS [trades]
                    WHERE ([trades].[idSecurityDefinition] <> 84277) AND ( EXISTS (SELECT 
                        1 AS [C1]
                        FROM [dbo].[precalc_details_tc] AS [precalcs]
                        WHERE [trades].[idTradeCapture] = [precalcs].[TradeCaptureId]
                    )) AND ( EXISTS (SELECT 
                        1 AS [C1]
                        FROM  [dbo].[products] AS [prods]
                        INNER JOIN [dbo].[security_definitions] AS [secDefs] 
                            ON [prods].[product_id] = [secDefs].[product_id]
                        WHERE ([trades].[idSecurityDefinition] = [secDefs].[idSecurityDefinition]) 
                            AND ([prods].[is_changed] = 1)
                    ))");

            using (MandaraEntities dbContext = new MandaraEntities())
            {
                dbContext.Database.CommandTimeout = 0;

                if (_recalcWorker.CancellationPending)
                {
                    return new List<int>() { 0 };
                }

                _recalcWorker.ReportProgress(0, "Counting monthly trades for recalculation");

                int monthlyCnt = dbContext.TradeCaptures.AsNoTracking()
                                          .Where(_isNonFxTrade)
                                          .Where(_isMonthlyTrade)
                                          .Where(
                                              trade => dbContext
                                                           .Products.Where(p => p.IsChanged)
                                                           .Select(p => p.ProductId)
                                                           .Contains(trade.SecurityDefinition.product_id.Value))
                                          .Select(trade => trade.SecurityDefinitionId)
                                          .Distinct()
                                          .Count();

                if (_recalcWorker.CancellationPending)
                {
                    return new List<int>() { 0 };
                }

                _recalcWorker.ReportProgress(0, "Counting ABN-sourced trades for recalculation");

                int srcDetailCnt = dbContext.SourceDetails.AsNoTracking()
                                            .Count(
                                                srcDetail => dbContext.Products.Where(p => p.IsChanged)
                                                                      .Select(p => p.ProductId)
                                                                      .Contains(srcDetail.product_id)
                                                             && srcDetail.SourceData.Date > sourceDetailsMinDate);

                if (_recalcWorker.CancellationPending)
                {
                    return new List<int>() { 0 };
                }

                _recalcWorker.ReportProgress(0, "Counting security definitions with no trades for recalculation");

                int secDefsCnt = dbContext.SecurityDefinitions.AsNoTracking()
                                          .Where(_isNonFxSecurityDef)
                                          .Count(
                                              secDef => secDef.TradeCaptures.Count == 0
                                                        && dbContext
                                                            .Products.Where(p => p.IsChanged)
                                                            .Select(p => p.ProductId)
                                                            .Contains(secDef.product_id.Value));
                return new List<int>() { customPeriodCnt, monthlyCnt, srcDetailCnt, secDefsCnt };
            }
        }

        private List<int> CountManualTradesForRecalc()
        {
            _recalcWorker.ReportProgress(0, "Counting manual trades for recalculation");

            using (MandaraEntities dbContext = new MandaraEntities())
            {
                dbContext.Database.CommandTimeout = 0;

                return new List<int>()
                {
                    dbContext.TradeCaptures.Where(tc => tc.TradeType != null).AsNoTracking().Count()
                };
            }
        }

        private List<int> CountNewAbnSrcDetailsForRecalc(DateTime sourceDetailsMinDate)
        {
            _recalcWorker.ReportProgress(0, "Counting ABN-sourced trades for recalculation");

            using (MandaraEntities dbContext = new MandaraEntities())
            {
                dbContext.Database.CommandTimeout = 0;

                return new List<int>()
                {
                    dbContext.SourceDetails.AsNoTracking()
                             .Count(
                                 sd => sd.SourceData.Date > sourceDetailsMinDate
                                       && sd.SecurityDefinition != null
                                       && sd.PrecalcDetails.Count == 0
                                       && sd.SecurityDefinition.PrecalcDetails.Count == 0)
                };
            }
        }

        private List<int> CountAllForRecalc(DateTime sourceDetailsMinDate)
        {
            using (MandaraEntities dbContext = new MandaraEntities())
            {
                dbContext.Database.CommandTimeout = 0;

                _recalcWorker.ReportProgress(0, "Counting custom period trades for recalculation");

                int customPeriodCnt = dbContext.TradeCaptures.AsNoTracking().Count(trade => trade.PrecalcDetails.Any());

                if (_recalcWorker.CancellationPending)
                {
                    return new List<int>() { 0 };
                }

                _recalcWorker.ReportProgress(0, "Counting monthly trades for recalculation");

                int monthlyCnt = dbContext.TradeCaptures.AsNoTracking()
                                          .Where(_isNonFxTrade)
                                          .Where(_isMonthlyTrade)
                                          .Select(trade => trade.SecurityDefinitionId)
                                          .Distinct()
                                          .Count();

                if (_recalcWorker.CancellationPending)
                {
                    return new List<int>() { 0 };
                }

                _recalcWorker.ReportProgress(0, "Counting ABN-sourced trades for recalculation");

                int srcDetailCnt = dbContext.SourceDetails
                                        .Where(srcDetail => srcDetail.SourceData.Date > sourceDetailsMinDate)
                                        .AsNoTracking()
                                        .Count();

                if (_recalcWorker.CancellationPending)
                {
                    return new List<int>() { 0 };
                }

                _recalcWorker.ReportProgress(0, "Counting security definitions with no trades for recalculation");

                int secDefsCnt = dbContext.SecurityDefinitions.AsNoTracking()
                                          .Count(secDef => secDef.TradeCaptures.Count == 0);

                return new List<int>() { customPeriodCnt, monthlyCnt, srcDetailCnt, secDefsCnt };
            }
        }

        private RecalcPrecalcDetailsInput GetRecalcInput(
            CalculationRound progress,
            RecalcMode recalcMode,
            BaseEntityIds lastEntityIds,
            DateTime sourceDetailsMinDate)
        {
            RecalcPrecalcDetailsInput recalcInput;

            using (MandaraEntities dbContext = new MandaraEntities())
            {
                dbContext.Database.CommandTimeout = 0;

                if (recalcMode == RecalcMode.ChangedProducts)
                {
                    recalcInput = GetDataForRecalcChanged(progress, dbContext, lastEntityIds, sourceDetailsMinDate);
                }
                else if (recalcMode == RecalcMode.ManualTrades)
                {
                    recalcInput = GetDataForRecalcManualTrades(progress, dbContext, lastEntityIds);
                }
                else if (recalcMode == RecalcMode.NewSourceDetails)
                {
                    recalcInput = GetDataForCalcNewPrecalc(
                        progress,
                        dbContext,
                        lastEntityIds.LastSourceDetail,
                        sourceDetailsMinDate);
                }
                else
                {
                    recalcInput = GetDataForRecalcAll(progress, dbContext, lastEntityIds, sourceDetailsMinDate);
                }
            }
            return recalcInput;
        }

        public class TradeSecurityDefinitionComparer : IEqualityComparer<TradeCapture>
        {
            public bool Equals(TradeCapture lhs, TradeCapture rhs)
            {
                return lhs?.SecurityDefinitionId == rhs?.SecurityDefinitionId;
            }

            public int GetHashCode(TradeCapture trade)
            {
                return trade.SecurityDefinitionId;
            }
        }

        private RecalcPrecalcDetailsInput GetDataForRecalcChanged(
            CalculationRound progress,
            MandaraEntities dbContext,
            BaseEntityIds lastEntityIds,
            DateTime sourceDetailsMinDate)
        {
            _recalcWorker.ReportProgress(
                progress.Progress,
                $"{progress.CurrentRound} of {progress.TotalRounds + 1}: Loading custom period trades");

            List<TradeCapture> customPeriodTrades = dbContext
                .TradeCaptures.AsNoTracking()
                .Where(_isNonFxTrade)
                .Where(_isCustomPeriodTrade)
                .Where(
                    trade => dbContext.Products.Where(p => p.IsChanged)
                                      .Select(p => p.ProductId)
                                      .Contains(trade.SecurityDefinition.product_id.Value)
                             && trade.TradeId > lastEntityIds.LastCustomPeriodTrade)
                .OrderBy(trade => trade.TradeId)
                .Take(LoadSizePerRound)
                .ToList();

            _recalcWorker.ReportProgress(
                progress.Progress,
                $"{progress.CurrentRound} of {progress.TotalRounds + 1}: Loading monthly trades");

            List<TradeCapture> monthlyTrades = dbContext.TradeCaptures.AsNoTracking()
                                                        .Where(_isNonFxTrade)
                                                        .Where(_isMonthlyTrade)
                                                        .Where(
                                                            trade => trade.SecurityDefinitionId
                                                                     > lastEntityIds.LastMonthlySecDef
                                                                     && dbContext
                                                                         .Products.Where(p => p.IsChanged)
                                                                         .Select(p => p.ProductId)
                                                                         .Contains(
                                                                             trade
                                                                                 .SecurityDefinition.product_id
                                                                                 .Value))
                                                        .OrderBy(trade => trade.SecurityDefinitionId)
                                                        .GroupBy(trade => trade.SecurityDefinitionId)
                                                        .OrderBy(grp => grp.Key)
                                                        .Take(LoadSizePerRound)
                                                        .Select(tradeGrp => tradeGrp.FirstOrDefault())
                                                        .Where(_isMonthlyTrade)
                                                        .ToList();

            _recalcWorker.ReportProgress(
                progress.Progress,
                $"{progress.CurrentRound} of {progress.TotalRounds + 1}: Loading ABN-sourced trades");

            List<SourceDetail> sourceDetails = dbContext.SourceDetails.AsNoTracking()
                                                        .Where(
                                                            srcDetail => dbContext.Products.Where(p => p.IsChanged)
                                                                                  .Select(p => p.ProductId)
                                                                                  .Contains(srcDetail.product_id)
                                                                         && srcDetail.SourceDetailId
                                                                            > lastEntityIds.LastSourceDetail
                                                                         && srcDetail.SourceData.Date
                                                                            > sourceDetailsMinDate
                                                                         && srcDetail.SecurityDefinition != null)
                                                        .OrderBy(sd => sd.SourceDetailId)
                                                        .Take(LoadSizePerRound)
                                                        .ToList();

            _recalcWorker.ReportProgress(
                progress.Progress,
                $"{progress.CurrentRound} of {progress.TotalRounds + 1}: Loading security definitions");

            List<SecurityDefinition> secDefs = dbContext.SecurityDefinitions.AsNoTracking()
                                                        .Where(_isNonFxSecurityDef)
                                                        .Where(
                                                            secDef => secDef.TradeCaptures.Count == 0
                                                                      && dbContext
                                                                          .Products.Where(p => p.IsChanged)
                                                                          .Select(p => p.ProductId)
                                                                          .Contains(secDef.product_id.Value)
                                                                      && secDef.SecurityDefinitionId
                                                                        > lastEntityIds.LastSecurityDef)
                                                        .OrderBy(sd => sd.SecurityDefinitionId)
                                                        .Take(LoadSizePerRound)
                                                        .ToList();

            return new RecalcPrecalcDetailsInput(customPeriodTrades, monthlyTrades, sourceDetails, secDefs);
        }

        private RecalcPrecalcDetailsInput GetDataForRecalcManualTrades(
            CalculationRound progress,
            MandaraEntities dbContext,
            BaseEntityIds lastEntityIds)
        {
            _recalcWorker.ReportProgress(
                progress.Progress,
                $"{progress.CurrentRound} of {progress.TotalRounds + 1}: Loading manual trades");

            List<TradeCapture> manualTrades = dbContext.TradeCaptures.AsNoTracking()
                                                       .Where(_isNonFxTrade)
                                                       .Where(_isManualTrade)
                                                       .Where(
                                                           tc => tc.TradeId > lastEntityIds.LastCustomPeriodTrade
                                                                 && tc.PrecalcDetails.Count == 0
                                                                 && tc.SecurityDefinition.PrecalcDetails.Count == 0)
                                                       .OrderBy(tc => tc.TradeId)
                                                       .GroupBy(trade => trade.SecurityDefinitionId)
                                                       .Take(LoadSizePerRound)
                                                       .Select(tradeGrp => tradeGrp.FirstOrDefault())
                                                       .ToList();
            List<TradeCapture> dummyMonthlyTrades = new List<TradeCapture>();
            List<SourceDetail> dummySrcDetails = new List<SourceDetail>();
            List<SecurityDefinition> dummySecDefs = new List<SecurityDefinition>();

            return new RecalcPrecalcDetailsInput(manualTrades, dummyMonthlyTrades, dummySrcDetails, dummySecDefs);
        }

        private RecalcPrecalcDetailsInput GetDataForCalcNewPrecalc(
            CalculationRound progress,
            MandaraEntities dbContext,
            int lastSourceDetailId,
            DateTime sourceDetailsMinDate)
        {
            List<TradeCapture> dummyTrades = new List<TradeCapture>();

            _recalcWorker.ReportProgress(
                progress.Progress,
                $"{progress.CurrentRound} of {progress.TotalRounds + 1}: Loading ABN-sourced trades");

            List<SourceDetail> sourceDetails = dbContext.SourceDetails.AsNoTracking()
                                                        .Where(_isNonFxSrcDetail)
                                                        .Where(
                                                            sd => sd.SourceDetailId > lastSourceDetailId
                                                                  && sd.SourceData.Date > sourceDetailsMinDate
                                                                  && sd.SecurityDefinition != null
                                                                  && sd.PrecalcDetails.Count == 0
                                                                  && sd.SecurityDefinition.PrecalcDetails.Count == 0)
                                                        .OrderBy(sd => sd.SourceDetailId)
                                                        .Take(LoadSizePerRound)
                                                        .ToList();
            List<SecurityDefinition> dummySecDefs = new List<SecurityDefinition>();

            return new RecalcPrecalcDetailsInput(dummyTrades, dummyTrades, sourceDetails, dummySecDefs);
        }

        private RecalcPrecalcDetailsInput GetDataForRecalcAll(
            CalculationRound progress,
            MandaraEntities cxt,
            BaseEntityIds lastEntityIds,
            DateTime sourceDetailsMinDate)
        {
            _recalcWorker.ReportProgress(
                progress.Progress,
                $"{progress.CurrentRound} of {progress.TotalRounds + 1}: Loading custom period trades");

            List<TradeCapture> customPeriodTrades = cxt.TradeCaptures.AsNoTracking()
                                                       .Where(_isNonFxTrade)
                                                       .Where(_isCustomPeriodTrade)
                                                       .Where(
                                                           trade => trade.TradeId
                                                                        > lastEntityIds.LastCustomPeriodTrade)
                                                       .OrderBy(tc => tc.TradeId)
                                                       .Take(LoadSizePerRound)
                                                       .ToList();

            _recalcWorker.ReportProgress(
                progress.Progress,
                $"{progress.CurrentRound} of {progress.TotalRounds + 1}: Loading monthly trades");

            List<TradeCapture> monthlyTrades = cxt.TradeCaptures.AsNoTracking()
                                                  .Where(_isNonFxTrade)
                                                  .Where(_isMonthlyTrade)
                                                  .Where(
                                                      trade => trade.SecurityDefinitionId
                                                               > lastEntityIds.LastMonthlySecDef)
                                                  .OrderBy(tc => tc.SecurityDefinitionId)
                                                  .GroupBy(trade => trade.SecurityDefinitionId)
                                                  .OrderBy(grp => grp.Key)
                                                  .Take(LoadSizePerRound)
                                                  .Select(tradeGrp => tradeGrp.FirstOrDefault())
                                                  .Where(_isMonthlyTrade)
                                                  .ToList();

            _recalcWorker.ReportProgress(
                progress.Progress,
                $"{progress.CurrentRound} of {progress.TotalRounds + 1}: Loading ABN-sourced trades");

            List<SourceDetail> srcDetails = cxt.SourceDetails.AsNoTracking()
                                               .Where(
                                                   sd => sd.SourceDetailId > lastEntityIds.LastSourceDetail
                                                         && sd.SourceData.Date > sourceDetailsMinDate
                                                         && sd.SecurityDefinition != null)
                                               .OrderBy(sd => sd.SourceDetailId)
                                               .Take(LoadSizePerRound)
                                               .ToList();

            _recalcWorker.ReportProgress(
                progress.Progress,
                $"{progress.CurrentRound} of {progress.TotalRounds + 1}: Loading security definitions");

            List<SecurityDefinition> secDefs = cxt.SecurityDefinitions.AsNoTracking()
                                                  .Where(_isNonFxSecurityDef)
                                                  .Where(
                                                      sd => sd.TradeCaptures.Count == 0
                                                            && sd.SecurityDefinitionId > lastEntityIds.LastSecurityDef)
                                                  .OrderBy(sd => sd.SecurityDefinitionId)
                                                  .Take(LoadSizePerRound)
                                                  .ToList();

            return new RecalcPrecalcDetailsInput(customPeriodTrades, monthlyTrades, srcDetails, secDefs);
        }

        private static BaseEntityIds GetCurrentLastEntityIds(
            RecalcPrecalcDetailsInput recalcInput,
            BaseEntityIds lastEntityIds)
        {
            int lastCustomPeriodTradeId = recalcInput.HasCustomPeriodTrades()
                ? recalcInput.CustomPeriodTrades[recalcInput.CustomPeriodTrades.Count - 1].TradeId
                : lastEntityIds.LastCustomPeriodTrade;
            int lastMonthlyTradeId = recalcInput.HasMonthlyTrades()
                ? recalcInput.MonthlyTrades[recalcInput.MonthlyTrades.Count - 1].SecurityDefinitionId
                : lastEntityIds.LastMonthlySecDef;
            int lastSrcDetailId = recalcInput.HasAbnSourceDetails()
                ? recalcInput.AbnSourceDetails[recalcInput.AbnSourceDetails.Count - 1].SourceDetailId
                : lastEntityIds.LastSourceDetail;
            int lastSecDefId = recalcInput.HasSecurityDefs()
                ? recalcInput.SecurityDefinitions[recalcInput.SecurityDefinitions.Count - 1].SecurityDefinitionId
                : lastEntityIds.LastSecurityDef;

            lastEntityIds = new BaseEntityIds(
                lastCustomPeriodTradeId,
                lastMonthlyTradeId,
                lastSrcDetailId,
                lastSecDefId);
            return lastEntityIds;
        }

        private int Recalc(
            CalculationRound progress,
            int errorCounter,
            RecalcPrecalcDetailsInput recalcInput,
            ISecurityDefinitionsStorage secDefStore,
            IProductsStorage productsStore,
            PrecalcPositionsCalculator precalcPositionsCalculator,
            StringBuilder recalcErrorTrades)
        {
            if (recalcInput.HasCustomPeriodTrades())
            {
                _recalcWorker.ReportProgress(
                    progress.Progress,
                    String.Format(
                        "{0} of {1}: Calculating for {2} custom period trades",
                        progress.CurrentRound,
                        progress.TotalRounds + 1,
                        recalcInput.CustomPeriodTrades.Count));
                errorCounter = recalcInput.CustomPeriodTrades.Aggregate(
                    errorCounter,
                    (errCnt, trade) =>
                    {
                        return RecalcForTrade(
                            secDefStore,
                            trade,
                            productsStore,
                            precalcPositionsCalculator,
                            errorCounter,
                            recalcErrorTrades);
                    });
            }

            if (recalcInput.HasMonthlyTrades())
            {
                _recalcWorker.ReportProgress(
                    progress.Progress,
                    String.Format(
                        "{0} of {1}: Calculating for {2} monthly trades",
                        progress.CurrentRound,
                        progress.TotalRounds + 1,
                        recalcInput.MonthlyTrades.Count));
                errorCounter = recalcInput.MonthlyTrades.Aggregate(
                    errorCounter,
                    (errCnt, trade) => RecalcForTrade(
                        secDefStore,
                        trade,
                        productsStore,
                        precalcPositionsCalculator,
                        errorCounter,
                        recalcErrorTrades));
            }

            if (recalcInput.HasAbnSourceDetails())
            {
                _recalcWorker.ReportProgress(
                    progress.Progress,
                    String.Format(
                        "{0} of {1}: Calculating for {2} ABN-sourced trades",
                        progress.CurrentRound,
                        progress.TotalRounds + 1,
                        recalcInput.AbnSourceDetails.Count));
                recalcInput.AbnSourceDetails.ForEach(
                    srcDetail =>
                    {
                        RecalcForAbnSourceDetail(
                            srcDetail,
                            secDefStore,
                            productsStore,
                            precalcPositionsCalculator);
                    });
            }

            if (recalcInput.HasSecurityDefs())
            {
                _recalcWorker.ReportProgress(
                    progress.Progress,
                    String.Format(
                        "{0} of {1}: Calculating for {2} security definitions that have no trades",
                        progress.CurrentRound,
                        progress.TotalRounds + 1,
                        recalcInput.SecurityDefinitions.Count));
                errorCounter = recalcInput.SecurityDefinitions
                                          .Where(secDef => !IsNonMonthlyStrip(secDef, productsStore))
                                          .Aggregate(
                                              errorCounter,
                                              (errCnt, secDef) => RecalcForSecurityDef(
                                                  secDef,
                                                  productsStore,
                                                  precalcPositionsCalculator,
                                                  errCnt,
                                                  recalcErrorTrades));
            }

            return errorCounter;
        }

        private int RecalcForTrade(
            ISecurityDefinitionsStorage securityDefinitionsStorage,
            TradeCapture trade,
            IProductsStorage productsStorage,
            PrecalcPositionsCalculator precalcPositionsCalculator,
            int errorCounter,
            StringBuilder recalcErrorTrades)
        {
            TryGetResult<SecurityDefinition> secDefResult =
                securityDefinitionsStorage.TryGetSecurityDefinition(trade.SecurityDefinitionId);

            if (!secDefResult.HasValue)
            {
                _log.Warn("ProductManager: Could not find security definitions for ID {0}", trade.SecurityDefinitionId);
                return errorCounter;
            }

            // TODO: What if the product is not in the storage?
            SecurityDefinition securityDefinition = secDefResult.Value;
            securityDefinition.Product = GetProductFromSecurityDefinition(securityDefinition, productsStorage);
            trade.SecurityDefinition = securityDefinition;

            try
            {
                precalcPositionsCalculator.ForceCalculatePrecalcPositions(trade, securityDefinition);
                _log.Debug($"Precalc positions calculated for trade id [{trade.TradeId}]");
            }
            catch (NoSourceDetailException noSrcDetail)
            {
                _log.Error(noSrcDetail, $"Error calculating precalc for trade id [{trade.TradeId}]");

                if (errorCounter < 10)
                {
                    recalcErrorTrades.AppendLine($"{trade.TradeId}");
                }

                errorCounter++;
            }

            return errorCounter;
        }

        private Product GetProductFromSecurityDefinition(SecurityDefinition secDef, IProductsStorage productsStorage)
        {
            Product product = GetProductOrDefaultById(secDef.product_id.Value, productsStorage);

            return product;
        }

        private Product GetProductOrDefaultById(int productId, IProductsStorage productsStore)
        {
            TryGetResult<Product> getProductResult = productsStore.TryGetProduct(productId);

            if (!getProductResult.HasValue)
            {
                return null;
            }

            return getProductResult.Value;
        }

        private bool IsNonMonthlyStrip(SecurityDefinition secDef, IProductsStorage prodStore)
        {
            List<Product> products = GetSecurityDefProducts(secDef, prodStore);
            bool isBalmoOrDailyProduct = HasBalmoOrDailyProduct(products);
            bool isSecurityDefinitionNontriptrip = false;

            if (secDef.Strip1DateType != null)
            {
                isSecurityDefinitionNontriptrip = secDef.IsNonMonthlySecurityDefinitionStrip();
            }

            return isBalmoOrDailyProduct || isSecurityDefinitionNontriptrip;
        }

        private List<Product> GetSecurityDefProducts(SecurityDefinition secDef, IProductsStorage prodStore)
        {
            List<int> productIds = new List<int>() { secDef.product_id.Value, };

            productIds.AddRange(secDef.PrecalcDetails?.Select(precalc => precalc.ProductId) ?? new List<int>());
            return productIds.Distinct().Select(id => prodStore.TryGetProduct(id).Value).ToList();
        }

        private bool HasBalmoOrDailyProduct(List<Product> products)
        {
            return products.Any(product => product.Type == ProductType.Balmo || product.IsProductDaily);
        }

        private int RecalcForSecurityDef(
            SecurityDefinition securityDefinition,
            IProductsStorage productsStorage,
            PrecalcPositionsCalculator precalcPositionsCalculator,
            int errCnt,
            StringBuilder recalcErrorTrades)
        {
            Product product = GetProductFromSecurityDefinition(securityDefinition, productsStorage);

            if (null == product)
            {
                _log.Warn("ProductManager: Could not find product for ID {0}", securityDefinition.product_id.Value);
            }

            securityDefinition.Product = product;

            TradeCapture trade = new TradeCapture
            {
                SecurityDefinition = securityDefinition,
                Exchange = product.Exchange.Name,
                IsParentTrade = true,
                OrdStatus = "Filled",
                Quantity = 1M,
                Price = 1M,
                TransactTime = SystemTime.Now()
            };
            try
            {
                precalcPositionsCalculator.ForceCalculatePrecalcPositions(trade, securityDefinition);
                _log.Debug(
                    "Precalc positions calculated for security definition id [{0}]",
                    securityDefinition.SecurityDefinitionId);
            }
            catch (NoSourceDetailException)
            {
                _log.Debug($"Error calculating precalc for trade id [{trade.TradeId}]");

                if (errCnt < 10)
                {
                    recalcErrorTrades.Append(trade.TradeId).Append(Environment.NewLine);
                }

                errCnt++;
            }

            return errCnt;
        }

        private void RecalcForAbnSourceDetail(
            SourceDetail srcDetail,
            ISecurityDefinitionsStorage securityDefinitionsStorage,
            IProductsStorage productsStorage,
            PrecalcPositionsCalculator precalcPositionsCalculator)
        {
            if (srcDetail.SecurityDefinitionId == null)
            {
                _log.Warn($"Source detail id [{srcDetail.SourceDetailId}] doesn't have a security definition");
                return;
            }

            Product product;
            // TODO: What if the product is not in the storage?
            SecurityDefinition securityDefinition = securityDefinitionsStorage
                .TryGetSecurityDefinition(srcDetail.SecurityDefinitionId.Value)
                .Value;

            product = GetProductOrDefaultById(srcDetail.product_id, productsStorage);
            securityDefinition.Product = product;
            srcDetail.SecurityDefinition = securityDefinition;
            srcDetail.Product = product;

            precalcPositionsCalculator.CalculatePrecalcPositions(srcDetail, securityDefinition);
            _log.Debug($"Precalc positions calculated for source detail id [{srcDetail.SourceDetailId}]");
        }

        private void ReportRecalcComplete(int errorCounter, StringBuilder recalcErrorTrades, BackgroundWorker worker)
        {
            StringBuilder finishMessage = new StringBuilder("Calculation finished, all changes have been saved.");

            if (errorCounter > 0)
            {
                finishMessage.AppendLine("Error recalculating trades:").AppendLine(recalcErrorTrades.ToString());
            }

            if (errorCounter > 10)
            {
                finishMessage.AppendLine($"... Total Errors: {errorCounter}");
            }

            _log.Info(finishMessage);
            worker.ReportProgress(100, finishMessage);
        }

        /// <summary>
        /// If saving is not completed, generally because the worker has been cancelled, return false.
        /// </summary>
        /// <param name="precalcPositionsCalculator"></param>
        /// <param name="worker"></param>
        /// <returns></returns>
        private bool SavePrecalcDetails(PrecalcPositionsCalculator _precalcPositionsCalculator, BackgroundWorker worker)
        {
            int entitiesToSave;
            int entitiesSaved = 0;

            _log.Info("Saving precalc details");

            using (MandaraEntities dbContext = new MandaraEntities())
            {
                dbContext.Database.CommandTimeout = 0;
                dbContext.Configuration.AutoDetectChangesEnabled = false;

                Dictionary<int, List<PrecalcSourcedetailsDetail>> srcDetailPrecalcsBySrcDetailId =
                    GetSrcDetailPrecalcs(_precalcPositionsCalculator.ClearerPrecalcs);
                Dictionary<int, List<PrecalcSdDetail>> srcDetailsSecDefPrecaclcsBySecDefId =
                    GetSrcDetailSecurityDefPrecalcs(_precalcPositionsCalculator.ClearerSecurityPrecalcs);

                entitiesToSave = _precalcPositionsCalculator.ExistingSecurityPrecalcs.Keys.Count
                              + _precalcPositionsCalculator.ExistingTradePrecalcs.Keys.Count
                              + srcDetailPrecalcsBySrcDetailId.Keys.Count
                              + srcDetailsSecDefPrecaclcsBySecDefId.Keys.Count;

                entitiesSaved = UpdateSecurityDefPrecalcs(
                    _precalcPositionsCalculator,
                    worker,
                    dbContext,
                    entitiesToSave);

                if (worker.CancellationPending)
                {
                    return false;
                }

                entitiesSaved += UpdateTradePrecalcs(
                    _precalcPositionsCalculator,
                    worker,
                    dbContext,
                    entitiesToSave,
                    entitiesSaved);

                if (worker.CancellationPending)
                {
                    return false;
                }

                entitiesSaved += UpdateSrcDetailPrecalcs(
                    worker,
                    srcDetailPrecalcsBySrcDetailId,
                    dbContext,
                    entitiesToSave,
                    entitiesSaved);

                if (worker.CancellationPending)
                {
                    return false;
                }

                entitiesSaved += UpdateSrcDetailSecurityDefPrecalcs(
                    worker,
                    srcDetailsSecDefPrecaclcsBySecDefId,
                    dbContext,
                    entitiesToSave,
                    entitiesSaved);
            }

            _log.Info("Saved all precalculation details");
            return entitiesSaved == entitiesToSave;
        }

        private static Dictionary<int, List<PrecalcSourcedetailsDetail>> GetSrcDetailPrecalcs(
            List<PrecalcSourcedetailsDetail> srcDetailPrecalcs)
        {
            Dictionary<int, List<PrecalcSourcedetailsDetail>> srcDetailPrecalcsBySrcDetailId =
                new Dictionary<int, List<PrecalcSourcedetailsDetail>>();

            foreach (PrecalcSourcedetailsDetail precalc in srcDetailPrecalcs)
            {
                List<PrecalcSourcedetailsDetail> precalcsForSrcDetailId;

                if (!srcDetailPrecalcsBySrcDetailId.TryGetValue(precalc.SourceDetailId, out precalcsForSrcDetailId))
                {
                    precalcsForSrcDetailId = new List<PrecalcSourcedetailsDetail>();
                }
                else
                {
                    srcDetailPrecalcsBySrcDetailId.Remove(precalc.SourceDetailId);
                }

                precalcsForSrcDetailId.Add(precalc);
                srcDetailPrecalcsBySrcDetailId.Add(precalc.SourceDetailId, precalcsForSrcDetailId);
            }
            return srcDetailPrecalcsBySrcDetailId;
        }

        private static Dictionary<int, List<PrecalcSdDetail>> GetSrcDetailSecurityDefPrecalcs(
            List<PrecalcSdDetail> srcDetailSecDefPrecalcs)
        {
            Dictionary<int, List<PrecalcSdDetail>> srcDetailsSecDefPrecaclcsBySecDefId =
                new Dictionary<int, List<PrecalcSdDetail>>();

            foreach (PrecalcSdDetail precalc in srcDetailSecDefPrecalcs)
            {
                List<PrecalcSdDetail> secDefPrecalcsForSrcDetail;

                if (!srcDetailsSecDefPrecaclcsBySecDefId.TryGetValue(
                    precalc.SecurityDefinitionId,
                    out secDefPrecalcsForSrcDetail))
                {
                    secDefPrecalcsForSrcDetail = new List<PrecalcSdDetail>();
                }
                else
                {
                    srcDetailsSecDefPrecaclcsBySecDefId.Remove(precalc.SecurityDefinitionId);
                }

                secDefPrecalcsForSrcDetail.Add(precalc);
                srcDetailsSecDefPrecaclcsBySecDefId.Add(precalc.SecurityDefinitionId, secDefPrecalcsForSrcDetail);
            }

            return srcDetailsSecDefPrecaclcsBySecDefId;
        }

        private int UpdateSecurityDefPrecalcs(
            PrecalcPositionsCalculator _precalcPositionsCalculator,
            BackgroundWorker worker,
            MandaraEntities dbContext,
            int entitiesToSave)
        {
            int entitiesSaved = 0;
            List<PrecalcSdDetail> secDefPrecalcs = new List<PrecalcSdDetail>();
            List<int> removePrecalcsForTheseSecDefs = new List<int>();

            foreach (int secDefId in _precalcPositionsCalculator.ExistingSecurityPrecalcs.Keys)
            {
                removePrecalcsForTheseSecDefs.Add(secDefId);
                secDefPrecalcs.AddRange(_precalcPositionsCalculator.ExistingSecurityPrecalcs[secDefId]);
                entitiesSaved++;

                if (secDefPrecalcs.Count > 100)
                {
                    if (worker.CancellationPending)
                    {
                        return entitiesSaved;
                    }

                    dbContext.PrecalcSdDetails.RemoveRange(
                        dbContext.PrecalcSdDetails.Where(
                                     precalc => removePrecalcsForTheseSecDefs.Contains(precalc.SecurityDefinitionId))
                                 .ToList());
                    dbContext.PrecalcSdDetails.AddRange(secDefPrecalcs);
                    dbContext.SaveChanges();

                    _log.Debug(
                        "Precalc security definitions saved: {0}",
                        string.Join(", ", removePrecalcsForTheseSecDefs));

                    removePrecalcsForTheseSecDefs.Clear();
                    secDefPrecalcs.Clear();

                    worker.ReportProgress(
                        Math.Min(100, 50 + (entitiesSaved * 50 / entitiesToSave)),
                        "Saving precalculated security definitions...");
                }
            }

            if (secDefPrecalcs.Count > 0)
            {
                dbContext.PrecalcSdDetails.RemoveRange(
                    dbContext.PrecalcSdDetails.Where(
                                 precalc => removePrecalcsForTheseSecDefs.Contains(precalc.SecurityDefinitionId))
                             .ToList());
                dbContext.PrecalcSdDetails.AddRange(secDefPrecalcs);
                dbContext.SaveChanges();

                _log.Debug("Precalc security definitions saved: {0}", string.Join(", ", removePrecalcsForTheseSecDefs));

                worker.ReportProgress(
                    Math.Min(100, 50 + (entitiesSaved * 50 / entitiesToSave)),
                    "Saving precalculated security definitions...");
            }
            return entitiesSaved;
        }

        private int UpdateTradePrecalcs(
            PrecalcPositionsCalculator _precalcPositionsCalculator,
            BackgroundWorker worker,
            MandaraEntities dbContext,
            int entitiesToSave,
            int entitiesSavedSoFar)
        {
            int entitiesSaved = entitiesSavedSoFar;
            List<PrecalcTcDetail> tradePrecalcs = new List<PrecalcTcDetail>();
            List<int> removePrecalcsForTheseTrades = new List<int>();

            foreach (int tradeId in _precalcPositionsCalculator.ExistingTradePrecalcs.Keys)
            {
                removePrecalcsForTheseTrades.Add(tradeId);
                tradePrecalcs.AddRange(_precalcPositionsCalculator.ExistingTradePrecalcs[tradeId]);
                entitiesSaved++;

                if (tradePrecalcs.Count > 100)
                {
                    if (worker.CancellationPending)
                    {
                        return entitiesSaved;
                    }

                    dbContext.PrecalcTcDetails.RemoveRange(
                        dbContext.PrecalcTcDetails.Where(
                                     precalc => removePrecalcsForTheseTrades.Contains(precalc.TradeCaptureId))
                                 .ToList());
                    dbContext.PrecalcTcDetails.AddRange(tradePrecalcs);
                    dbContext.SaveChanges();

                    _log.Debug("Precalc trade captures saved: {0}", string.Join(", ", removePrecalcsForTheseTrades));

                    removePrecalcsForTheseTrades.Clear();
                    tradePrecalcs.Clear();
                    worker.ReportProgress(
                        Math.Min(100, 50 + (entitiesSaved * 50 / entitiesToSave)),
                        "Saving precalculated trades...");
                }
            }

            if (tradePrecalcs.Count > 0)
            {
                dbContext.PrecalcTcDetails.RemoveRange(
                    dbContext.PrecalcTcDetails.Where(
                                 precalc => removePrecalcsForTheseTrades.Contains(precalc.TradeCaptureId))
                             .ToList());
                dbContext.PrecalcTcDetails.AddRange(tradePrecalcs);
                dbContext.SaveChanges();

                _log.Debug("Precalc trade captures saved: {0}", string.Join(", ", removePrecalcsForTheseTrades));

                worker.ReportProgress(
                    Math.Min(100, 50 + (entitiesSaved * 50 / entitiesToSave)),
                    "Saving precalculated trades...");
            }
            return entitiesSaved - entitiesSavedSoFar;
        }

        private int UpdateSrcDetailPrecalcs(
            BackgroundWorker worker,
            Dictionary<int, List<PrecalcSourcedetailsDetail>> srcDetailPrecalcsBySrcDetailId,
            MandaraEntities dbContext,
            int entitiesToSave,
            int entitiesSavedSoFar)
        {
            int entitiesSaved = entitiesSavedSoFar;
            List<PrecalcSourcedetailsDetail> srcDetailPrecalcs1 = new List<PrecalcSourcedetailsDetail>();
            List<int> removePrecalcsForTheseSrcDetails = new List<int>();

            foreach (int srcDetailId in srcDetailPrecalcsBySrcDetailId.Keys)
            {
                removePrecalcsForTheseSrcDetails.Add(srcDetailId);
                srcDetailPrecalcs1.AddRange(srcDetailPrecalcsBySrcDetailId[srcDetailId]);
                entitiesSaved++;

                if (srcDetailPrecalcs1.Count > 100)
                {
                    if (worker.CancellationPending)
                    {
                        return entitiesSaved;
                    }

                    dbContext.PrecalcSourcedetailsDetails.RemoveRange(
                        dbContext.PrecalcSourcedetailsDetails
                                 .Where(precalc => removePrecalcsForTheseSrcDetails.Contains(precalc.SourceDetailId))
                                 .ToList());
                    dbContext.PrecalcSourcedetailsDetails.AddRange(srcDetailPrecalcs1);
                    dbContext.SaveChanges();

                    _log.Debug("Precalc source details saved: {0}", string.Join(", ", removePrecalcsForTheseSrcDetails));

                    removePrecalcsForTheseSrcDetails.Clear();
                    srcDetailPrecalcs1.Clear();
                    worker.ReportProgress(
                        Math.Min(100, 50 + (entitiesSaved * 50 / entitiesToSave)),
                        "Saving precalculated source details...");
                }
            }

            if (srcDetailPrecalcs1.Count > 0)
            {
                dbContext.PrecalcSourcedetailsDetails.RemoveRange(
                    dbContext.PrecalcSourcedetailsDetails
                             .Where(precalc => removePrecalcsForTheseSrcDetails.Contains(precalc.SourceDetailId))
                             .ToList());
                dbContext.PrecalcSourcedetailsDetails.AddRange(srcDetailPrecalcs1);
                dbContext.SaveChanges();

                _log.Debug("Precalc source details saved: {0}", string.Join(", ", removePrecalcsForTheseSrcDetails));

                worker.ReportProgress(
                    Math.Min(100, 50 + (entitiesSaved * 50 / entitiesToSave)),
                    "Saving precalculated source details...");
            }
            return entitiesSaved - entitiesSavedSoFar;
        }

        private int UpdateSrcDetailSecurityDefPrecalcs(
            BackgroundWorker worker,
            Dictionary<int, List<PrecalcSdDetail>> srcDetailsSecDefPrecaclcsBySecDefId,
            MandaraEntities dbContext,
            int entitiesToSave,
            int entitiesSavedSoFar)
        {
            int entitiesSaved = entitiesSavedSoFar;
            List<PrecalcSdDetail> srcDetailSecDefPrecalcs1 = new List<PrecalcSdDetail>();
            List<int> removePrecalcsForTheseSecDefs1 = new List<int>();

            foreach (int secDefId in srcDetailsSecDefPrecaclcsBySecDefId.Keys)
            {
                removePrecalcsForTheseSecDefs1.Add(secDefId);
                srcDetailSecDefPrecalcs1.AddRange(srcDetailsSecDefPrecaclcsBySecDefId[secDefId]);
                entitiesSaved++;

                if (srcDetailSecDefPrecalcs1.Count > 100)
                {
                    if (worker.CancellationPending)
                    {
                        return entitiesSaved;
                    }

                    dbContext.PrecalcSdDetails.RemoveRange(
                        dbContext.PrecalcSdDetails.Where(
                                     precalc => removePrecalcsForTheseSecDefs1
                                         .Contains(precalc.SecurityDefinitionId))
                                 .ToList());
                    dbContext.PrecalcSdDetails.AddRange(srcDetailSecDefPrecalcs1);
                    dbContext.SaveChanges();

                    _log.Debug(
                        "Precalc security definitions saved: {0}",
                        string.Join(", ", removePrecalcsForTheseSecDefs1));

                    removePrecalcsForTheseSecDefs1.Clear();
                    srcDetailSecDefPrecalcs1.Clear();
                    worker.ReportProgress(
                        Math.Min(100, 50 + (entitiesSaved * 50 / entitiesToSave)),
                        "Saving precalculated source details...");
                }
            }

            if (srcDetailSecDefPrecalcs1.Count > 0)
            {
                dbContext.PrecalcSdDetails.RemoveRange(
                    dbContext.PrecalcSdDetails.Where(
                                 precalc => removePrecalcsForTheseSecDefs1.Contains(precalc.SecurityDefinitionId))
                             .ToList());
                dbContext.PrecalcSdDetails.AddRange(srcDetailSecDefPrecalcs1);
                dbContext.SaveChanges();

                _log.Debug(
                    "Precalc security definitions saved: {0}",
                    string.Join(", ", removePrecalcsForTheseSecDefs1));

                worker.ReportProgress(
                    Math.Min(100, 50 + (entitiesSaved * 50 / entitiesToSave)),
                    "Saving precalculated source details...");
            }

            return entitiesSaved - entitiesSavedSoFar;
        }
    }
}