using System.Collections.Generic;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages;
using Mandara.Entities;

namespace Mandara.Business.Bus.Commands
{
    public class SendTradeSupportAlertsCommand : BusCommandBase
    {
        private readonly List<TradeSupportAlert> _alerts;

        public SendTradeSupportAlertsCommand(List<TradeSupportAlert> alerts)
        {
            _alerts = alerts;

            TopicName = InformaticaHelper.TradeSupportUpdateTopicName;
        }

        public override void Execute()
        {
            SendMessage(new TradeSupportAlertsMessage
            {
                NotAcknowledgedAlerts = _alerts
            });
        }
    }
}