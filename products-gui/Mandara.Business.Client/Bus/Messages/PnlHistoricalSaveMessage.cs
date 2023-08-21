using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages
{
    public class PnlHistoricalSaveMessageArgs: MessageBase
    {
        public DateTime SnapshotDate { get; set; }

        public List<PnlReportRowData> PnlReportRowCollection { get; set; }
    }

    public class PnlHistoricalSaveMessage : MessageBase
    {
        public bool ResultSave { get; set; }
    }

}