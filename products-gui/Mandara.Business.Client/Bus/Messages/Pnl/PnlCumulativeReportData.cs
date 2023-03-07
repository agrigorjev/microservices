using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Pnl
{
    public class PnlCumulativeReportData : MessageBase
    {
        public PnlCumulativeReportData()
        {
            PnlCumulativeCollection = new List<PnlReportRowData>();
        }

        public List<PnlReportRowData> PnlCumulativeCollection { get; set; }
    }
}
