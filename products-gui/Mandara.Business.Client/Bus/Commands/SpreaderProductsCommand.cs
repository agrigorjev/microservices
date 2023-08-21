using System;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages.Spreader;
using Mandara.Entities.ErrorDetails;

namespace Mandara.Business.Bus.Commands
{
    public class SpreaderProductsCommand : BusCommandBase
    {
        private SpreaderProductsMessage _message;

        private readonly Action<SpreaderProductsResponseMessage> _onRequestComplete;
        private readonly Action<FailureCallbackInfo> _onRequestFail;

        public SpreaderProductsCommand(
            SpreaderProductsMessage message,
            Action<SpreaderProductsResponseMessage> onRequestComplete,
            Action<FailureCallbackInfo> onRequestFail)
        {
            _message = message;
            _onRequestComplete = onRequestComplete;
            _onRequestFail = onRequestFail;
            TopicName = InformaticaHelper.SpreaderProductsTopicName;
        }

        public override void Execute()
        {
            SendRequest(_message, OnResponse, _onRequestFail);
        }

        private void OnResponse(byte[] responseData)
        {
            //TODO : add error handling, verion check
            SpreaderProductsResponseMessage response =
                JsonHelper.Deserialize<SpreaderProductsResponseMessage>(responseData);

            _onRequestComplete?.Invoke(response);
        }
    }
}
