using System;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.ErrorDetails;
using Mandara.Extensions.Nullable;

namespace Mandara.Business.Bus.Commands
{
    public class SendRequestCommand<TRequest, TResponse> : BusCommandBase
        where TRequest : MessageBase
        where TResponse : MessageBase
    {
        private readonly Action<FailureCallbackInfo> _callbackOnFailure;
        private readonly TRequest _message;
        private readonly Action<TResponse> _callback;

        private SendRequestCommand(string topicName, TRequest message, Action<TResponse> callback)
        {
            _message = message;
            _callback = callback;

            TopicName = topicName;
        }

        public SendRequestCommand(string topicName, TRequest message, Action<TResponse> callback, Action<FailureCallbackInfo> callbackOnFailure)
            : this(topicName, message, callback)
        {
            _callbackOnFailure = callbackOnFailure;
        }

        public SendRequestCommand(string topicName, TRequest message, Action<TResponse> callback, int responseTimeout, Action<FailureCallbackInfo> callbackOnFailure)
            : this(topicName, message, callback)
        {
            _callbackOnFailure = callbackOnFailure;
            ResponseTimeout = responseTimeout;
        }

        public override void Execute()
        {
            if (_message != null)
                SendRequest(_message, OnResponse, _callbackOnFailure);
        }

        private void OnResponse(byte[] responseData)
        {
            TResponse response = JsonHelper.Deserialize<TResponse>(responseData);
            MessageWithErrorBase rspWithError = response as MessageWithErrorBase;

            if (null != rspWithError && (rspWithError?.Success).False() && null != _callbackOnFailure)
            {
                _callbackOnFailure.Invoke(
                    new FailureCallbackInfo() { MessageTypeName = rspWithError.ErrorMessage, TopicName = TopicName });
                return;
            }

            _callback?.Invoke(response);
        }
    }
}