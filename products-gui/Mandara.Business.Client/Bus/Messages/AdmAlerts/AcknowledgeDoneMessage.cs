using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.AdmAlerts
{
    public class AcknowledgeDoneMessage : MessageBase
    {
        public string TriggerKey { get; set; }
    }
}