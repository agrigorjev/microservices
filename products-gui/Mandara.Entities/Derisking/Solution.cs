using System.Collections.Generic;
using Mandara.Entities.Calculation;

namespace Mandara.Entities
{
    public class Solution
    {
        public int SolutionId { get; set; }
        
        public List<TradeScenario> TradeScenarios { get; set; }

        public List<CalculationDetail> Impact { get; set; }

        public double VarValue { get; set; }
    }
}