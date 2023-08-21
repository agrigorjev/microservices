using Mandara.Business.Bus.Messages.Base;
using Newtonsoft.Json;
using System;

namespace Mandara.Business.Bus.Messages.Reporting
{
    [Serializable]
    public class RunReportRequest : MessageBase
    {
        public string EntityName { get; }
        public ReportParams Report { get; }

        [JsonConstructor]
        public RunReportRequest(string entityName, ReportParams report)
        {
            EntityName = entityName;
            Report = report
                     ?? throw new ArgumentException(
                         "No report parameters have been provided.  A valid request cannot be constructed.",
                         nameof(report));
        }
    }
}
