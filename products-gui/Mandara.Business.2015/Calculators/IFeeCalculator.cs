using Mandara.Entities;

namespace Mandara.Business.Calculators
{
    public interface IFeeCalculator
    {
        void CalculateBasicFees(TradeCapture trade, Product product);
        void CalculateFeesForStandardTradeInclRebate(TradeCapture trade);
        void CalculateFeesForStandardTradeInclRebate(TradeCapture trade, Product product);
        void CalculateFeesWithParentSpreadInclRebate(TradeCapture trade);
        void CalculateFeesWithParentSpreadInclRebate(TradeCapture trade, Product product);
        decimal? CalcBlock(TradeCapture trade, Product product);
        decimal? CalcClearing(TradeCapture trade, Product product);
        decimal? CalcCommission(TradeCapture trade, Product product);
        decimal? CalcExchange(TradeCapture trade, Product product);
        decimal? CalcNfa(TradeCapture trade, Product product);
    }
}