using System.Collections.Generic;
using AutoMapper;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.Calculation;

namespace Mandara.Business.Bus.Messages.TradeImpact
{
    public class TradeImpactResponseMessage : MessageBase
    {
        public string ErrorMessage { get; set; }
        public decimal Pnl { get; set; }
        public decimal? LivePrice1 { get; set; }
        public decimal? LivePrice2 { get; set; }
        public List<CalculationDetail> Positions { get; set; }

        public List<CalculationError> CalcErrors { get; set; }
        public decimal? TradeBrokerage { get; set; }
    }
}