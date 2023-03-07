using System.Collections.Generic;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages;
using Mandara.Entities.ErrorReporting;
using System.Linq;

namespace Mandara.Business.Bus.Commands
{
    public class SendErrorCommand : BusCommandBase
    {
        private readonly List<string> _topicNames;
        private readonly List<Error> _errors;

        public SendErrorCommand(Error error)
            : this(new[] { error }.ToList()) {}

        public SendErrorCommand(List<Error> error, List<string> topicNames = null)
            : this(topicNames)
        {
            _errors = error;
        }

        private SendErrorCommand(List<string> topicNames)
        {
            _topicNames = topicNames ?? new List<string> { InformaticaHelper.ErrorsTopicName };
        }

        public override void Execute()
        {
            foreach (var topicName in _topicNames)
            {
                TopicName = topicName;
                SendMessage(new ErrorMessage {Errors = _errors});
            }
        }
    }
}