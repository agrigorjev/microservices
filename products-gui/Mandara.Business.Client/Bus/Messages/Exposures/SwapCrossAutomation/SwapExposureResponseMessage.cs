using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Client.Bus.Messages.Exposures;

namespace Mandara.Business.Bus.Messages.Exposures.SwapCrossAutomation
{
    public class SwapExposureResponseMessage : MessageWithErrorBase
    {
        public List<Exposure> Exposures { get; set; }

        public override void OnErrorSet()
        {
        }
    }
}