using Mandara.Business.Bus.Messages.Base;
using Newtonsoft.Json;

namespace Mandara.Business.Bus.Messages.Reporting
{
    public class RunReportResult : MessageWithErrorBase
    {
        public string EntityName { get; private set; }
        public ReportParams Report { get; private set; }

        [JsonConstructor]
        public RunReportResult(string entityName, ReportParams report)
        {
            EntityName = entityName;
            Report = report;
        }

        public override void OnErrorSet()
        {
        }
    }
}
