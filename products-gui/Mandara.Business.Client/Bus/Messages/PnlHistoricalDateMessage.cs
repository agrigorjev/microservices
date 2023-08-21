using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Date;

namespace Mandara.Business.Bus.Messages
{
    public class PnlHistoricalDateMessageArgs : MessageBase
    {
        public DateTime ReportDate { get; set; }

        public PnlHistoricalDateMessageArgs(DateTime date)
        {
            ReportDate = date;
        }
    }

    /// <summary>
    /// </summary>
    public class PnlHistoricalDateMessage : MessageWithErrorBase
    {
        public PnlHistoricalDateMessage()
        {
            DateCollection = new List<DateTime>();
        }

        public List<DateTime> DateCollection { get; set; }

        public override void OnErrorSet()
        {
            DateCollection.Clear();
        }
    }

    public class PnlReportDateTime
    {
        public DateTime DateTime { get; set; }

        public override string ToString()
        {
            return DateTime.To24HourTime();
        }
    }
}