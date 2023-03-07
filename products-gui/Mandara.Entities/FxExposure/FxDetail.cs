using System;
using System.Globalization;

namespace Mandara.Entities.FxExposure
{
    public class FxDetail
    {
        public DateTime CalculationDate { get; private set; }
        public decimal Position { get; private set; }
        public decimal ForwardPosition { get; private set; }
        public decimal InitialExposure { get; private set; }
        public decimal RemainingInitialExposure { get; private set; }
        public decimal RealizedSpotPosition { get; private set; }
        public int PortfolioId { get; private set; }
        public int CurrencyId { get; private set; }

        private FxDetail()
        {
        }

        public FxDetail(
            DateTime calculationDate,
            decimal position,
            decimal forwardPosition,
            decimal initialExposure,
            decimal remainingInitialExposure,
            decimal realizedSpotPosition,
            int portfolioId,
            int currencyId)
        {
            if (portfolioId < 1)
            {
                throw new ArgumentException(
                    $"Portfolio Id should be grater than 0, provided [{portfolioId}]",
                    nameof(portfolioId));
            }

            if (currencyId < 1)
            {
                throw new ArgumentException(
                    $"Currency Id should be grater than 0, provided [{currencyId}]",
                    nameof(currencyId));
            }

            if (calculationDate == DateTime.MinValue)
            {
                throw new ArgumentException(
                    $"Calculation Date should be set to a valid datetime, provided " +
                    $"[{calculationDate.ToString(Mandara.Date.Formats.DashSeparatedShortDate, CultureInfo.InvariantCulture)}]",
                    nameof(calculationDate));
            }

            CalculationDate = calculationDate;
            Position = position;
            ForwardPosition = forwardPosition;
            PortfolioId = portfolioId;
            CurrencyId = currencyId;
            InitialExposure = initialExposure;
            RemainingInitialExposure = remainingInitialExposure;
            RealizedSpotPosition = realizedSpotPosition;
        }

        public void AddForwardPosition(decimal quantity)
        {
            ForwardPosition += quantity;
        }

        public void AddCashPositions(decimal quantity)
        {
            Position += quantity;
        }

        public void AddInitialExposure(decimal quantity)
        {
            InitialExposure += quantity;
        }

        public void AddRemainingInitialExposure(decimal quantity)
        {
            RemainingInitialExposure += quantity;
        }

        public void AddRealizedSpotPosition(decimal quantity)
        {
            RealizedSpotPosition += quantity;
        }

        public void UpdateCalculationDate(DateTime newDate)
        {
            CalculationDate = newDate;
        }
    }
}