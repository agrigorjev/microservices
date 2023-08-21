using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Portfolio
{
    public class PortfolioUpdateMessage : MessageBase
    {
        public List<Entities.Portfolio> Portfolios { get; set; }
    }
}