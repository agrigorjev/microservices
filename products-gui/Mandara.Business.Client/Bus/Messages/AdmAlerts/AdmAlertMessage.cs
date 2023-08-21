using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.AdmAlerts
{
    public class AdmAlertMessage : MessageBase
    {
        public AdmAlertDto AdmAlert { get; set; }
    }
}