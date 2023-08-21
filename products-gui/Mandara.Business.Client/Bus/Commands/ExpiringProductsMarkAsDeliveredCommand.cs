namespace Mandara.Business.Bus.Commands
{
    using Mandara.Business.Bus.Messages;

    class ExpiringProductsMarkAsDeliveredCommand : SendMessageCommand<ExpiringProductsMarkAsDeliveredMessage>
    {
        public ExpiringProductsMarkAsDeliveredCommand(int securityDefinitionId)
            : base(InformaticaHelper.ExpiringProductsMarkAsDeliveredTopicName,
                    new ExpiringProductsMarkAsDeliveredMessage(securityDefinitionId)) {}
    }
}
