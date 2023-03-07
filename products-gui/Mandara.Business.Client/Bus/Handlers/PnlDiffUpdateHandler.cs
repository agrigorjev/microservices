using com.latencybusters.lbm;
using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages.Pnl;
using System;
using System.Collections.Generic;

namespace Mandara.Business.Bus.Handlers
{
    public class PnlDiffUpdateHandler : MessageHandler<PnlDiffMessage>
    {
        public class PnlDiffEventArgs : EventArgs
        {
            public PnlDiffMessage Message { get; set; }
        }

        protected override void Handle(PnlDiffMessage message)
        {
            BusClient.Instance.OnPnlDiffUpdated(new PnlDiffEventArgs { Message = message });
        }
    }
}
