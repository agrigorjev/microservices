using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.Calculation;

namespace Mandara.Business.Bus.Messages
{
    public class RolloffNotificationMessage : MessageBase
    {
        public List<RolloffDetail> RolloffDetails { get; set; }
    }
}