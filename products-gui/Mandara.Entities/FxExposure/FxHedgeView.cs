using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandara.Entities.FxExposure
{
    public class FxHedgeView
    {
        public DateTime ExpiryDate { get; set; }
        public decimal RollOffExposure { get; set; }
        public decimal SpotExecuted { get; set; }
        public decimal Unhedged { get; set; }
    }
}
