using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages.Derisking
{
    public class DeriskingCalcSnapshotMessage : SnapshotMessageBase
    {
        // request
        public int PortfolioId { get; set; }
        public decimal ConfidenceLevel { get; set; }
        public int NumOfPicks { get; set; }

        // response
        public decimal Var { get; set; }
        public decimal MinVar { get; set; }
        public List<TradeScenario> Trades { get; set; }
        public string ResponseString { get; set; }
    }
}