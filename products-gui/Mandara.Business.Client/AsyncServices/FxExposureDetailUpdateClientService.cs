using Mandara.Business.Bus;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages.Trades;
using Mandara.Entities.Trades;
using Ninject.Extensions.Logging;
using System;
using System.Threading;
using Mandara.Extensions.Option;

namespace Mandara.Business.AsyncServices
{
    public class FxExposureDetailUpdateClientService : QueuePollingAsyncService<FxExposureDetailUpdateMessage>
    {
        public FxExposureDetailUpdateClientService(CommandManager commandManager, BusClient busClient, ILogger log)
            : base(commandManager, busClient, log,busClient.PendingFxExposureDetailUpdates)
        {
        }

        protected override void DoWork()
        {
            TryGetResult<FxExposureDetailUpdateMessage> message = PollForNextElement();

            if (!message.HasValue)
            {
                return;
            }

            if (message.Value.FxExposureDetails != null)
            {
                foreach (FxExposureDetail fxExposureDetail in message.Value.FxExposureDetails)
                {
                    BusClient.OnFxExposureDetailChanged(fxExposureDetail);
                }
            }
        }
    }
}