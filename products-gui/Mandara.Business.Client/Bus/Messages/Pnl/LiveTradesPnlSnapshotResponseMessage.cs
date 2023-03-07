using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.Calculation;

namespace Mandara.Business.Bus.Messages.Pnl
{
    public class LiveTradesPnlSnapshotMessage : SnapshotMessageBase
    {
        public DateTime SnapshotDatetime { get; set; }
        public List<PnlCalculationDetail> Details { get; set; }
        public decimal PnlGrandTotal { get; set; }
    }
}