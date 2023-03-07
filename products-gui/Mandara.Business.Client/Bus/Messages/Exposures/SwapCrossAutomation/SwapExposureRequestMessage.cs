using System;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.Enums;

namespace Mandara.Business.Bus.Messages.Exposures.SwapCrossAutomation
{
    public class SwapExposureRequestMessage : MessageBase
    {
        public int? SourcePortfolioId { get; set; }
        public DateTime CurrentMonth { get; set; }
        public BaseMonth BaseMonth { get; set; }
    }
}