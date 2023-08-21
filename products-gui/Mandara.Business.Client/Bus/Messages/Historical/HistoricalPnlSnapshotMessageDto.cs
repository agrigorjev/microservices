using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;
using Mandara.Entities.Calculation;
using Mandara.Entities.Import;

namespace Mandara.Business.Bus.Messages.Historical
{
    public class HistoricalPnlSnapshotMessageDto : SnapshotMessageBase
    {
        public DateTime SourceDataDate { get; set; }
        public DateTime RiskDate { get; set; }
        public DateTime PriceDate { get; set; }
        public SourceDataType DataType { get; set; }
        public string AccountNumber { get; set; }
        public string ExchangeValue { get; set; }
        public List<ProductPriceDetailDto> ExternalPrices { get; set; }

        public List<PnlCalculationDetail> HistoricalPnl { get; set; }
    }
}