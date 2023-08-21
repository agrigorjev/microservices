using System.Collections.Generic;
using Mandara.Entities.Calculation;

namespace Mandara.Business.Bus.Messages.Positions
{
    public class PositionsSnapshotMessage : PositionsSnapshotMessageBase
    {
        public List<CalculationDetail> Positions { get; set; }
        public int LastSequenceNumber { get; set; }
    }
}