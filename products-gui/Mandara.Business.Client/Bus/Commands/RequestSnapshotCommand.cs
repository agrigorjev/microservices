using com.latencybusters.lbm;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Bus.SnapshotReceivers;
using System;
using Ninject.Extensions.Logging;

namespace Mandara.Business.Bus.Commands
{
    public class RequestSnapshotCommand<T> : BusCommandBase
        where T : SnapshotMessageBase
    {
        private readonly T _snapshotMessage;
        private readonly Action<T> _successHandler;
        private readonly Action _onResponseReceived;
        private readonly Action _onRequestFailed;
        private SnapshotReceiver<T> _snapshotReceiver;

        private static readonly ILogger Logger = new NLogLoggerFactory().GetCurrentClassLogger();

        public static readonly Action DefaultResponseReceivedHandler = () =>
        {
            Logger.Debug("{0} - Received snapshot response", typeof(T));
        };

        public static readonly Action DefaultRequestFailureHandler = () =>
        {
            Logger.Warn("{0} - Snapshot request failed", typeof(T));
        };

        public RequestSnapshotCommand(T snapshotMessage, string topicName, Action<T> successHandler) : this(
            snapshotMessage,
            topicName,
            successHandler,
            DefaultResponseReceivedHandler,
            DefaultRequestFailureHandler)
        {
        }

        public RequestSnapshotCommand(
            T snapshotMessage,
            string topicName,
            Action<T> successHandler,
            Action onResponseReceived,
            Action onRequestFailed)
        {
            _snapshotMessage = snapshotMessage;
            _successHandler = successHandler;
            _onResponseReceived = onResponseReceived;
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

            _snapshotReceiver = new SnapshotReceiver<T>(lbmContext, lbmSource, TopicName, ResponseTimeout);
            _snapshotReceiver.GetSnapshot(_snapshotMessage, _successHandler, _onResponseReceived, _onRequestFailed);
        }
    }
}