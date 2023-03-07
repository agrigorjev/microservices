using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mandara.Entities.Calculation
{
    [Serializable]
    public class PnlCalculationDetail
    {
        public DateTime CalculationDate { get; set; }
        public ProductDateType DateType { get; set; }
        
        public String ProductCategory { get; set; }
        public String Product { get; set; }
        public Decimal Amount { get; set; }

        public Int32 ProductId { get; set; }

        public string TimeSpreadStrip { get; set; }

        public String PeriodName
        {
            get
            {
                if (!string.IsNullOrEmpty(TimeSpreadStrip))
                {
                    return TimeSpreadStrip;
                }

                switch (DateType)
                {
                    case ProductDateType.Day:
                    case ProductDateType.Daily:
                    case ProductDateType.MonthYear:
                    {
                        return CalculationDate.ToString("MMMM");
                    }

                    case ProductDateType.Quarter:
                    {
                        return $"Q{CalculationDate.Month / 3 + 1}";
                    }

                    case ProductDateType.Year:
                    {
                        return $"CAL{CalculationDate.Year}";
                    }

                    case ProductDateType.Custom:
                    {
                        return "Custom";
                    }

                    default:
                    {
                        return "Unknown";
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"{nameof(ProductCategory)}: {ProductCategory}, {nameof(Product)}: {Product}, {nameof(Amount)}: {Amount}, {nameof(ProductId)}: {ProductId}, {nameof(TimeSpreadStrip)}: {TimeSpreadStrip}, {nameof(PeriodName)}: {PeriodName}";
        }
    }
}
