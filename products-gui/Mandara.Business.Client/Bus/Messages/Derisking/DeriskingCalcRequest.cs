using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Derisking
{
    public class DeriskingCalcRequest : MessageBase
    {
        public int PortfolioId { get; set; }
        public decimal ConfidenceLevel { get; set; }
        public int NumOfPicks { get; set; }
    }
}
