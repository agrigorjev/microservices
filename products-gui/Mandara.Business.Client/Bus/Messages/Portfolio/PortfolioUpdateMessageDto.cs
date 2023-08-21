using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Portfolio
{
    public class PortfolioUpdateMessageDto : MessageBase
    {
        public List<PortfolioDto> Portfolios { get; set; }
    }
}