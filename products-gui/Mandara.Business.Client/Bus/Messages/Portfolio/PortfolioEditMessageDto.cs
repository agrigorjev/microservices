using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Portfolio
{
    public class PortfolioEditMessageDto : MessageBase
    {
        public PortfolioDto Portfolio { get; set; }
        public string Result { get; set; } 
    }
}