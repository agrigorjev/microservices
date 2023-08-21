using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;
using Mandara.Entities.Dto;

namespace Mandara.Business.Bus.Messages.Trades
{
    public class TradesUpdateMessageDto : MessageBase
    {
        public List<TradeCaptureDto> TradeCaptures { get; set; }
    }
}