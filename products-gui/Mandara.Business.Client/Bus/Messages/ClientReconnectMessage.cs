using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages
{
    public class ClientReconnectMessage : MessageBase
    {
        public string Prefix { get; set; }
    }
}