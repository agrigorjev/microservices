using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.Calculation;
using System;
using System.Collections.Generic;

namespace Mandara.Business.Bus.Messages.Pnl
{

    public class PnlUpdateMessage : MessageBase
    {
        public DateTime? SnapshotDatetime { get; set; }

        //        [JsonProperty(ItemConverterType = typeof(PnlKeyValuePairConverter))]
        public List<KeyValuePair<int, List<KeyValuePair<string, PnlData>>>> PnlByCurrencyByPortfolio { get; set; }

        public decimal? OvernightAbnPnl { get; set; }
    }


}