using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages.ClosingPositions
{
    public class ClosingPositionsPlaceTradesRequestMessage : MessageBase
    {
        public string AuthorizedUsername { get; set; }
        public List<TradeCapture> Trades { get; set; }
    }
}