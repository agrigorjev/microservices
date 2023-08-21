using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages
{
    public class PriceTimestampsRequestMessage : MessageBase
    {
        public DateTime PriceDate { get; set; }
    }

    public class PriceTimestampsResponseMessage : MessageBase
    {
        public List<TimeSpan> PriceTimestamps { get; set; }

        public TimeSpan? SnapshotsEod { get; set; }
    }
}