using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.Trades;

namespace Mandara.Business.Bus.Messages.PricingReport
{
    public class TasCheckDetailResponseMessage : MessageBase
    {
        public List<TasCheckDetail> TasCheckDetails { get; set; }
    }
}