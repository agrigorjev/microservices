using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Spreader
{
    public class SpreaderRequestMessage : MessageBase
    {
        public int ProductId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IncludeSimulatedPositions { get; set; }
        public List<SpreaderInput> ManualInput { get; set; }
        public List<SpreaderInput> PositionsInput { get; set; }
    }
}
