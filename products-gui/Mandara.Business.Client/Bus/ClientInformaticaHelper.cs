using System.Collections.Generic;
using com.latencybusters.lbm;
using Mandara.Business.Bus.Handlers;
using Mandara.Business.Bus.Handlers.Base;

namespace Mandara.Business.Bus
{
    public class ClientInformaticaHelper : InformaticaHelper
    {
        private readonly bool _turnOnErrorsReceiver;

        public ClientInformaticaHelper(LBMContext lbmContext, HandlerManager handlerManager, bool turnOnErrorsReceiver = true)
            : base(lbmContext, handlerManager)
        {
            _turnOnErrorsReceiver = turnOnErrorsReceiver;
        }

        public override void CreateSources()
        {
            AddSource(TradesSnapshotTopicName);
            AddSource(FxTradesSnapshotTopicName);
            AddSource(PnlSnapshotChangedTopicName);
            AddSource(FxExposureDetailSnapshotTopicName);
            AddSource(PositionsTradesTopicName);
            AddSource(PositionsSnapshotTopicName);
            AddSource(FxExposureTradesTopicName);
            AddSource(PositionsWithTradesSnapshotTopicName);
            AddSource(TasSnapshotTopicName);
            AddSource(OvernightPnlSnapshotTopicName);
            AddSource(PnLByProductSnapshotTopicName);
            AddSource(SpreaderCalculationTopicName);
            AddSource(SpreaderProductsTopicName);

            AddSource(TasTradesTopicName);
            AddSource(TasTradesUpdateTopicName);
            AddSource(CsvDataImportTopicName);
            AddSource(HistoricalPositionsSnapshotTopicName);
            AddSource(HistoricalPnlSnapshotTopicName);
            AddSource(HistoricalParamsTopicName);
            AddSource(PriceTimestampsTopicName);
            AddSource(PortfolioSnapshotTopicName);
            AddSource(PortfolioEditTopicName);
            AddSource(ExpiringProductsSnapshotTopicName);
            AddSource(ExpiringProductsMarkAsDeliveredTopicName);
            AddSource(AssignTradesTopicName);
            AddSource(DataValidationSnapshotTopicName);

            AddSource(PnlCumulativeSnapshotTopicName);
            AddSource(UserPermissionSnapshotTopicName);
            AddSource(ChangePasswordTopicName);

            AddSource(UncancelTradesTopicName);
            AddSource(CancelTradesTopicName);
            AddSource(EditTradesTopicName);
            AddSource(ValidateBalmoDateTopicName);

            AddSource(TradeImpactTopicName);
            AddSource(EditTradeParamsTopicName);

            AddSource(OfficialProductInfoTopicName);

            AddSource(PnlHistoricalDateTopicName);
            AddSource(PnlHistoricalInformationTopicName);
            AddSource(PnlHistoricalSaveTopicName);

            AddSource(MatchSnapshotTopicName);
            AddSource(WrongMatchTopicName);

            AddSource(TasCheckSnapshotTopicName);
            AddSource(TasCheckSkipTopicName);

            AddSource(SwapExposureTopicName);
            AddSource(SwapExposureRequestTradesTopicName);
            AddSource(SwapExposurePlaceTradesTopicName);

            AddSource(WriteAuditTopicName);

            AddSource(DeriskingCalcTopicName);

            AddSource(AdmAlertAcknowledgeTopicName);
            AddSource(AdmAlertsSnapshotTopicName);
            AddSource(AdmAlertHistorySnapshotTopicName);

            AddSource(ClosingPositionsTradesTopicName);
            AddSource(ClosingPositionsPlaceTradesTopicName);

            //Trade support topics
            AddSource(TradeSupportAcknowlegeTopicName);
            AddSource(TradeSupportSnapshotTopicName);
            //CustomSpreads topics
            AddSource(CustomSpreadTopicName);
            AddSource(CustomSpreadSnapshotTopicName);

            AddSource(ProductCategoryInfoTopicName);

            AddSource(LiveFeedReplayTopicName);

            AddSource(SettingsTopicName);

            AddSource(TradeAddPrerequisitesTopicName);
            AddSource(TradeAddImpactTopicName);
            AddSource(TradeAddCreateTopicName);

            AddSource(ChangeSnapshotTimeTopicName);
            AddSource(LiveTradesPnlTopicName);

            AddSource(AmendBrokerageTopicName);
            AddSource(TradesPnlOnDateTopicName);
            AddSource(FeaturesTopicName);
            AddSource(CurrenciesTopicName);

            AddSource(ExportSourceDataTopicName);

            AddSource(PricingReportTradesTopicName);
            AddSource(ClientHeartbeatTopicName);

            AddSource(DailyReconciliationSnapshotTopicName);
            AddSource(FxExposureSnapshotTopicName);
            AddSource(ForceRecalculatePositionsTopicName);

            AddSource(FxHedgeDetailSnapshotTopicName);

            AddSource(ReportRequestTopicName);

            base.CreateSources();
        }

        public override void CreateReceivers()
        {
            AddReceiver(HeartbeatTopicName, typeof(HeartbeatHandler));
            AddReceiver(PnlUpdateTopicName, typeof(PnlUpdateHandler));
            AddReceiver(FxExposureDetailUpdateTopicName, typeof(FxExposureDetailUpdateHandler));
            AddReceiver(TradesUpdateTopicName, typeof(TradesUpdateHandler));
            AddReceiver(FxTradesUpdateTopicName, typeof(FxTradesUpdateHandler));
            AddReceiver(PositionsUpdateTopicName, typeof(PositionsUpdateHandler));

            if (_turnOnErrorsReceiver)
            {
                AddReceiver(ErrorsTopicName, typeof(ErrorsHandler));
                AddReceiver(PositionsCalcErrorTopicName, typeof(CalculationErrorsHandler));
            }

            AddReceiver(LiveDataTopicName, typeof(LiveDataHandler));
            AddReceiver(PortfolioUpdateTopicName, typeof(PortfolioUpdateHandler));

            AddReceiver(MatchUpdateTopicName, typeof(MatchingDummiesUpdateHandler));
            AddReceiver(RolloffNotificationTopicName, typeof(RolloffNotificationHandler));

            AddReceiver(TasCheckUpdateTopicName, typeof(TasCheckUpdateHandler));

            AddReceiver(AdmAlertTopicName, typeof(AdmAlertHandler));
            AddReceiver(AdmAlertAcknowledgeDoneTopicName, typeof(AdmAlertAcknowledgeDoneHandler));
            AddReceiver(TradeSupportUpdateTopicName, typeof(TradeSupportHandler));

            AddReceiver(ClientReconnectTopicName, typeof(ClientReconnectHandler));
            AddReceiver(ClientReconnectIndividualTopicName, typeof(ClientReconnectHandler));
            AddReceiver(VarLatestTopicName, typeof(VarLatestUpdateHandler));
            AddReceiver(PnlDiffTopicName, typeof(PnlDiffUpdateHandler));

            base.CreateReceivers();
        }
    }
}
