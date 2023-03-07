namespace Mandara.Business.Authorization
{
    public enum PermissionType
    {
        [EntityId(1)]
        LaunchRiskTool = 1,
        [EntityId(2)]
        LaunchProductMgmtTool = 2,
        [EntityId(3)]
        RiskToolWriteAccess = 4,
        [EntityId(4)]
        ProductMgmtToolWriteAccess = 8,

        [EntityId(5)]
        Administrator = 16,
        [EntityId(6)]
        SuperAdministrator = 32,
        [EntityId(7)]
        LaunchHAL = 64,
        [EntityId(8)]
        LaunchVHAL = 128,

        [EntityId(9)]
        ViewCumulativePnl = 256,

        [EntityId(10)]
        UseProductBreakdown = 512,
    }
}