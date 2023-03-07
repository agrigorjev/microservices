using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Positions;

namespace Mandara.Business.Bus.Messages.TradeAdd
{
    public class TradeAddImpactDto
    {
        public decimal Pnl { get; set; }
        public decimal? LivePrice1 { get; set; }
        public decimal? LivePrice2 { get; set; }
        public List<CalculationDetailDto> Positions { get; set; }
        public decimal? TradeBrokerage { get; set; }
        public decimal? Volume2 { get; set; }
    }
}