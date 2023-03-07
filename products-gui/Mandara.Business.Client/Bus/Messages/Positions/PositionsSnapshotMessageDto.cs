using System.Collections.Generic;

namespace Mandara.Business.Bus.Messages.Positions
{
    public class PositionsSnapshotMessageDto : PositionsSnapshotMessageBase
    {
        public List<CalculationDetailDto> Positions { get; set; }
        public int LastSequenceNumber { get; set; }

        public override string ToString()
        {
            return $"{nameof(SnapshotId)}: {SnapshotId}  {nameof(RiskDate)}: {RiskDate} {nameof(Positions)} Count: {Positions?.Count}, {nameof(LastSequenceNumber)}: {LastSequenceNumber}";
        }
    }
}