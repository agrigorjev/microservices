using System;
using AutoMapper;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.ErrorDetails;

namespace Mandara.Business.Bus.Commands
{
    public class SendRequestDtoCommand<TRequestMessage, TRequestMessageDto, TResponseMessage, TResponseMessageDto> : BusCommandBase
        where TRequestMessage : MessageBase
        where TRequestMessageDto : MessageBase
        where TResponseMessage : class
        where TResponseMessageDto : class
    {
        private readonly TRequestMessage _message;
        private readonly Action<TResponseMessage> _callback;
        private readonly Action<FailureCallbackInfo> _callbackOnFailure;

        private SendRequestDtoCommand(string topicName, TRequestMessage message, Action<TResponseMessage> callback)
        {
            _message = message;
            _callback = callback;

            TopicName = topicName;
        }

        public SendRequestDtoCommand(string topicName, TRequestMessage message, Action<TResponseMessage> callback, Action<FailureCallbackInfo> callbackOnFailure)
            : this(topicName, message, callback)
        {
            _callbackOnFailure = callbackOnFailure;
        }

        public SendRequestDtoCommand(string topicName, TRequestMessage message, int responseTimeout, Action<TResponseMessage> callback, Action<FailureCallbackInfo> callbackOnFailure)
            : this(topicName, message, callback, callbackOnFailure)
        {
            ResponseTimeout = responseTimeout;
        }

        public override void Execute()
        {
            if (_message != null)
            {
                TRequestMessageDto dto = Mapper.Map<TRequestMessage, TRequestMessageDto>(_message);

                SendRequest(dto, OnResponse, _callbackOnFailure);
            }
        }

        private void OnResponse(byte[] responseData)
        {
            TResponseMessageDto responseDto = JsonHelper.Deserialize<TResponseMessageDto>(responseData);

            TResponseMessage response = Mapper.Map<TResponseMessageDto, TResponseMessage>(responseDto);

            if (_callback != null)
                _callback(response);
        }
    }
}