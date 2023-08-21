using System;

namespace Mandara.Entities.Calculation
{
    [Serializable]
    public class DailyDetail
    {
        public DateTime CalculationDate { get; }
        public decimal Amount { get; }

        public DailyDetail(DateTime calculationDate, decimal amount)
        {
            CalculationDate = calculationDate;
            Amount = amount;
        }

        public override bool Equals(object obj)
        {
            return Object.ReferenceEquals(this, obj)
                   || (obj is DailyDetail rhs && CalculationDate.Equals(rhs.CalculationDate) && Amount == rhs.Amount);
        }

        public override int GetHashCode()
        {
            return (CalculationDate.GetHashCode()) ^ (Amount.GetHashCode() * 271);
        }
    }
}