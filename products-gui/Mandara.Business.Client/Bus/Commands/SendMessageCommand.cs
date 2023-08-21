namespace Mandara.Business.Bus.Commands
{
    using Mandara.Business.Bus.Commands.Base;
    using Mandara.Business.Bus.Messages.Base;

    public class SendMessageCommand<T> : BusCommandBase where T : MessageBase
    {
        private readonly T _message;

        public SendMessageCommand(string topicName, T message)
        {
            _message = message;
            TopicName = topicName;
        }

        public override void Execute()
        {
            SendMessage(_message, _message.UseGzip);
        }
    }
}
