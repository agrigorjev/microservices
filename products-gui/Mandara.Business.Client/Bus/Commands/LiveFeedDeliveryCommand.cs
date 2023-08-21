using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Bus.Messages.Positions;
using Mandara.Business.Bus.Messages.Trades;
using NLog;

namespace Mandara.Business.Bus.Commands
{
    public class LiveFeedDeliveryCommand : BusCommandBase
    {
        private readonly string _topicName;
        private readonly List<int> _sequences;
        public Logger Log { get; private set; }

        public LiveFeedDeliveryCommand(string topicName, List<int> sequences)
        {
            Log = LogManager.GetCurrentClassLogger();

            _topicName = topicName;
            _sequences = sequences;
        }

        public override void Execute()
        {
            Log.Debug(string.Format("Requesting live feed replay, topic [{0}], sequences [{1}]", _topicName,
                                    string.Join(", ", _sequences)));

            BusClient.Instance.GetLiveFeedSnapshot(_topicName, _sequences, Callback);
        }

        private void Callback(List<LiveFeedReplaySnapshotMessage> messages)
        {
            if (messages == null)
                return;

            List<MessageBase> dataMessages =
                messages.Where(x => x.DataMessages != null).SelectMany(x => x.DataMessages).ToList();

            if (dataMessages.Count > 0)
            {
                foreach (MessageBase message in dataMessages)
                {
                    TradesUpdateMessageDto tradesDto = message as TradesUpdateMessageDto;

                    if (tradesDto != null)
                    {
                        TradesUpdateMessage original = Mapper.Map<TradesUpdateMessage>(tradesDto);

                        Log.Debug("Replaying trades update message");
                        BusClient.Instance.PendingTradesUpdates.Enqueue(original);
                    }

                    PositionsUpdateMessageDto positionsDto = message as PositionsUpdateMessageDto;

                    if (positionsDto != null)
                    {
                        PositionsUpdateMessage original = Mapper.Map<PositionsUpdateMessage>(positionsDto);

                        Log.Debug("Replaying positions update message");
                        BusClient.Instance.PendingPositionsUpdates.Enqueue(original);
                    }

                    FxExposureDetailUpdateMessage fxExposureDetail = message as FxExposureDetailUpdateMessage;

                    if (fxExposureDetail != null)
                    {
                        Log.Debug("Replaying FX Exposure details update message");
                        BusClient.Instance.PendingFxExposureDetailUpdates.Enqueue(fxExposureDetail);
                    }
                }
            }
            else
            {
                Log.Warn("Resetting client sequence numbers");
                IoC.Get<BusClient>().PendingTradesUpdates.ResetSequence();
                IoC.Get<BusClient>().PendingPositionsUpdates.ResetSequence();
                IoC.Get<BusClient>().PendingFxExposureDetailUpdates.ResetSequence();

                CommandManager.AddCommand(new BusClientRestartCommand());
            }

        }
    }
}