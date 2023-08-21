using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.FxExposure;

namespace Mandara.Business.Bus.Messages.Fx
{
    public class FxExposureResponseMessage : SnapshotMessageBase
    {
        public int PortfolioId { get; }
        public int CurrencyId { get; }
        public DateTime RiskDate { get; }

        private readonly List<FxDetail> _fxDetails;
        public IReadOnlyCollection<FxDetail> FxDetails => _fxDetails;

        private FxExposureResponseMessage()
        {
        }

        public FxExposureResponseMessage(FxExposureRequestMessage requestMessage)
        {
            PortfolioId = requestMessage.PortfolioId;
            CurrencyId = requestMessage.CurrencyId;
            RiskDate = requestMessage.RiskDate;

            _fxDetails = new List<FxDetail>();
            Success = true;
        }

        public override void SetError(string errorMessage)
        {
            HandleInvalidErrorMessage(errorMessage);
            _fxDetails.Clear();
            ErrorMessage = errorMessage;
            Success = false;
        }

        public void AddFxDetail(FxDetail fxDetail)
        {
            if (fxDetail == null)
            {
                throw new ArgumentNullException(nameof(fxDetail));
            }

            _fxDetails.Add(fxDetail);
        }
    }
}