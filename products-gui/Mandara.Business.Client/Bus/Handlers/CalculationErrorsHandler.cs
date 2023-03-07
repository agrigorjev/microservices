using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages;
using Mandara.Entities.ErrorReporting;
using com.latencybusters.lbm;

namespace Mandara.Business.Bus.Handlers
{
    using System.Linq;

    public class CalculationErrorsHandler : MessageHandler<CalculationErrorsMessage>
    {
        protected override void Handle(CalculationErrorsMessage message)
        {
            if (message.CalculationErrors == null)
                return;

            ErrorReportingHelper.EnqueueCalculationErrors(
                message.CalculationErrors
                    .Where(e => BusClient.IsCurrentUserCanSeeErrorForUser(e.UserName))
                    .ToList(),
                "Server Positions");
        }

        public void Handle(string topicName, LBMMessage lbmMessage)
        {
            CalculationErrorsMessage message = null;

            switch (lbmMessage.type())
            {
                case LBM.MSG_DATA:
                    message = JsonHelper.Deserialize<CalculationErrorsMessage>(lbmMessage.data());
                    break;

                case LBM.MSG_BOS:
                    break;

                case LBM.MSG_EOS:
                    break;

                default:
                    break;
            }

            lbmMessage.dispose();

            if (message == null || message.CalculationErrors == null)
                return;

        }
    }
}