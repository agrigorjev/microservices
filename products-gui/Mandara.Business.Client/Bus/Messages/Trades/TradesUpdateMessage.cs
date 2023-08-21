using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages.Trades
{
    public class TradesUpdateMessage : MessageBase
    {
        public List<TradeCapture> TradeCaptures { get; set; }
    }
}