using Mandara.Entities.ProductDefinition;

namespace Mandara.RiskMgmtTool.Spreader.MonthlySpreader
{
    public class MonthlySpread
    {
        public DateString Month { get; set; }
        public decimal? FuturesEquivalent { get; set; }
        public string Spreads { get; set; }
        public decimal JALSpreads { get; set; }

        public decimal BookFuturesEquivalent { get; set; }
        public decimal TotalJALSpreads { get; set; }
    }
}