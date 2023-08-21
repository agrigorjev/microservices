using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;
using System;
using System.Collections.Generic;

namespace Mandara.Business.Bus.Messages
{
    [Serializable]
    public class TradeSupportMessage : SnapshotMessageBase
    {
        public int DatesOffset { get; set; }
        public List<TradeSupportAlert> NotAcknowledgedAlerts { get; set; }
        public Guid AcknowledgedAlert { get; set; }

        public override void OnErrorSet()
        {
            NotAcknowledgedAlerts.Clear();
        }
    }
}