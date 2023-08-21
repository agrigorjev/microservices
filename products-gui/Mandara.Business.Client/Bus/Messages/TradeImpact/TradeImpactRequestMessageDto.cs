using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.Dto;

namespace Mandara.Business.Bus.Messages.TradeImpact
{
    public class TradeImpactRequestMessageDto : MessageBase
    {
        public List<TradeCaptureDto> TradeCaptures { get; set; }
    }
}