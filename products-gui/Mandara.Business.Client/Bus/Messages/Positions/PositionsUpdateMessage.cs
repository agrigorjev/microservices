using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.Calculation;

namespace Mandara.Business.Bus.Messages.Positions
{
    public class PositionsUpdateMessage : MessageBase
    {
        public List<CalculationDetail> Positions { get; set; }
    }
}