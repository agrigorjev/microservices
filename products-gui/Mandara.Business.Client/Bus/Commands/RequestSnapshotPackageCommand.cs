using com.latencybusters.lbm;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Bus.SnapshotReceivers;
using System;
using Ninject.Extensions.Logging;

namespace Mandara.Business.Bus.Commands
{
    using System.Collections.Generic;

    public class RequestSnapshotPackageCommand<T> : BusCommandBase
        where T : SnapshotMessageBase
    {
        private readonly T _snapshotMessage;
        private readonly Action<List<T>> _successCallback;
        private SnapshotPackageReceiver<T> _snapshotReceiver;
        private readonly InformaticaHelper _informaticaHelper;
        private readonly Action _snapshotReceivedCallback;
        private readonly Action _snapshotFailureCallback;

        private static readonly ILogger Logger = new NLogLoggerFactory().GetCurrentClassLogger();

        public static readonly Action DefaultResponseReceivedHandler = () =>
        {
            Logger.Debug("{0} - Received snapshot response", typeof(T));
        };

        public static readonly Action DefaultRequestFailureHandler = () =>
        {
            Logger.Debug("{0} - Snapshot request failed", typeof(T));
        };

        public RequestSnapshotPackageCommand(T snapshotMessage, string topicName, Action<List<T>> successCallback) :
            this(
                snapshotMessage,
                topicName,
                successCallback,
                DefaultResponseReceivedHandler,
                DefaultRequestFailureHandler,
                null)
        {
        }

        public RequestSnapshotPackageCommand(
            T snapshotMessage,
            string topicName,
            Action<List<T>> successCallback,
            Action snapshotReceivedCallback) : this(
            snapshotMessage,
            topicName,
            successCallback,
            snapshotReceivedCallback,
            DefaultRequestFailureHandler,
            null)
        {
        }

        public RequestSnapshotPackageCommand(
            T snapshotMessage,
            string topicName,
            Action<List<T>> successCallback,
            Action snapshotReceivedCallback,
            Action snapshotFailureCallback)
            : this(snapshotMessage, topicName, successCallback, snapshotReceivedCallback, snapshotFailureCallback, null)
        {
        }

        public RequestSnapshotPackageCommand(
            T snapshotMessage,
            string topicName,
            Action<List<T>> successCallback,
            Action snapshotReceivedCallback,
            Action snapshotFailureCallback,
            InformaticaHelper informaticaHelper)
        {
            _snapshotMessage = snapshotMessage;
            _successCallback = successCallback;
            _informaticaHelper = informaticaHelper;
            _snapshotReceivedCallback = snapshotReceivedCallback;
            _snapshotFailureCallback = snapshotFailureCallback;
            TopicName = topicName;
        }

        public override void Execute()
        {
            (LBMContext lbmContext, LBMSource lbmSource) = GetInformaticaSendContext();

            if (lbmContext == null || lbmSource == null)
            {
                return;
            }

            _snapshotReceiver = new SnapshotPackageReceiver<T>(lbmContext, lbmSource, TopicName, ResponseTimeout);
            _snapshotReceiver.GetSnapshot(
                _snapshotMessage,
                _successCallback,
                _snapshotReceivedCallback,
                _snapshotFailureCallback);
        }

        private (LBMContext, LBMSource) GetInformaticaSendContext()
        {
            return null != _informaticaHelper
                ? (_informaticaHelper.LbmContext, _informaticaHelper.GetSource(TopicName))
                : (BusClient.Instance.InformaticaHelper.LbmContext,
                    BusClient.Instance.InformaticaHelper.GetSource(TopicName));
        }
    }
}