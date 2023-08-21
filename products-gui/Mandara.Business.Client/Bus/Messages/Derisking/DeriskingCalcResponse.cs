using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages.Derisking
{
    public class DeriskingCalcResponse : MessageBase
    {
        public decimal Var { get; set; }
        public decimal MinVar { get; set; }
        public List<TradeScenario> Trades { get; set; }
    }
}
