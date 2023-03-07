using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.PricingReport
{
    public class TasSnapshotMessageDto : SnapshotMessageBase
    {
        public int? PortfolioId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? PositionDate { get; set; }
        public bool IncludeFutures { get; set; }

        public List<PricingCalculationDetailDto> TasPositions { get; set; }
    }
}