using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages;
using Mandara.Business.Bus.Messages.Spreader;

namespace Mandara.Business.Bus.Commands
{
    public class SpreaderCalculationCommand : BusCommandBase
    {
        private SpreaderRequestMessage _message;

        private Action<SpreaderResponseMessage> _callback;

        public SpreaderCalculationCommand(SpreaderRequestMessage message, Action<SpreaderResponseMessage> callback)
        {
            _message = message;
            _callback = callback;
            TopicName = InformaticaHelper.SpreaderCalculationTopicName;
        }

        public override void Execute()
        {
            SendRequest(_message, OnResponse);
        }

        private void OnResponse(byte[] responseData)
        {
            //TODO : add error handling, verion check
            SpreaderResponseMessage response = JsonHelper.Deserialize<SpreaderResponseMessage>(responseData);
            if (_callback != null)
                _callback(response);
        }
    }
}
