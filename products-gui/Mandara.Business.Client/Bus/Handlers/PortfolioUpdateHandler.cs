using AutoMapper;
using com.latencybusters.lbm;
using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages.Portfolio;

namespace Mandara.Business.Bus.Handlers
{
    public class PortfolioUpdateHandler : IHandler
    {
        public void Handle(string topicName, LBMMessage lbmMessage, long receivedEpoch)
        {
            if (LBM.MSG_DATA != lbmMessage.type())
            {
                return;
            }

            PortfolioUpdateMessage message = GetPortfolioUpdate();
            message.ReceivedAt = receivedEpoch;
            if (!IsValidPortfolioCollection())
            {
                return;
            }

            BusClient.Instance.UpdatePortfolios(message.Portfolios);

            bool IsValidPortfolioCollection()
            {
                return null != message?.Portfolios;
            }

            PortfolioUpdateMessage GetPortfolioUpdate()
            {
                return Mapper.Map<PortfolioUpdateMessageDto, PortfolioUpdateMessage>(
                    JsonHelper.Deserialize<PortfolioUpdateMessageDto>(lbmMessage.data()));
            }
        }
    }
}