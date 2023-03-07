using Mandara.Entities.FxExposure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Mandara.Business.Bus.Messages.Fx
{
    public class FxExposureMessage : FxSnapshotMessageBase
    {
        public int PortfolioId { get; private set; }
        public int CurrencyId { get; private set; }
        public DateTime RiskDate { get; private set; }

        private readonly List<FxDetail> _fxDetails;
        public IReadOnlyCollection<FxDetail> FxDetails
        {
            get
            {
                return _fxDetails;
            }
        }

        private FxExposureMessage()
        {
        }

        public FxExposureMessage(FxExposureMessage requestMessage)
            : this(
                requestMessage.PortfolioId,
                requestMessage.CurrencyId,
                requestMessage.RiskDate,
                requestMessage.FxDetails,
                requestMessage.ErrorMessage,
                requestMessage.Success)
        {
        }

        public FxExposureMessage(int portfolioId, int currencyId, DateTime riskDate)
            : this(portfolioId, currencyId, riskDate, new List<FxDetail>(), String.Empty, true)
        {
        }

        public FxExposureMessage(int portfolioId, int currencyId, DateTime riskDate, IEnumerable<FxDetail> fxDetails)
            : this(portfolioId, currencyId, riskDate, fxDetails, String.Empty, true)
        {
        }
        public FxExposureMessage(int portfolioId, int currencyId, DateTime riskDate, string errorMessage)
            : this(portfolioId, currencyId, riskDate, new List<FxDetail>(), errorMessage, false)
        {
        }


        [JsonConstructor]
        public FxExposureMessage(
            int portfolioId,
            int currencyId,
            DateTime riskDate,
            IEnumerable<FxDetail> fxDetails,
            string errorMessage,
            bool success)
        {
            PortfolioId = portfolioId;
            CurrencyId = currencyId;
            RiskDate = riskDate;

            IEnumerable<FxDetail> inputFxDetails = fxDetails ?? new List<FxDetail>();

            _fxDetails = new List<FxDetail>(inputFxDetails);

            if (!String.IsNullOrWhiteSpace(errorMessage))
            {
                SetError(errorMessage);
            }

            Success = success;
        }

        public override void OnErrorSet()
        {
            _fxDetails.Clear();
        }

        public void AddFxDetail(FxDetail fxDetail)
        {
            if (fxDetail == null)
            {
                throw new ArgumentNullException("fxDetail");
            }

            _fxDetails.Add(fxDetail);
        }
    }
}