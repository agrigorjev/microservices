using System.Collections.Generic;

namespace Mandara.Entities.Enums
{
    public static class ReportNames
    {
        public const string BigBuckets = "BigBuckets";
        public const string Cem = "Cem";
        public const string FxPositionAlertChecker = "FxPositionAlertChecker";
        public const string FxPositionAlertSender = "FxPositionAlertSender";
        public const string GeneratePosition = "GeneratePosition";
        public const string IceSettlementPrice = "IceSettlementPrice";
        public const string Margin = "Margin";
        public const string ManualTrades = "ManualTrades";
        public const string OhlcPattern = "OHLCPattern";
        public const string OPandPrice = "OPandPrice";
        public const string OpSaccr = "OpSaccr";
        public const string PnL = "PnL";
        public const string PnLByPortfolio = "PnLByPortfolio";
        public const string PnLByProduct = "PnLByProduct";
        public const string PnLSpotVsForward = "PnLSpotVsForward";
        public const string PnlVarStats = "PnlVarStats";
        public const string Pricing = "Pricing";
        public const string RetrieveTrx = "RetrieveTrx";
        public const string SoftwareUsageReport = "SoftwareUsageReport";
        public const string UploadReports = "UploadReports";
        public const string VaRUsage = "VaRUsage";

        public static readonly HashSet<string> Reports = new HashSet<string>()
        {
            BigBuckets,
            Cem,
            FxPositionAlertChecker,
            FxPositionAlertSender,
            GeneratePosition,
            IceSettlementPrice,
            Margin,
            ManualTrades,
            OhlcPattern,
            OPandPrice,
            OpSaccr,
            PnL,
            PnLByPortfolio,
            PnLByProduct,
            PnLSpotVsForward,
            PnlVarStats,
            Pricing,
            RetrieveTrx,
            SoftwareUsageReport,
            UploadReports,
            VaRUsage,
        };
    }
}
