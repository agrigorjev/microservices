using System;
using System.ComponentModel;
using Mandara.Entities.Enums;

namespace Mandara.Entities.Trades
{
    [Serializable]
    public class PositionsTradeView : TradeView
    {
        public decimal ReportAmount { get; set; }

        [Browsable(false)]
        public decimal ReportAmountInner { get; set; }
        [Browsable(false)]
        public TradeMessageType MessageType { get; set; }
       
        public int ColorMk { get; set; }
    }
}