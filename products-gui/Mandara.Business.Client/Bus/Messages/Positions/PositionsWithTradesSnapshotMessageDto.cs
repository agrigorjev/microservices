using System.Collections.Generic;
using Mandara.Business.Client.Managers;
using Mandara.Entities.Trades;
using Newtonsoft.Json;

namespace Mandara.Business.Bus.Messages.Positions
{
    public class PositionsWithTradesSnapshotMessageDto : PositionsSnapshotMessageBase
    {
        [JsonIgnore]
        public List<CalculationDetailDto2> Positions { get; set; }
        [JsonIgnore]
        public List<PositionsTradeView> Trades { get; set; }

        
    }
}