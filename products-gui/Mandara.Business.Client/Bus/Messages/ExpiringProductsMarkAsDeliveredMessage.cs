namespace Mandara.Business.Bus.Messages
{
    using Mandara.Business.Bus.Messages.Base;

    public class ExpiringProductsMarkAsDeliveredMessage : MessageBase
    {
        private readonly int _securityDefinitionId;

        public ExpiringProductsMarkAsDeliveredMessage(int securityDefinitionId)
        {
            _securityDefinitionId = securityDefinitionId;
        }

        public int SecurityDefinitionId
        {
            get { return _securityDefinitionId; }
        }
    }
}
