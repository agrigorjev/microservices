using Mandara.Entities.Dto;

namespace Mandara.Entities.MatchingDummies
{
    public class MatchingDummiesObject : TradeCaptureDto
    {
        public MatchingDummiesObject()
        {
            ParentTradeCapture = new ParentTradeCaptureCollection(); 
        }

        public ParentTradeCaptureCollection ParentTradeCapture { get; set; }
    }
}