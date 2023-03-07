namespace Mandara.Entities.Enums
{
    public enum FixMessageType
    {
        Unknown,
        Logon,
        Logout,
        TradeCaptureReportRequest,
        TradeCaptureReportRequestAck,
        Heartbeat,
        ResendRequest,
        Reject,
        SecurityDefinition,
        SecurityDefinitionRequest,
        TestRequest,
        TradeCaptureReport,
        TradeCaptureReportAck,
        SequenceReset,
        ChangePassword,
        TradeCaptureHistoricalReportRequest,
        TradeCaptureHistoricalReportRequestAck,
        SubscriptionSignoffRequest
    }
}