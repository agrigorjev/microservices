using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.Calculation;

namespace Mandara.Business.Bus.Messages.ClosingPositions
{
    public class ClosingPositionsTradesRequestMessage : MessageBase
    {
        public int SourcePortfolioId { get; set; }
        public int DestinationPortfolioId { get; set; }
        public List<ClosingPosition> ClosingPositions { get; set; }
    }
}