using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Json;
using Mandara.Entities.Dto;
using Newtonsoft.Json;

namespace Mandara.Business.Bus.Messages.Trades
{
    public class OfficialProductNameMessage : MessageBase
    {
        public List<ProductDto2> Products { get; set; }
        [JsonProperty(ItemConverterType = typeof(IdNameKeyValuePairConverter))]
        public List<KeyValuePair<int, string>> OfficialNames { get; set; }
        public string ErrorMessage{get; set;}
    }
}
