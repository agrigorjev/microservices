using System.Collections.Generic;
using Mandara.Entities.Calculation;
using Mandara.Entities.Trades;

namespace Mandara.Business.Bus.Messages.Positions
{
    public class PositionsWithTradesSnapshotMessage : PositionsSnapshotMessageBase
    {
        public List<CalculationDetail> Positions { get; set; }
        public List<PositionsTradeView> Trades { get; set; }
    }
}