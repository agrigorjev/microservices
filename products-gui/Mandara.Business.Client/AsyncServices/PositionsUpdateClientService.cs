using Mandara.Business.Bus;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Bus.Messages.Positions;
using Mandara.Entities.Calculation;
using Mandara.Extensions.Option;
using Ninject.Extensions.Logging;
using System.Collections.Concurrent;
using System.Linq;

namespace Mandara.Business.AsyncServices
{
    public class PositionsUpdateClientService : QueuePollingAsyncService<PositionsUpdateMessage>
    {


        public PositionsUpdateClientService(CommandManager commandManager, BusClient busClient, ILogger log)
            : base(commandManager, busClient, log, busClient.PendingPositionsUpdates)
        {

        }

        protected override bool IsQueueReady()
        {
            return BusClient.PositionsReceived;
        }

        protected override void DoWork()
        {
            if (!IsQueueReady())
            {
                SleepForQueue();
                return;
            }

            TryGetResult<PositionsUpdateMessage> message = PollForNextElement();

            if (!message.HasValue)
            {
                return;
            }

            if ((message.Value.Positions?.Count ?? 0) == 0)
            {
                if (message.Value.StatusCode == MessageStatusCode.ResetData)
                {
                    BusClient.OnSequenceReset();
                }
            }
            else
            {
                _log.Info("Positions Update Started");
                int added = 0;
                int changed = 0;
                int removed = 0;

                foreach (CalculationDetail newDetail in message.Value.Positions)
                {
                    if (newDetail.SourceDetailAmounts != null)
                    {
                        newDetail.SourceDetailAmountsDict =
                            new ConcurrentDictionary<int, decimal>(newDetail.SourceDetailAmounts);
                    }

                    string key = newDetail.GetKey();
             
                    switch (message.Value.StatusCode)
                    {
                        case MessageStatusCode.AddData:
                        case MessageStatusCode.UpdateData:
                        {
                            BusClient.Positions.AddOrUpdate(
                                key,
                                id =>
                                {
                                    added++;
                                    return newDetail;
                                },
                                (id, existing) =>
                                {
                                    changed++;
                                    return newDetail;
                                });
                        }
                        break;

                        case MessageStatusCode.RemoveData:
                        {
                            removed++;

                            if (!BusClient.Positions.TryRemove(key, out CalculationDetail _))
                            {
                                    _log.Error("Remove position for Key {0} could not be matched", key);
                            }
                        }
                        break;

                    }
                }

                _log.Info("Positions Update Finished {0} added {1} changed {2} removed", added, changed, removed);

                if (added + changed + removed > 0)
                {
                    BusClient.RaisePositionChanged();
                }
            }
        }
    }
}