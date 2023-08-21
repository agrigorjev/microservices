using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Bus.Messages.Trades;
using Mandara.Entities.Dto;

namespace Mandara.Business.Bus.Messages.ClosingPositions
{
    public class ClosingPositionsPlaceTradesRequestMessageDto : MessageBase
    {
        public string AuthorizedUsername { get; set; }
        public List<TradeCaptureDto> Trades { get; set; }
    }
}