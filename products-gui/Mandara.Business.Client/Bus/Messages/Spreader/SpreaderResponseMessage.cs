using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Spreader
{
    public class SpreaderResponseMessage : MessageBase
    {
        public List<SpreaderOutput> Output { get; set; }
    }
}
