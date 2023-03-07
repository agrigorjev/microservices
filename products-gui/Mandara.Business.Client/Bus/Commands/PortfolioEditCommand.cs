using System;
using AutoMapper;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages;
using Mandara.Business.Bus.Messages.Portfolio;
using Mandara.Entities.ErrorDetails;

namespace Mandara.Business.Bus.Commands
{
    public class PortfolioEditCommand : BusCommandBase
    {
        private readonly PortfolioEditMessageDto _message;
        private readonly Action<PortfolioEditMessage> _callback;
        private readonly Action<FailureCallbackInfo> _callbackOnFailture;

        public PortfolioEditCommand(PortfolioEditMessageDto message, Action<PortfolioEditMessage> callback, Action<FailureCallbackInfo> callbackOnFailture)
        {
            _message = message;
            _callback = callback;
            _callbackOnFailture = callbackOnFailture;

            TopicName = InformaticaHelper.PortfolioEditTopicName;
        }


        public override void Execute()
        {
            if (_message != null)
            {
                SendRequest(_message, OnResponse, _callbackOnFailture);
            }
        }

        private void OnResponse(byte[] responseData)
        {
            //TODO : add error handling, verion check
            PortfolioEditMessageDto dto = JsonHelper.Deserialize<PortfolioEditMessageDto>(responseData);

            PortfolioEditMessage message = Mapper.Map<PortfolioEditMessageDto, PortfolioEditMessage>(dto);

            if (_callback != null)
                _callback(message);
        }
    }
}