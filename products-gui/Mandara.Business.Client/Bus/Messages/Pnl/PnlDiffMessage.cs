using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Pnl
{
    public class PnlDiffMessage : MessageBase
    {
        public int PortfolioId { get; set; }
        public double PnlDiff { get; set; }
    }
}
