using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.Dto;

namespace Mandara.Business.Bus.Messages.Exposures.SwapCrossAutomation
{
    public class TransferTradesResponseMessageDto : MessageBase
    {
        public Dictionary<string, List<List<TradeCaptureDto>>> Trades { get; set; }
    }
}