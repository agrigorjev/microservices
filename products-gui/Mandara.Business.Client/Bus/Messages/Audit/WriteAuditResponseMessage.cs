using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Audit
{
    public class WriteAuditResponseMessage : MessageBase
    {
        public string Response { get; set; } 
    }
}