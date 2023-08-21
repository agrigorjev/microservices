using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Portfolio
{
    public class PortfolioEditMessage : MessageBase
    {
        public Entities.Portfolio Portfolio { get; set; }
        public string Result { get; set; }
    }
}