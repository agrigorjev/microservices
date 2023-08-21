using Mandara.Business.Bus.Messages.Base;
using System;

namespace Mandara.Business.Bus.Messages.Pnl
{
    [Serializable]
    public abstract class PnLSnapshotMessage : SnapshotMessageBase
    {
        public DateTime? PriceDate { get; set; }
        public bool IsLiveMode { get; set; }
        public int? PortfolioId { get; set; }
        public DateTime? LivePriceSnapshotDatetime { get; set; }
        public string Currency { get; set; }
    }
}
