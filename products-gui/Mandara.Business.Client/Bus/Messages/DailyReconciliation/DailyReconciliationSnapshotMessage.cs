using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;
using Mandara.Entities.Calculation;

namespace Mandara.Business.Bus.Messages.DailyReconciliation
{
    public class DailyReconciliationSnapshotMessage : SnapshotMessageBase
    {
        public DateTime? DailyDate { get; set; }

        public DateTime? RiskDate { get; set; }

        public DateTime? SourceDate { get; set; }

        public SourceDataType DataType { get; set; }

        public List<CalculationDetail> HistoricalPositions { get; set; }

        public List<CalculationDetail> LivePositions { get; set; }
    }
}
