using System;
using AutoMapper;
using com.latencybusters.lbm;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Bus.SnapshotReceivers;
using Ninject.Extensions.Logging;

namespace Mandara.Business.Bus.Commands
{
    public class RequestSnapshotDtoCommand<TMessage, TMessageDto> : BusCommandBase
        where TMessage : SnapshotMessageBase
        where TMessageDto : SnapshotMessageBase
    {
        private readonly TMessage _snapshotMessage;
        private readonly Action<TMessage> _onResponse;
        private readonly Action _snapshotResponseReceived;
        private readonly Action _onRequestFailed;
        private SnapshotReceiver<TMessageDto> _snapshotReceiver;

        private static readonly ILogger Logger = new NLogLoggerFactory().GetCurrentClassLogger();

        public static readonly Action DefaultResponseReceivedHandler = () =>
        {
            Logger.Debug("{0} - Received snapshot response", typeof(TMessage));
        };

        public static readonly Action DefaultRequestFailureHandler = () =>
        {
            Logger.Warn("{0} - Snapshot request failed", typeof(TMessage));
        };

        public RequestSnapshotDtoCommand(TMessage snapshotMessage, string topicName, Action<TMessage> onResponse) :
            this(snapshotMessage, topicName, onResponse, DefaultResponseReceivedHandler, DefaultRequestFailureHandler)
        {
        }

        public RequestSnapshotDtoCommand(
            TMessage snapshotMessage,
            string topicName,
            Action<TMessage> onResponse,
            Action onResponseReceived,
            Action onRequestFailed)
        {
            _snapshotMessage = snapshotMessage;
            _onResponse = onResponse;
            _snapshotResponseReceived = onResponseReceived;
            _onRequestFailed = onRequestFailed;
            TopicName = topicName;
        }

        public override void Execute()
        {
            LBMContext lbmContext = BusClient.Instance.InformaticaHelper.LbmContext;
            LBMSource lbmSource = BusClient.Instance.InformaticaHelper.GetSource(TopicName);

            if (lbmContext == null || lbmSource == null)
            {
                Logger.Warn("Informatica context or message source for '{0}' unavailable.  Cannot send.", TopicName);
                return;
            }

            _snapshotReceiver = new SnapshotReceiver<TMessageDto>(
                lbmContext,
                lbmSource,
                TopicName,
                ResponseTimeout);

            TMessageDto messageDto = Mapper.Map<TMessage, TMessageDto>(_snapshotMessage);

            _snapshotReceiver.GetSnapshot(
                messageDto,
                dtoMessage => _onResponse(Mapper.Map<TMessageDto, TMessage>(dtoMessage)),
                _snapshotResponseReceived,
                _onRequestFailed);
        }
    }
}