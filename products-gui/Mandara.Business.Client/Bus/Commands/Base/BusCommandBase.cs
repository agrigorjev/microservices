using System;
using System.Configuration;
using com.latencybusters.lbm;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Client.Managers;
using Mandara.Entities.ErrorDetails;
using Mandara.Extensions.Collections;
using Ninject.Extensions.Logging;

namespace Mandara.Business.Bus.Commands.Base
{
    public abstract class BusCommandBase : ICommand
    {
        public CommandManager CommandManager { get; set; }
        public string TopicName { get; set; }
        public int ResponseTimeout { get; set; }

        private Action<byte[]> _callback;
        private RequestWatchdog _requestWatchdog;
        private readonly ILogger _log = new NLogLogger(typeof(BusCommandBase));

        protected BusCommandBase()
        {
            int defaultTimeout;
            if (int.TryParse(ConfigurationManager.AppSettings["LbmRequestTimeout_Seconds"], out defaultTimeout))
            {
                defaultTimeout *= 1000;
            }
            else
            {
                defaultTimeout = 1500;
            }

            ResponseTimeout = defaultTimeout;
        }

        protected virtual void SendMessage(MessageBase message, bool useGzip = false)
        {
            LBMSource lbmSource;

            if (InformaticaHelper.Sources.TryGetValue(TopicName, out lbmSource))
            {
                SendMessage(message, lbmSource, TopicName, useGzip);
            }
        }

        protected void SendMessage(MessageBase message, LBMSource source, string topicName, bool useGzip)
        {
            try
            {
                InformaticaHelper.UpdateMessageUserNameAndIp(message);

                byte[] messageBytes = message.Serialize();
                if (useGzip)
                {
                    messageBytes = ZipManager.Zip(messageBytes);
                }

                InformaticaHelper.SendMessageToSource(source, topicName, messageBytes);
            }
            catch (AggregateException ex)
            {
                _log.Error(
                    "BusCommandBase::SendMessage: an aggregate exception sending the message to topic [{0}], " +
                    "below are [{1}] inner exceptions:",
                    topicName, ex.InnerExceptions.Count);

                ex.InnerExceptions.ForEach(
                    innerEx =>
                    {
                        _log.Error(innerEx, "AggregateException InnerException");
                    });
            }
            catch (Exception ex)
            {
                _log.Error(ex, "BusCommandBase::SendMessage: exception sending the message to topic [{0}]", topicName);
            }
        }

        protected void RespondToLbmMessage(InformaticaResponder lbmMessage, MessageBase message)
        {
            InformaticaHelper.RespondToMessage(lbmMessage, message);
        }

        protected void DisposeLbmMessage(LBMMessage lbmMessage)
        {
            InformaticaHelper.DisposeMessage(lbmMessage);
        }

        protected virtual void SendRequest(
            IMessage message,
            Action<byte[]> callback,
            Action<FailureCallbackInfo> callbackOnFailure = null)
        {
            if (InformaticaHelper.Sources.TryGetValue(TopicName, out LBMSource lbmSource))
            {
                SendRequest(message, lbmSource, TopicName, callback, callbackOnFailure);
            }
            else
            {
                callbackOnFailure?.Invoke(
                    new FailureCallbackInfo(
                        message.GetType().Name,
                        TopicName,
                        "Could not find Informatica source for topic"));
            }
        }

        private void SendRequest(
            IMessage message,
            LBMSource source,
            string topicName,
            Action<byte[]> callback,
            Action<FailureCallbackInfo> callbackOnFailure)
        {
            _callback = callback;

            Action<object, IMessage, LBMResponseCallback> request =
                (additionalCallbackArgs, requestMessage, responseCallback) =>
                    SendRequest(source, topicName, requestMessage, responseCallback, additionalCallbackArgs);

            Action localCallbackOnFailure = new Action(
                delegate ()
                {
                    callbackOnFailure?.Invoke(
                        new FailureCallbackInfo(message.GetType().Name, topicName, "Timeout"));
                });

            _requestWatchdog = new RequestWatchdog(
                message,
                OnResponse,
                request,
                null,
                ResponseTimeout,
                localCallbackOnFailure);

            _requestWatchdog.Start();
        }

        private static void SendRequest(
            LBMSource source,
            string topicName,
            IMessage message,
            LBMResponseCallback responseCallback,
            object additionalCallbackArgs)
        {
            InformaticaHelper.UpdateMessageUserNameAndIp(message);
            byte[] bytes = message.Serialize();
            InformaticaHelper.SendRequestToSource(source, topicName, bytes, responseCallback, additionalCallbackArgs);
        }

        private void OnResponse(object additionalCallbackArgs, byte[] data)
        {
            _callback(data);
        }

        public abstract void Execute();
    }
}