using System;
using System.Collections.Generic;
using Mandara.Entities;

namespace Mandara.Business.Bus
{
    public class FxTradesChangedEventArgs : EventArgs
    {
        public FxTrade FxTrade { get; set; }

        public FxTradesChangedEventArgs(FxTrade fxTrade)
        {
            FxTrade = fxTrade;
        }
    }
}