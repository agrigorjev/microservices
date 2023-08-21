using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages
{
    public class TradeSupportAlertsMessage : MessageBase
    {
        public List<TradeSupportAlert> NotAcknowledgedAlerts { get; set; }
    }
}