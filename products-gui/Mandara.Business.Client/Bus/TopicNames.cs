using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using Mandara.Extensions.Collections;
using MoreLinq;

namespace Mandara.Business.Bus
{
    public partial class InformaticaHelper
    {
        [Obsolete("Set a single prefix in ServerPrefixes instead.")]
        public static string ServerPrefix
        {
            get
            {
                return ServerPrefixes.Any() ? ServerPrefixes[0] : String.Empty;
            }
            set
            {
                if (ServerPrefixes.Any())
                {
                    ServerPrefixes[0] = value;
                }
                else
                {
                    ServerPrefixes = new[] { value };
                }
            }
        }

        public static string[] ServerPrefixes { get; set; } = new string[] {};

        public static string DefaultMessagingEnvironment
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TopicDefinition.DefaultEnvironment))
                {
                    string defaultMsgEnviron = ConfigurationManager.AppSettings["MessagingEnvironment"] ?? "DEV";

                    TopicDefinition.DefaultEnvironment = defaultMsgEnviron;
                }

                return TopicDefinition.DefaultEnvironment;
            }
        }

        public const string AppPrefix = "IRM";

        public const string HeartbeatTopic = "Heartbeat";
        public static string HeartbeatTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(HeartbeatTopic), GetDefaultAppSettingName(HeartbeatTopic));

        public static string GetDefaultTopicStructure(string topic)
        {
            return $"/{AppPrefix}/{topic}";
        }

        public static string GetDefaultAppSettingName(string topic)
        {
            return $"{topic}TopicName";
        }

        public const string PnlUpdateTopic = "PNL/Update";
		public const string PnlUpdatePrefix = "PnlUpdate";
		public static string PnlUpdateTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(PnlUpdateTopic), GetDefaultAppSettingName(PnlUpdatePrefix));

        public const string FxExposureDetailUpdateTopic = "FxExposureDetail/Update";
		public const string FxExposureDetailUpdatePrefix = "FxExposureDetailUpdate";
		public static string FxExposureDetailUpdateTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(FxExposureDetailUpdateTopic), GetDefaultAppSettingName(FxExposureDetailUpdatePrefix));

        public const string FxExposureDetailSnapshotTopic = "FxExposureDetail/Snapshot";
		public const string FxExposureDetailSnapshotPrefix = "FxExposureDetailSnapshot";
		public static string FxExposureDetailSnapshotTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(FxExposureDetailSnapshotTopic), GetDefaultAppSettingName(FxExposureDetailSnapshotPrefix));

        public const string PnlSnapshotChangedTopic = "PNL/SnapshotChanged";
		public const string PnlSnapshotChangedPrefix = "PnlSnapshotChanged";
		public static string PnlSnapshotChangedTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(PnlSnapshotChangedTopic), GetDefaultAppSettingName(PnlSnapshotChangedPrefix));

        public const string TradesSnapshotTopic = "LT/Snapshot";
		public const string TradesSnapshotPrefix = "TradesSnapshot";
		public static string TradesSnapshotTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(TradesSnapshotTopic), GetDefaultAppSettingName(TradesSnapshotPrefix));

        public const string FxTradesSnapshotTopic = "LFXT/Snapshot";
		public const string FxTradesSnapshotPrefix = "FxTradesSnapshot";
		public static string FxTradesSnapshotTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(FxTradesSnapshotTopic), GetDefaultAppSettingName(FxTradesSnapshotPrefix));

        public const string ExpiringProductsTopic = "ExpiringProducts";
        public static string ExpiringProductsTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(ExpiringProductsTopic), GetDefaultAppSettingName(ExpiringProductsTopic));

        public const string MarkExpiringProductsTopic = "ExpiringProductsMarkAsDelivered";

        public static string ExpiringProductsMarkAsDeliveredTopicName =>
            GetServerAwareTopicName(
                GetDefaultTopicStructure(MarkExpiringProductsTopic),
                GetDefaultAppSettingName(MarkExpiringProductsTopic));

        public const string ExpiringProductsSnapshotTopic = "ExpiringProductsSnapshot";
        public static string ExpiringProductsSnapshotTopicName =>
            GetServerAwareTopicName(
                GetDefaultTopicStructure(ExpiringProductsSnapshotTopic),
                GetDefaultAppSettingName(ExpiringProductsSnapshotTopic));

        public const string PositionsTradesTopic = "LP/Trades";
		public const string PositionsTradesPrefix = "PositionsTrades";
		public static string PositionsTradesTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(PositionsTradesTopic), GetDefaultAppSettingName(PositionsTradesPrefix));

        public const string FxExposureTradesTopic = "FxExposureDetail/Trades";
		public const string FxExposureTradesPrefix = "FxExposureTrades";
		public static string FxExposureTradesTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(FxExposureTradesTopic), GetDefaultAppSettingName(FxExposureTradesPrefix));

        public const string PositionsSnapshotTopic = "LP/Snapshot";
		public const string PositionsSnapshotPrefix = "PositionsSnapshot";
		public static string PositionsSnapshotTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(PositionsSnapshotTopic), GetDefaultAppSettingName(PositionsSnapshotPrefix));

        public const string TradesUpdateTopic = "LT/Update";
		public const string TradesUpdatePrefix = "TradesUpdate";
		public static string TradesUpdateTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(TradesUpdateTopic), GetDefaultAppSettingName(TradesUpdatePrefix));

        public const string FxTradesUpdateTopic = "LFXT/Update";
		public const string FxTradesUpdatePrefix = "FxTradesUpdate";
		public static string FxTradesUpdateTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(FxTradesUpdateTopic), GetDefaultAppSettingName(FxTradesUpdatePrefix));

        public const string PositionsUpdateTopic = "LP/Update";
		public const string PositionsUpdatePrefix = "PositionsUpdate";
		public static string PositionsUpdateTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(PositionsUpdateTopic), GetDefaultAppSettingName(PositionsUpdatePrefix));

        public const string PositionsWithTradesSnapshotTopic = "LP/SnapshotWithTrades";
		public const string PositionsWithTradesSnapshotPrefix = "PositionsWithTradesSnapshot";
        public static string PositionsWithTradesSnapshotTopicName =>
            GetServerAwareTopicName(
                GetDefaultTopicStructure(PositionsWithTradesSnapshotTopic),
                GetDefaultAppSettingName(PositionsWithTradesSnapshotPrefix));

        public const string TasSnapshotTopic = "TAS/Snapshot";
		public const string TasSnapshotPrefix = "TasSnapshot";
		public static string TasSnapshotTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(TasSnapshotTopic), GetDefaultAppSettingName(TasSnapshotPrefix));

        public const string OvernightPnlSnapshotTopic = "ONPNL/Snapshot";
		public const string OvernightPnlSnapshotPrefix = "OvernightPnlSnapshot";
		public static string OvernightPnlSnapshotTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(OvernightPnlSnapshotTopic), GetDefaultAppSettingName(OvernightPnlSnapshotPrefix));

        public const string PnLByProductSnapshotTopic = "PNLP/Snapshot";
		public const string PnLByProductSnapshotPrefix = "PnLByProductSnapshot";
		public static string PnLByProductSnapshotTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(PnLByProductSnapshotTopic), GetDefaultAppSettingName(PnLByProductSnapshotPrefix));

        public const string DataValidationSnapshotTopic = "DataValidationSnapshot";
        public static string DataValidationSnapshotTopicName =>
            GetServerAwareTopicName(
                GetDefaultTopicStructure(DataValidationSnapshotTopic),
                GetDefaultAppSettingName(DataValidationSnapshotTopic));

        public const string ErrorsTopic = "Errors";
        public static string ErrorsTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(ErrorsTopic), GetDefaultAppSettingName(ErrorsTopic));

        public const string PositionsCalcErrorTopic = "LP/Error";
		public const string PositionsCalcErrorPrefix = "PositionsCalcError";
		public static string PositionsCalcErrorTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(PositionsCalcErrorTopic), GetDefaultAppSettingName(PositionsCalcErrorPrefix));

        public const string SpreaderCalculationTopic = "Spreader/Calculation";
		public const string SpreaderCalculationPrefix = "SpreaderCalculation";
		public static string SpreaderCalculationTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(SpreaderCalculationTopic), GetDefaultAppSettingName(SpreaderCalculationPrefix));

        public const string SpreaderProductsTopic = "Spreader/Products";
		public const string SpreaderProductsPrefix = "SpreaderProducts";
		public static string SpreaderProductsTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(SpreaderProductsTopic), GetDefaultAppSettingName(SpreaderProductsPrefix));

        public const string TasTradesTopic = "TasTrades";
        public static string TasTradesTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(TasTradesTopic), GetDefaultAppSettingName(TasTradesTopic));

        public const string TasTradesUpdateTopic = "TasTrades/Update";
		public const string TasTradesUpdatePrefix = "TasTradesUpdate";
		public static string TasTradesUpdateTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(TasTradesUpdateTopic), GetDefaultAppSettingName(TasTradesUpdatePrefix));

        public const string CsvDataImportTopic = "SourceData";
		public const string CsvDataImportPrefix = "CsvDataImport";
		public static string CsvDataImportTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(CsvDataImportTopic), GetDefaultAppSettingName(CsvDataImportPrefix));

        public const string HistoricalPositionsSnapshotTopic = "HistoricalPositionsSnapshot";

        public static string HistoricalPositionsSnapshotTopicName =>
            GetServerAwareTopicName(
                GetDefaultTopicStructure(HistoricalPositionsSnapshotTopic),
                GetDefaultAppSettingName(HistoricalPositionsSnapshotTopic));

        public const string HistoricalPnlSnapshotTopic = "HistoricalPnlSnapshot";
        public static string HistoricalPnlSnapshotTopicName =>
            GetServerAwareTopicName(
                GetDefaultTopicStructure(HistoricalPnlSnapshotTopic),
                GetDefaultAppSettingName(HistoricalPnlSnapshotTopic));

        public const string HistoricalParamsTopic = "HistoricalParams";
        public static string HistoricalParamsTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(HistoricalParamsTopic), GetDefaultAppSettingName(HistoricalParamsTopic));

        public const string PriceTimestampsTopic = "PriceTimestamps";
        public static string PriceTimestampsTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(PriceTimestampsTopic), GetDefaultAppSettingName(PriceTimestampsTopic));

        public const string AssignTradesTopic = "AssignTrades";
        public static string AssignTradesTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(AssignTradesTopic), GetDefaultAppSettingName(AssignTradesTopic));

        public const string LiveDataTopic = "TradesPnlAndPrices";
		public const string LiveDataPrefix = "LiveData";
		public static string LiveDataTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(LiveDataTopic), GetDefaultAppSettingName(LiveDataPrefix));

        public const string PortfolioUpdateTopic = "PortfolioUpdate";
        public static string PortfolioUpdateTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(PortfolioUpdateTopic), GetDefaultAppSettingName(PortfolioUpdateTopic));

        public const string PortfolioSnapshotTopic = "PortfolioSnapshot";
        public static string PortfolioSnapshotTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(PortfolioSnapshotTopic), GetDefaultAppSettingName(PortfolioSnapshotTopic));

        public const string PortfolioEditTopic = "PortfolioEdit";
        public static string PortfolioEditTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(PortfolioEditTopic), GetDefaultAppSettingName(PortfolioEditTopic));

        public const string PnlCumulativeSnapshotTopic = "CumulativePnl";
		public const string PnlCumulativeSnapshotPrefix = "PnlCumulativeSnapshot";
		public static string PnlCumulativeSnapshotTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(PnlCumulativeSnapshotTopic), GetDefaultAppSettingName(PnlCumulativeSnapshotPrefix));

        public const string UserPermissionSnapshotTopic = "UserPermissions";
		public const string UserPermissionSnapshotPrefix = "UserPermissionSnapshot";
		public static string UserPermissionSnapshotTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(UserPermissionSnapshotTopic), GetDefaultAppSettingName(UserPermissionSnapshotPrefix));

        public const string CancelTradesTopic = "CancelTrades";
        public static string CancelTradesTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(CancelTradesTopic), GetDefaultAppSettingName(CancelTradesTopic));

        public const string EditTradesTopic = "EditTrades";
        public static string EditTradesTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(EditTradesTopic), GetDefaultAppSettingName(EditTradesTopic));

        public const string ValidateBalmoDateTopic = "ValidateBalmoDate";
        public static string ValidateBalmoDateTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(ValidateBalmoDateTopic), GetDefaultAppSettingName(ValidateBalmoDateTopic));

        public const string UncancelTradesTopic = "UncancelTrades";
        public static string UncancelTradesTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(UncancelTradesTopic), GetDefaultAppSettingName(UncancelTradesTopic));

        public const string TradeImpactTopic = "TradeImpact";
        public static string TradeImpactTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(TradeImpactTopic), GetDefaultAppSettingName(TradeImpactTopic));

        public const string EditTradeParamsTopic = "EditTradeParams";
        public static string EditTradeParamsTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(EditTradeParamsTopic), GetDefaultAppSettingName(EditTradeParamsTopic));

        public const string PnlHistoricalDateTopic = "PnlHistoricalDate";
        public static string PnlHistoricalDateTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(PnlHistoricalDateTopic), GetDefaultAppSettingName(PnlHistoricalDateTopic));

        public const string PnlHistoricalInformationTopic = "PnlHistoricalInformation";
        public static string PnlHistoricalInformationTopicName =>
            GetServerAwareTopicName(
                GetDefaultTopicStructure(PnlHistoricalInformationTopic),
                GetDefaultAppSettingName(PnlHistoricalInformationTopic));

        public const string ReportRequestTopic = "ReportRequest";
        public static string ReportRequestTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(ReportRequestTopic), GetDefaultAppSettingName(ReportRequestTopic));

        public const string PnlHistoricalSaveTopic = "PnlHistoricalSave";
        public static string PnlHistoricalSaveTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(PnlHistoricalSaveTopic), GetDefaultAppSettingName(PnlHistoricalSaveTopic));

        public const string OfficialProductInfoTopic = "OfficialProductInfo";
        public static string OfficialProductInfoTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(OfficialProductInfoTopic), GetDefaultAppSettingName(OfficialProductInfoTopic));

        public const string ResetServerTopic = "ResetServer";
        public static string ResetServerTopicName =>
            GetServerAwareTopicName($"/PT/{ResetServerTopic}", GetDefaultAppSettingName(ResetServerTopic), "PT");

        //This topic is not depend on IRM server change
        public const string MatchingSnapshotTopic = "MatchSnapshot";
        public const string MatchingSnapshotSetting = "MatchingSnapshot";

        public static string MatchSnapshotTopicName =>
            GetTopicName(
                new TopicDefinition
                {
                    DefaultTopicNameSuffix = $"/DM/{MatchingSnapshotTopic}",
                    TopicAppSettingName = GetDefaultAppSettingName(MatchingSnapshotSetting)
                });

        //This topic is not depend on IRM server change
        public const string MatchUpdateTopic = "MatchUpdate";
        public static string MatchUpdateTopicName =>
            GetTopicName(
                new TopicDefinition
                {
                    DefaultTopicNameSuffix = $"/DM/{MatchUpdateTopic}",
                    TopicAppSettingName = GetDefaultAppSettingName(MatchUpdateTopic)
                });

        //This topic is not depend on IRM server change
        public const string WrongMatchTopic = "WrongMatch";

        public static string WrongMatchTopicName =>
            GetTopicName(
                new TopicDefinition
                {
                    DefaultTopicNameSuffix = $"/DM/{WrongMatchTopic}",
                    TopicAppSettingName = GetDefaultAppSettingName(WrongMatchTopic)
                });

        public const string ChangePasswordTopic = "ChangePassword";
        public static string ChangePasswordTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(ChangePasswordTopic), GetDefaultAppSettingName(ChangePasswordTopic));

        public const string RolloffNotificationTopic = "RolloffNotification";
        public static string RolloffNotificationTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(RolloffNotificationTopic), GetDefaultAppSettingName(RolloffNotificationTopic));

        public const string TasCheckUpdateTopic = "TasCheckUpdate";
        public static string TasCheckUpdateTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(TasCheckUpdateTopic), GetDefaultAppSettingName(TasCheckUpdateTopic));

        public const string TasCheckSnapshotTopic = "TasCheckSnapshot";
        public static string TasCheckSnapshotTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(TasCheckSnapshotTopic), GetDefaultAppSettingName(TasCheckSnapshotTopic));

        public const string TasCheckSkipTopic = "TasCheckSkip";
        public static string TasCheckSkipTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(TasCheckSkipTopic), GetDefaultAppSettingName(TasCheckSkipTopic));

        public const string SwapExposureTopic = "SwapExposure";
        public static string SwapExposureTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(SwapExposureTopic), GetDefaultAppSettingName(SwapExposureTopic));

        public const string SwapExposureRequestTradesTopic = "SwapExposureRequestTrades";
        public static string SwapExposureRequestTradesTopicName =>
            GetServerAwareTopicName(
                GetDefaultTopicStructure(SwapExposureRequestTradesTopic),
                GetDefaultAppSettingName(SwapExposureRequestTradesTopic));

        public const string SwapExposurePlaceTradesTopic = "SwapExposurePlaceTrades";
        public static string SwapExposurePlaceTradesTopicName =>
            GetServerAwareTopicName(
                GetDefaultTopicStructure(SwapExposurePlaceTradesTopic),
                GetDefaultAppSettingName(SwapExposurePlaceTradesTopic));

        public const string WriteAuditTopic = "WriteAudit";
        public static string WriteAuditTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(WriteAuditTopic), GetDefaultAppSettingName(WriteAuditTopic));

        public const string CalcDeriskingTopic = "CalcDerisking";
        public static string DeriskingCalcTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(CalcDeriskingTopic), GetDefaultAppSettingName(CalcDeriskingTopic));

        public const string AdmAlertTopic = "AdmAlert";
        public static string AdmAlertTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(AdmAlertTopic), GetDefaultAppSettingName(AdmAlertTopic));

        public const string AdmAlertAcknowledgeTopic = "AdmAlertAcknowledge";
        public static string AdmAlertAcknowledgeTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(AdmAlertAcknowledgeTopic), GetDefaultAppSettingName(AdmAlertAcknowledgeTopic));

        public const string AdmAlertsSnapshotTopic = "AdmAlertsSnapshot";
        public static string AdmAlertsSnapshotTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(AdmAlertsSnapshotTopic), GetDefaultAppSettingName(AdmAlertsSnapshotTopic));

        public const string AdmAlertAcknowledgeDoneTopic = "AdmAlertAcknowledgeDone";
        public static string AdmAlertAcknowledgeDoneTopicName =>
            GetServerAwareTopicName(
                GetDefaultTopicStructure(AdmAlertAcknowledgeDoneTopic),
                GetDefaultAppSettingName(AdmAlertAcknowledgeDoneTopic));

        public const string ClosingPositionsTradesTopic = "ClosingPositionsTrades";
        public static string ClosingPositionsTradesTopicName =>
            GetServerAwareTopicName(
                GetDefaultTopicStructure(ClosingPositionsTradesTopic),
                GetDefaultAppSettingName(ClosingPositionsTradesTopic));

        public const string ClosingPositionsPlaceTopic = "ClosingPositionsPlaceTrades";
        public static string ClosingPositionsPlaceTradesTopicName =>
            GetServerAwareTopicName(
                GetDefaultTopicStructure(ClosingPositionsPlaceTopic),
                GetDefaultAppSettingName(ClosingPositionsPlaceTopic));

        public const string AdmAlertHistorySnapshotTopic = "AdmAlertHistorySnapshot";
        public static string AdmAlertHistorySnapshotTopicName =>
            GetServerAwareTopicName(
                GetDefaultTopicStructure(AdmAlertHistorySnapshotTopic),
                GetDefaultAppSettingName(AdmAlertHistorySnapshotTopic));

        public const string TradeSupportSnapshotTopic = "TradeSupport";
		public const string TradeSupportSnapshotPrefix = "TradeSupportSnapshot";
        public static string TradeSupportSnapshotTopicName =>
            GetServerAwareTopicName(
                GetDefaultTopicStructure(TradeSupportSnapshotTopic),
                GetDefaultAppSettingName(TradeSupportSnapshotPrefix));

        public const string TradeSupportAcknowlegeTopic = "AcknowledgeTradeSupport";
		public const string TradeSupportAcknowlegePrefix = "TradeSupportAcknowlege";

        public static string TradeSupportAcknowlegeTopicName =>
            GetServerAwareTopicName(
                GetDefaultTopicStructure(TradeSupportAcknowlegeTopic),
                GetDefaultAppSettingName(TradeSupportAcknowlegePrefix));

        public const string TradeSupportUpdateTopic = "TradeSupportUpdate";
        public static string TradeSupportUpdateTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(TradeSupportUpdateTopic), GetDefaultAppSettingName(TradeSupportUpdateTopic));

        public const string CustomSpreadSnapshotTopic = "CustomSpreadSnapshot";
        public static string CustomSpreadSnapshotTopicName =>
            GetServerAwareTopicName(
                GetDefaultTopicStructure(CustomSpreadSnapshotTopic),
                GetDefaultAppSettingName(CustomSpreadSnapshotTopic));

        public const string CustomSpreadNotifyTopic = "CustomSpreadNotify";
        public static string CustomSpreadNotifyTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(CustomSpreadNotifyTopic), GetDefaultAppSettingName(CustomSpreadNotifyTopic));

        public const string CustomSpreadTopic = "CustomSpread";
        public static string CustomSpreadTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(CustomSpreadTopic), GetDefaultAppSettingName(CustomSpreadTopic));

        public const string ProductCategoryInfoTopic = "ProductCategoryInfo";
        public static string ProductCategoryInfoTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(ProductCategoryInfoTopic), GetDefaultAppSettingName(ProductCategoryInfoTopic));

        public const string LiveFeedReplayTopic = "LiveFeedReplay";
        public static string LiveFeedReplayTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(LiveFeedReplayTopic), GetDefaultAppSettingName(LiveFeedReplayTopic));

        public const string SettingsTopic = "Settings";
        public static string SettingsTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(SettingsTopic), GetDefaultAppSettingName(SettingsTopic));

        public const string TradeAddPrerequisitesTopic = "TradeAddPrerequisites";
        public static string TradeAddPrerequisitesTopicName =>
            GetServerAwareTopicName(
                GetDefaultTopicStructure(TradeAddPrerequisitesTopic),
                GetDefaultAppSettingName(TradeAddPrerequisitesTopic));

        public const string TradeAddImpactTopic = "TradeAddImpact";
        public static string TradeAddImpactTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(TradeAddImpactTopic), GetDefaultAppSettingName(TradeAddImpactTopic));

        public const string TradeAddCreateTopic = "TradeAddCreate";
        public static string TradeAddCreateTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(TradeAddCreateTopic), GetDefaultAppSettingName(TradeAddCreateTopic));

        public const string ChangeSnapshotTimeTopic = "ChangeSnapshotTime";
        public static string ChangeSnapshotTimeTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(ChangeSnapshotTimeTopic), GetDefaultAppSettingName(ChangeSnapshotTimeTopic));

        public const string LiveTradesPnLTopic = "LiveTradesPnl";

        public static string LiveTradesPnlTopicName =>
            GetServerAwareTopicName(
                GetDefaultTopicStructure(LiveTradesPnLTopic),
                GetAppSettingName(LiveTradesPnLTopic, String.Empty));

        private static string GetAppSettingName(string topic, string suffix)
        {
            return $"{topic}{suffix}";
        }

        public const string AmendBrokerageTopic = "AmendBrokerage";
        public static string AmendBrokerageTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(AmendBrokerageTopic), GetDefaultAppSettingName(AmendBrokerageTopic));

        public const string TradesPnlOnDateTopic = "TradesPnlOnDate";
        public static string TradesPnlOnDateTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(TradesPnlOnDateTopic), GetDefaultAppSettingName(TradesPnlOnDateTopic));

        public const string FeaturesTopic = "Features";
        public static string FeaturesTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(FeaturesTopic), GetDefaultAppSettingName(FeaturesTopic));

        public const string CurrenciesTopic = "Currencies";
        public static string CurrenciesTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(CurrenciesTopic), GetDefaultAppSettingName(CurrenciesTopic));

        public const string TransferErrorsTopic = "TransferErrors";
        public static string TransferErrorsTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(TransferErrorsTopic), GetDefaultAppSettingName(TransferErrorsTopic));

        public const string ExportSourceDataTopic = "ExportSourceData";
        public static string ExportSourceDataTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(ExportSourceDataTopic), GetDefaultAppSettingName(ExportSourceDataTopic));

        public const string PricingReportTradesTopic = "PricingReportTrades";
        public static string PricingReportTradesTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(PricingReportTradesTopic), GetDefaultAppSettingName(PricingReportTradesTopic));

        // only IRM Client (listens to) and AdmTool (sends to) uses this topic so it's not "server aware"-like
        public const string ClientReconnectTopic = "ClientReconnect";
        public static string ClientReconnectTopicName =>
            GetTopicName(
                new TopicDefinition
                {
                    DefaultTopicNameSuffix = GetDefaultTopicStructure(ClientReconnectTopic),
                    TopicAppSettingName = GetDefaultAppSettingName(ClientReconnectTopic)
                });

        // only IRM Client (listens to) and AdmTool (sends to) uses this topic so it's not "server aware"-like
        public static string ClientReconnectIndividualTopicName =>
            GetClientIndividualTopicName(ClientReconnectTopicName, ClientGuidHolder.ClientGuid);

        public const string ClientHeartbeatTopic = "ClientHeartbeat";
        /// <summary>
        /// The Client Heartbeat 
        /// </summary>
        public static string ClientHeartbeatTopicName =>
            GetTopicName(
                new TopicDefinition
                {
                    DefaultTopicNameSuffix = GetDefaultTopicStructure(ClientHeartbeatTopic),
                    TopicAppSettingName = GetDefaultAppSettingName(ClientHeartbeatTopic)
                });

        public const string VarTopic = "VarLatest";
        public const string VarSetting = "Var";
        //This topic is not depend on IRM server change
        /// <summary>
        /// Communication between VarService and IRM client.
        /// </summary>
        public static string VarLatestTopicName =>
            GetTopicName(
                new TopicDefinition
                {
                    DefaultTopicNameSuffix = GetDefaultTopicStructure(VarTopic), TopicAppSettingName = GetDefaultAppSettingName(VarSetting)
                });

        public const string PnlDiffTopic = "Sc";
        public const string PnlDiffSetting = "PnlDiff";

        /// <summary>
        /// Communication between PnlDiff providers and IRM client.
        /// </summary>
        public static string PnlDiffTopicName =>
            GetTopicName(
                new TopicDefinition
                {
                    DefaultTopicNameSuffix = GetDefaultTopicStructure(PnlDiffTopic),
                    TopicAppSettingName = GetDefaultAppSettingName(PnlDiffSetting)
                });

        public const string SecurityDefinitionsSnapshotTopic = "SecurityDefinitionsSnapshot";
        public static string SecurityDefinitionsSnapshotTopicName =>
            GetTopicName(
                new TopicDefinition
                {
                    DefaultTopicNameSuffix = GetDefaultTopicStructure(SecurityDefinitionsSnapshotTopic),
                    TopicAppSettingName = GetDefaultAppSettingName(SecurityDefinitionsSnapshotTopic)
                });

        public const string ProductBreakdownTopic = "ProductBreakdown";
        public static string ProductBreakdownTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(ProductBreakdownTopic), GetDefaultAppSettingName(ProductBreakdownTopic));

        public const string DailyRecsSnapshotTopic = "DailyReconciliationSnapshot";

        public static string DailyReconciliationSnapshotTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(DailyRecsSnapshotTopic), GetDefaultAppSettingName(DailyRecsSnapshotTopic));

        public const string FxExposureSnapshotTopic = "FxExposureSnapshot";
        public static string FxExposureSnapshotTopicName =>
            GetServerAwareTopicName(GetDefaultTopicStructure(FxExposureSnapshotTopic), GetDefaultAppSettingName(FxExposureSnapshotTopic));

        public const string FxHedgeDetailSnapshotTopic = "FxHedgeDetailSnapshot";
        public static string FxHedgeDetailSnapshotTopicName =>
            GetServerAwareTopicName(
                GetDefaultTopicStructure(FxHedgeDetailSnapshotTopic),
                GetDefaultAppSettingName(FxHedgeDetailSnapshotTopic));

        public const string ForceRecalculatePositionsTopic = "ForceRecalculatePositions";
        public static string ForceRecalculatePositionsTopicName =>
            GetServerAwareTopicName(
                GetDefaultTopicStructure(ForceRecalculatePositionsTopic),
                GetDefaultAppSettingName(ForceRecalculatePositionsTopic));

        private static string GetServerAwareTopicName(string defaultTopicNameSuffix, string topicAppSettingName)
        {
            TopicDefinition topicDef = new TopicDefinition
            {
                DefaultTopicNameSuffix = defaultTopicNameSuffix, TopicAppSettingName = topicAppSettingName
            };

            return GetServerAwareTopicName(topicDef);
        }

        private static string GetServerAwareTopicName(
            string defaultTopicNameSuffix,
            string topicAppSettingName,
            string subsystem)
        {
            return GetServerAwareTopicName(GetTopic(defaultTopicNameSuffix, topicAppSettingName, subsystem));
        }

        private static TopicDefinition GetTopic(
            string defaultTopicNameSuffix,
            string topicAppSettingName,
            string subsystem)
        {
            TopicDefinition topicDef = new TopicDefinition
            {
                DefaultTopicNameSuffix = defaultTopicNameSuffix,
                TopicAppSettingName = topicAppSettingName,
                SubsystemName = subsystem
            };
            
            return topicDef;
        }

        public static string GetServerAwareTopicName(TopicDefinition topicDef)
        {
            string topicName = GetTopicName(topicDef);

            return GetServerAwareTopicNameWithPrefix(topicName, ServerPrefix ?? "", topicDef.SubsystemName);
        }

        private static string GetTopicName(TopicDefinition topicDef)
        {
            return topicDef.HasTopicAppSettingName
                ? GetTopicName(topicDef.DefaultTopicNameSuffix, topicDef.TopicAppSettingName, topicDef.Environment)
                : GetTopicName(topicDef.DefaultTopicNameSuffix, topicDef.Environment);
        }

        private static string GetTopicName(
            string defaultTopicNameSuffix,
            string topicAppSettingName,
            string environment)
        {
            return ConfigurationManager.AppSettings[topicAppSettingName]
                   ?? GetTopicName(defaultTopicNameSuffix, environment);
        }

        private static string GetTopicName(string defaultTopicNameSuffix, string environment)
        {
            return string.Format("MND/{0}{1}", environment, defaultTopicNameSuffix);
        }

        /// <summary>
        /// This method only works for topics using the default topic app setting format.
        /// </summary>
        /// <param name="topicName"></param>
        /// <returns></returns>
        public static string[] GetAllServerAwareTopicNames(string topicName)
        {
            string baseTopicName = GetDefaultTopicStructure(topicName);
            string defaultAppsetting = GetDefaultAppSettingName(topicName);

            return ServerPrefixes.Any()
                ? GetAllServerAwareTopicNames(GetTopic(baseTopicName, defaultAppsetting, AppPrefix))
                : new[] { GetServerAwareTopicName(baseTopicName, defaultAppsetting, AppPrefix) };
        }

        private static string[] GetAllServerAwareTopicNames(TopicDefinition topicDef)
        {
            string topicName = GetTopicName(topicDef);

            return ServerPrefixes.Aggregate(
                new List<string>(),
                (topics, serverPrefix) => topics.AddF(
                    GetServerAwareTopicNameWithPrefix(
                        topicName,
                        serverPrefix,
                        topicDef.SubsystemName))).ToArray();
        }

        public static string GetClientIndividualTopicName(string topicName, Guid clientGuid)
        {
            return topicName + "/" + clientGuid;
        }

        public static string GetServerAwareTopicNameWithPrefix(
            string topicName,
            string serverPrefix,
            string subsystemName = AppPrefix)
        {
            if (serverPrefix != string.Empty)
            {
                serverPrefix = "-" + serverPrefix;
            }

            string replacement = string.Format(@"/{0}{1}/", subsystemName, serverPrefix);
            string regexp = string.Format(@"/{0}/", subsystemName);

            Match match = Regex.Match(topicName, regexp);

            if (match.Success)
            {
                return Regex.Replace(topicName, regexp, replacement);
            }

            throw new Exception(string.Format("Topic name [{0}] does not contain [{1}] part.", topicName, regexp));
        }
    }
}