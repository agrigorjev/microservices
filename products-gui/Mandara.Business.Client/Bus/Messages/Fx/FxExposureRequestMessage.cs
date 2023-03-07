using System;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Fx
{
    public class FxExposureRequestMessage : SnapshotMessageBase
    { 
        public int PortfolioId { get; private set; }
        public int CurrencyId { get; private set; }
        public DateTime RiskDate { get; private set; }

        private FxExposureRequestMessage()
        {
        }

        public FxExposureRequestMessage(int portfolioId, int currencyId, DateTime riskDate)
        {
            if (portfolioId < 1)
            {
                throw new ArgumentException(
                    string.Format("Portfolio Id should be grater than 0, provided [{0}]", portfolioId),
                    "portfolioId");
            }

            if (currencyId < 1)
            {
                throw new ArgumentException(
                    string.Format("Currency Id should be grater than 0, provided [{0}]", currencyId),
                    "currencyId");
            }

            if (riskDate == DateTime.MinValue)
            {
                throw new ArgumentException(
                    string.Format(
                        "Risk Date should be set to a valid datetime, provided [{0}]",
                        riskDate.ToShortDateString()),
                    "riskDate");
            }

            PortfolioId = portfolioId;
            CurrencyId = currencyId;
            RiskDate = riskDate;
        }
    }
}