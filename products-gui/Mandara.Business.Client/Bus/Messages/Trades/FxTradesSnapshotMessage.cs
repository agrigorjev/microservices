using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages.Trades
{
    public class FxTradesSnapshotMessage : SnapshotMessageBase
    {
        // message parameters
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // message data
        public List<FxTrade> FxTrades { get; set; }
    }
}