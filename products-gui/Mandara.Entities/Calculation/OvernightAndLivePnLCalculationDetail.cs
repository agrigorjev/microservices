using System;

namespace Mandara.Entities.Calculation
{
    [Serializable]
    public class OvernightAndLivePnLCalculationDetail : OvernightPnlCalculationDetail
    {
        public Money LiveAmount { get; set; }
        public Decimal GrossAmount
        {
            get
            {
                return LiveAmount.Amount + OvernightAmount.Amount;
            }
        }
        public Decimal LivePnL
        {
            get
            {
                return LiveAmount.Amount;
            }
        }
        public Decimal OvernightPnL
        {
            get
            {
                return OvernightAmount.Amount;
            }
        }

    }
}
