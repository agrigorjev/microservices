using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.AdmAlerts
{
    public class AdmAlertsSnapshotRequestMessage : MessageBase
    {
        public string IrmUserName { get; set; }
    }
}