using System;
using System.Collections.Generic;
using Mandara.Entities;

namespace Mandara.Business.Bus
{
    public class TradesChangedEventArgs : EventArgs
    {
        public TradeCapture TradeCapture { get; set; }

        public TradesChangedEventArgs(TradeCapture tradeCapture)
        {
            TradeCapture = tradeCapture;
        }
    }
}