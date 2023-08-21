using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages.Historical
{
    public class HistoricalPositionsSnapshotMessageDto : SnapshotMessageBase
    {
        public DateTime SourceDataDate { get; set; }
        public DateTime RiskDate { get; set; }
        public SourceDataType DataType { get; set; }
        public string AccountNumber { get; set; }
        public string ExchangeValue { get; set; }

        public List<CalculationDetailDto3> HistoricalPositions { get; set; }
    }
}