using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.Dto;
using System.Collections.Generic;

namespace Mandara.Business.Bus.Messages.Trades
{
    public class FxTradesUpdateMessage : MessageBase
    {
        public List<FxTradeDto> FxTrades { get; set; }
    }
}