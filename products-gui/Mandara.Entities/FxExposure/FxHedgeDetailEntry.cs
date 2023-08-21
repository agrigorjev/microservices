using System;

namespace Mandara.Entities.FxExposure
{
    public class FxHedgeDetailEntry
    {
        public DateTime ExpiryDate { get; private set; }
        public decimal RollOffExposure { get; private set; }
        public decimal SpotExecuted { get; private set; }
        public int PortfolioId { get; private set; }

        public FxHedgeDetailEntry(DateTime expiryDate, decimal rolloffExposure, decimal spotExecuted, int portfolioId)
        {
            ValidateFxHedgeData(
                IsValidExpiryDate,
                expiryDate,
                "Expiry date {0} cannot be the minimum or maximum date type value");
            ValidateFxHedgeData(
                IsValidDecimalAmountValue,
                rolloffExposure,
                "The roll-off exposure {0} must be between the minimum and maximum decimal values.");
            ValidateFxHedgeData(
                IsValidDecimalAmountValue,
                spotExecuted,
                "The spot {0} must be between the minimum and maximum decimal values.");
            ValidateFxHedgeData(
                IsValidPortfolioId,
                portfolioId,
                "The portfolio ID {0} must be 1 or higher to be a valid portfolio ID.");

            ExpiryDate = expiryDate;
            RollOffExposure = rolloffExposure;
            SpotExecuted = spotExecuted;
            PortfolioId = portfolioId;
        }

        private void ValidateFxHedgeData<T>(Predicate<T> validator, T valueToTest, string errorMessageFormat)
        {
            if (!validator(valueToTest))
            {
                throw new ArgumentException(String.Format(errorMessageFormat, valueToTest));
            }
        }

        private bool IsValidExpiryDate(DateTime expiryDate)
        {
            return expiryDate != DateTime.MinValue && expiryDate != DateTime.MaxValue;
        }

        private bool IsValidDecimalAmountValue(decimal amount)
        {
            return amount != Decimal.MinValue && amount != Decimal.MaxValue;
        }

        private bool IsValidPortfolioId(int portfolioId)
        {
            return portfolioId >= 1 && portfolioId != Int32.MaxValue;
        }

        public override string ToString()
        {
            return String.Format(
                "{0} || {1} || {2} || {3} || {4} (calculated)",
                ExpiryDate,
                PortfolioId,
                RollOffExposure,
                SpotExecuted,
                RollOffExposure - SpotExecuted);
        }
    }
}
