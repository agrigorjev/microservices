using AutoMapper;
using Mandara.Entities.Calculation;
using System.Collections.Generic;

namespace Mandara.Business.Bus.Messages.TradeAdd
{
    public class TradeAddImpact
    {
        public decimal Pnl { get; private set; }
        public decimal? LivePrice1 { get; private set; }
        public decimal? LivePrice2 { get; private set; }
        public List<CalculationDetail> Positions { get; private set; }

        public List<CalculationError> CalcErrors { get; private set; }
        public decimal? TradeBrokerage { get; private set; }
        public decimal? Volume2 { get; private set; }

        public static TradeAddImpact Default { get;  set; }

        static TradeAddImpact()
        {
            Default = new TradeAddImpact(
                0M,
                0M,
                0M,
                new List<CalculationDetail>(),
                new List<CalculationError>(),
                0M,
                0M);
        }

        public TradeAddImpact()
        {
            Positions = new List<CalculationDetail>();
            CalcErrors = new List<CalculationError>();
        }

        public TradeAddImpact(
            decimal pnl,
            decimal? livePrice1,
            decimal? livePrice2,
            List<CalculationDetail> positions,
            List<CalculationError> calcErrors,
            decimal? tradeBrokerage,
            decimal? volume2)
        {
            Pnl = pnl;
            LivePrice1 = livePrice1;
            LivePrice2 = livePrice2;
            Positions = positions;
            CalcErrors = calcErrors;
            TradeBrokerage = tradeBrokerage;
            Volume2 = volume2;
        }

        public bool IsDefault()
        {
            return this == Default;
        }
    }
}