using Mandara.Business.Bus.Messages.Base;
using Newtonsoft.Json;

namespace Mandara.Business.Bus.Messages.TradeAdd
{
    public class TradeAddImpactResponseMessageDto : MessageBase
    {
        [JsonProperty("Impact1")]
        public TradeAddImpactDto TradesImpact { get; set; }
        [JsonProperty("Impact2")]
        public TradeAddImpactDto TransferTradesImpact { get; set; }
        public string ErrorMessage { get; set; }
    }
}