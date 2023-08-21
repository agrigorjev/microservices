using Mandara.Business.Bus.Messages.Pnl;
using Mandara.Entities.Calculation;
using System;
using System.Collections.Generic;

namespace Mandara.Business.Bus.Messages
{
    [Serializable]
    public class OvernightPnlSnapshotMessage : PnLSnapshotMessage
    {
        public List<OvernightPnlCalculationDetail> OvernightPnlPositions { get; set; }
    }
}