using Mandara.Entities.Calculation;
using System;
using System.Collections.Generic;

namespace Mandara.Business.Bus.Messages.Pnl
{
    [Serializable]
    public class PnLByProductSnapshotMessage : PnLSnapshotMessage
    {
        public List<OvernightAndLivePnLCalculationDetail> OvernightAndLivePnLPositions { get; set; }
    }
}