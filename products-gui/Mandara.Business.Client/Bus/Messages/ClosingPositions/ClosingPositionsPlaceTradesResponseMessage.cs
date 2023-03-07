using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.ClosingPositions
{
    public class ClosingPositionsPlaceTradesResponseMessage : MessageBase
    {
        public string ErrorMessage { get; set; }
    }
}