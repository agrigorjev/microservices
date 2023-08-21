using AutoMapper;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;
using System.Collections.Generic;

namespace Mandara.Business.Bus.Messages.ClosingPositions
{
    public class ClosingPositionsTradesResponseMessage : MessageBase
    {
        public List<TradeCapture> Trades { get; set; }

        public string ErrorMessage { get; set; }
    }
}