using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Bus.Messages.Positions;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages.TradeImpact
{
    public class TradeImpactResponseMessageDto : MessageBase
    {
        public string ErrorMessage { get; set; }
        public decimal Pnl { get; set; }
        public decimal? LivePrice1 { get; set; }
        public decimal? LivePrice2 { get; set; }
        public List<CalculationDetailDto> Positions { get; set; }
        public decimal? TradeBrokerage { get; set; }
    }
}