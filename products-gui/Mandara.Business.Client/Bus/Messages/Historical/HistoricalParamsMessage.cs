using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Json;
using Mandara.Entities;
using Newtonsoft.Json;

namespace Mandara.Business.Bus.Messages.Historical
{
    [Serializable]
    public class HistoricalParamsMessage : SnapshotMessageBase
    {
        [JsonProperty(ItemConverterType = typeof(SourceDataKeyValuePairConverter))]
        public List<KeyValuePair<DateTime, int>> SourceDataTypeMap { get; set; }
        public List<string> Accounts { get; set; }
        public List<Exchange> Exchanges { get; set; }
        public List<string> Currencies { get; set; }
    }
}