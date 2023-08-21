using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandara.Entities.Trades
{
    public class FxExposureTradeView : TradeView
    {
        public decimal Position { get; set; }
        public decimal ForwardPosition { get; set; }
        public decimal InitialExposure { get; set; }
    }
}
