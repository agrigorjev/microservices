using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages.Pnl
{
    public class TradesPnlOnDateSnapshotMessage : SnapshotMessageBase
    {
        public DateTime LivePriceSnapshotDatetime { get; set; }

        public DateTime FilterFrom { get; set; }
        public DateTime FilterTo { get; set; }

        public List<KeyValuePair<int, Money>> TradesPnl { get; set; }
    }
}