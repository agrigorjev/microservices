using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.AdmAlerts
{
    public class AdmAlertsSnapshotResponseMessage : MessageBase
    {
        public List<AdmAlertDto> AdmAlerts { get; set; }
    }
}