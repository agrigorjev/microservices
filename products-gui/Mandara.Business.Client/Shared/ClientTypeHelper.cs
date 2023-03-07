using System;
using System.Reflection;

namespace Mandara.Business.Shared
{
    public static class ClientTypeHelper
    {
        public static ClientType DetermineClientType()
        {
            if (Assembly.GetEntryAssembly().GetName().Name.StartsWith("Mandara.RiskMgmtTool", StringComparison.OrdinalIgnoreCase))
                return ClientType.IrmClient;

            if (Assembly.GetEntryAssembly().GetName().Name.StartsWith("Mandara.AdminAlertService", StringComparison.OrdinalIgnoreCase))
                return ClientType.AlertService;

            if (Assembly.GetEntryAssembly().GetName().Name.StartsWith("Mandara.ReportingService", StringComparison.OrdinalIgnoreCase))
                return ClientType.ReportingService;

            return ClientType.IrmClient;
        }
    }
}
