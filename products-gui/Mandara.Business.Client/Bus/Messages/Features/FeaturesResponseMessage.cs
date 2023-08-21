using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages.Features
{
    public class FeaturesResponseMessage : MessageBase
    {
        public IEnumerable<Feature> Features { get; set; }
    }
}