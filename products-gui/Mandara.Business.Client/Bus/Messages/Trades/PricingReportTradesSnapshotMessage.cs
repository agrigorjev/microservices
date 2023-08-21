using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages.Trades
{
    public class PricingReportTradesSnapshotMessage : SnapshotMessageBase
    {
        public List<Cell> Cells { get; set; }
        public List<TradeCapture> TradeCaptures { get; set; }
        public List<KeyValuePair<int, decimal?>> TradesAmounts { get; set; }
        public int PortfolioId { get; set; }
    }

    public class Cell
    {
        public DateTime Day { get; set; }
        public DateTime Month { get; set; }
        public int ProductId { get; set; }
    }
}