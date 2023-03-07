using System;
using System.Collections.Generic;
using Mandara.Entities.Enums;

namespace Mandara.Entities.Calculation
{
    public class PricingCalculationDetail
    {
        public Guid DetailId { get; set; }
        public DateTime CalculationDate { get; set; }
        public DateTime RolloffTime { get; set; }
        public String ProductCategory { get; set; }
        public String Product { get; set; }
        public DateTime Period { get; set; }
        public String Source { get; set; }
        public Decimal Amount { get; set; }
        public Int32 ProductId { get; set; }
        public Int32 SourceProductId { get; set; }
        public List<SourceDetail> SourceDetails { get; set; }
        public List<KeyValuePair<int, decimal>> SourceDetailAmounts { get; set; }
        public Dictionary<int, decimal> SourceDetailAmountsDict { get; set; }
        public List<int> PortfolioIds { get; set; } 

        public Product ProductReference { get; set; }
    }
}
