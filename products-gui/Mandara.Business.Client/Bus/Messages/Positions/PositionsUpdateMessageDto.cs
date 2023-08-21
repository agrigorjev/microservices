using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.Calculation;

namespace Mandara.Business.Bus.Messages.Positions
{
    public class PositionsUpdateMessageDto : MessageBase
    {
        public List<CalculationDetailDto> Positions { get; set; }
    }
}