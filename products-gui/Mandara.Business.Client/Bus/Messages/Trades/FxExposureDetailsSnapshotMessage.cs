using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.Trades;

namespace Mandara.Business.Bus.Messages.Trades
{
    public class FxExposureDetailsSnapshotMessage : SnapshotMessageBase
    {
        public List<FxExposureDetail> Details { get; set; } 
    }
}
