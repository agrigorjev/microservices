using System;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages
{
    public class PnlCumulativeArgs : MessageBase
    {
        public PnlCumulativeArgs()
        {

        }

        public PnlCumulativeArgs(DateTime from, DateTime to)
        {
            From = from;
            To = to;
            Guid = Guid.NewGuid();
        }

        public PnlCumulativeArgs(Guid id, DateTime from, DateTime to)
        {
            From = from;
            To = to;
            Guid = new Guid(id.ToString());
        }

        public Guid Guid { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }
    }
}
