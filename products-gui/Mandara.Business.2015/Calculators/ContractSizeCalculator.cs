using Mandara.Entities.Enums;
using System;

namespace Mandara.Business.Calculators
{
    public class ContractSizeCalculator
    {
        public static decimal ApplyContractSizeMultiplier(
            decimal amount,
            ContractSizeMultiplier multiplierSelection,
            DateTime calculationDateTime)
        {
            return
                ApplyContractSizeMultiplier(
                    amount,
                    multiplierSelection,
                    calculationDateTime.Year,
                    calculationDateTime.Month);
        }

        public static decimal ApplyContractSizeMultiplier(
            decimal amount,
            ContractSizeMultiplier multiplierSelection,
            int productYear,
            int productMonth)
        {
            decimal adjustedAmount = amount;

            switch (multiplierSelection)
            {
                case ContractSizeMultiplier.Daily:
                    {
                        int numberOfDays = DateTime.DaysInMonth(productYear, productMonth);

                        adjustedAmount = amount * numberOfDays;
                    }
                    break;

                case ContractSizeMultiplier.Hourly:
                    {
                        decimal numberOfHours = HoursInMonthCalculator.HoursInMonth(productYear, productMonth);

                        adjustedAmount = amount * numberOfHours;
                    }
                    break;
            }

            return adjustedAmount;
        }
    }
}
