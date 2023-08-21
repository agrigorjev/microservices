using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;
using Mandara.Entities.Calculation;

namespace Mandara.Business.Bus.Messages.Historical
{
    public class HistoricalPositionsSnapshotMessage : SnapshotMessageBase
    {
        public DateTime SourceDataDate { get; set; }
        public DateTime RiskDate { get; set; }
        public SourceDataType DataType { get; set; }
        public string AccountNumber { get; set; }
        public string ExchangeValue { get; set; }

        public List<CalculationDetail> HistoricalPositions { get; set; }
    }
}