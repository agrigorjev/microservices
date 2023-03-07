using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.AdmAlerts
{
    public class AcknowledgeAlertResponseMessage : MessageBase
    {
        public string TriggerKey { get; set; }
        public string ErrorMessage { get; set; }
    }
}