using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.EditTrades
{
    public class TradesEditResponseMessage : MessageWithErrorBase
    {
        public string Response { get; set; }

        public override void OnErrorSet()
        {
        }

        public override string ToString()
        {
            return $"Success:({Success}) Response:({Response}) Error:({ErrorMessage}) by ({UserName}@{UserIp})";
        }
    }
}