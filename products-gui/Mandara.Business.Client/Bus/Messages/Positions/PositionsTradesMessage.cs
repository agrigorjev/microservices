using System;
using System.Collections.Generic;
using Mandara.Entities.Trades;

namespace Mandara.Business.Bus.Messages.Positions
{
    [Serializable]
    public class PositionsTradesMessage : PositionsSnapshotMessageBase
    {
        public List<string> PositionsKeys { get; set; }
        public List<DateTime> DailyTradesDays { get; set; }

        public List<PositionsTradeView> PositionsTrades { get; set; }
    }
}