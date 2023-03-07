using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages.Pnl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Extensions.Logging;


namespace Mandara.Business.Bus.Handlers
{
    public class VarLatestUpdateHandler : MessageHandler<VarMessage>
    {
        private ILogger _logger = new NLogLoggerFactory().GetLogger("VarDelayLogger");
        public class VarDataEventArgs : EventArgs
        {
            public enum VarType : byte
            {
                v100 = 100,
                v99 = 99,
                v95 = 95
            }
            public List<VarMessage.Data> VarValues { get; set; }
        }

        protected override void Handle(VarMessage message)
        {
            if (message.HasTimeStamp())
            {
                _logger.Info("Var Delay was {Delay} second",message.ReceivedTimeDelay.TotalSeconds);
            }
            BusClient.Instance.OnVarLatestUpdated(new VarDataEventArgs() { VarValues = message.VarValues });
        }
    }
}
