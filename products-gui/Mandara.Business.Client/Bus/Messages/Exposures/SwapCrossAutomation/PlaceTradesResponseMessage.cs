using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Exposures.SwapCrossAutomation
{
    public class PlaceTradesResponseMessage : MessageWithErrorBase
    {
        public string Response { get; set; }
        public override void OnErrorSet()
        {
        }
    }
}