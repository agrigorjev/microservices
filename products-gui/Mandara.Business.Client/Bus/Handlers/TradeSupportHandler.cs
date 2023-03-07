using System.Collections.Generic;
using AutoMapper;
using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages;
using Mandara.Business.Bus.Messages.Trades;
using Mandara.Entities;
using com.latencybusters.lbm;
using Mandara.Entities.ErrorReporting;
using System;
using System.Linq;

namespace Mandara.Business.Bus.Handlers
{
    /// <summary>
    /// Runtime support messages handle
    /// </summary>
 
    public class TradeSupportHandler : MessageHandler<TradeSupportMessage>
    {
        protected override void Handle(TradeSupportMessage message)
        {
            
            if (HasUnacknowledgedAlerts())
            {
                message.NotAcknowledgedAlerts.ForEach(a => BusClient.Instance.AddTradeSupport(a));
            }

            if (HasAcknowledgedAlert())
            {
                BusClient.Instance.AcknowledgeTradeSupport(message.AcknowledgedAlert);
            }

            bool HasUnacknowledgedAlerts()
            {
                return message.NotAcknowledgedAlerts != null && message.NotAcknowledgedAlerts.Any();
            }

            bool HasAcknowledgedAlert()
            {
                return message.AcknowledgedAlert != Guid.Empty;
            }
        }
    }
}