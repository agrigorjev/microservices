using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages;
using Mandara.Entities.Enums;
using Mandara.Entities.ErrorReporting;
using System.Linq;

namespace Mandara.Business.Bus.Handlers
{
    public class ErrorsHandler : MessageHandler<ErrorMessage>
    {
        protected override void Handle(ErrorMessage message)
        {
            if (message.Errors == null)
                return;

            foreach (var error in message.Errors)
            {
                HandleError(error);
            }
        }

        private void HandleError(Error error)
        {
            if (BusClient.IsCurrentUserCanSeeErrorForUser(error.UserName) &&
                (error.PortfolioIds.Count == 0
                || error.PortfolioIds.Any(
                    portfolioId =>
                        BusClient.Instance.GetPrivileges().IsCurrentUserAuthorizedToPortfolio(
                            portfolioId,
                            PortfolioPermission.CanViewRisk))))
            {
                ErrorReportingHelper.GlobalQueue.Enqueue(error);
            }
        }
    }
}