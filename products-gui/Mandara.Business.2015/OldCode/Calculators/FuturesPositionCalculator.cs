using Mandara.Entities;
using Mandara.Entities.Calculation;
using System;
using System.Collections.Generic;

namespace Mandara.Business.OldCode.Calculators
{
    internal class FuturesPositionCalculator : PositionCalculatorBase
    {
        private readonly SwapPositionCalculator _swapPositionCalculator;

        public FuturesPositionCalculator(
            CalculationManager calculationManager,
            SwapPositionCalculator swapPositionCalculator)
            : base(calculationManager)
        {
            _swapPositionCalculator = swapPositionCalculator;
        }

        public override List<CalculationDetail> Calculate(
            SourceDetail sourceDetail,
            CalculationContext calculationContext)
        {
            calculationContext.SuppressRolloff = true;

            DateTime riskDate = calculationContext.RiskDate;
            int productYear = calculationContext.ProductYear;
            int productMonth = calculationContext.ProductMonth;
            Product product = calculationContext.Product;
            CalculationCache cache = calculationContext.CalculationCache;
            Product sourceProduct = calculationContext.SourceProduct;
            ProductDateType productDateType = calculationContext.ProductDateType;

            var productDate = new DateTime(productYear, productMonth, 1);

            DateTime expiryDate = CalculationManager.GetExpiryDate(
                cache,
                sourceDetail,
                product,
                productYear,
                productMonth);

            // save expiry date for another operations
            if (sourceDetail.TradeCapture != null && !sourceDetail.TradeCapture.ExpiryDate.HasValue)
                sourceDetail.TradeCapture.ExpiryDate = expiryDate;

            // if expiryDate is less than riskDate then contract has expired
            if (product.LocalRolloffTime.HasValue && product.LocalRolloffTime.Value.TimeOfDay > TimeSpan.Zero)
            {
                DateTime expiryDateWithTime = expiryDate.Date.Add(product.LocalRolloffTime.Value.TimeOfDay);

                if (expiryDateWithTime < riskDate) // time sensitive comparison
                {
                    if (expiryDateWithTime.Year == riskDate.Year && expiryDateWithTime.Month == riskDate.Month)
                    {
                        CalculationDetail calculation = CalculationDetail.Create(
                            sourceDetail,
                            product,
                            sourceProduct,
                            productYear,
                            productMonth,
                            0M,
                            productDate,
                            productDateType,
                            sourceDetail.StripName,
                            null,
                            null,
                            Product.NoDailyDiffMonthShift);

                        return new List<CalculationDetail> { calculation };
                    }
                    else
                    {
                        return new List<CalculationDetail>();
                    }
                }
            }
            else
            {
                if (expiryDate.Date < riskDate.Date) // day sensitive (time independent)
                {
                    if (expiryDate.Year == riskDate.Year && expiryDate.Month == riskDate.Month)
                    {
                        CalculationDetail calculation = CalculationDetail.Create(
                            sourceDetail,
                            product,
                            sourceProduct,
                            productYear,
                            productMonth,
                            0M,
                            productDate,
                            productDateType,
                            sourceDetail.StripName,
                            null,
                            null,
                            Product.NoDailyDiffMonthShift);

                        return new List<CalculationDetail> { calculation };
                    }
                    else
                    {
                        return new List<CalculationDetail>();
                    }
                }
            }

            return _swapPositionCalculator.Calculate(sourceDetail, calculationContext);
        }
    }
}