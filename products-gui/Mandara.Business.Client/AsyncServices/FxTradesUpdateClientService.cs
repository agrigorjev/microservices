using Mandara.Business.Bus;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages.Trades;
using Mandara.Entities;
using Mandara.Entities.Dto;
using Mandara.Extensions.Option;
using Ninject.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.AsyncServices
{
    public class FxTradesUpdateClientService : QueuePollingAsyncService<FxTradesUpdateMessage>
    {
        public FxTradesUpdateClientService(CommandManager commandManager, BusClient busClient, ILogger log)
            : base(commandManager, busClient, log, busClient.PendingFxTradesUpdates)
        {
        }


        protected override void DoWork()
        {
            TryGetResult<FxTradesUpdateMessage> message = PollForNextElement();

            if (!message.HasValue)
            {
                return;
            }

            if (message.Value.FxTrades != null)
            {
                IEnumerable<FxTrade> fxTrades =
                    message.Value.FxTrades.Select(fxTradeDto => AutoMapper.Mapper.Map<FxTradeDto, FxTrade>(fxTradeDto));

                foreach (FxTrade fxTrade in fxTrades)
                {
                    BusClient.OnFxTradesChanged(fxTrade);
                }
            }
        }
    }
}