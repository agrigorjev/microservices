using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Bus.Messages.Historical;
using Mandara.Business.Bus.Messages.Positions;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages.DailyReconciliation
{
    public class DailyReconciliationSnapshotMessageDto : SnapshotMessageBase
    {
        public DateTime? DailyDate { get; set; }

        public DateTime? RiskDate { get; set; }

        public DateTime? SourceDate { get; set; }

        public SourceDataType DataType { get; set; }

        public List<CalculationDetailDto3> HistoricalPositions { get; set; }

        public List<CalculationDetailDto> LivePositions { get; set; }
    }
}
