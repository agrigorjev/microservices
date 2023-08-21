using System;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages
{
    public class ClientHeartbeatMessage : MessageBase
    {
        public Guid ClientGuid { get; set; }

        public ClientType ClientType { get; set; }

        public string ServerPrefix { get; set; }

        public DateTime LastHeartbeatUtc { get; set; }
    }
}
