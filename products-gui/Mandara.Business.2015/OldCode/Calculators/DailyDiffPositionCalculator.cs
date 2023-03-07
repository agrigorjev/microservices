using Mandara.Entities;
using Mandara.Entities.Calculation;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Mandara.Business.OldCode.Calculators
{
    internal class DailyDiffPositionCalculator : PositionCalculatorBase
    {
        private readonly MandaraEntities _efContext;
        private readonly DailySwapPositionCalculator _dailySwapPositionCalculator;

        public DailyDiffPositionCalculator(CalculationManager calculationManager, MandaraEntities efContext, DailySwapPositionCalculator dailySwapPositionCalculator) : base(calculationManager)
        {
            _efContext = efContext;
            _dailySwapPositionCalculator = dailySwapPositionCalculator;
        }

        public override List<CalculationDetail> Calculate(
            SourceDetail sourceDetail,
            CalculationContext calculationContext)
        {
            Product product = calculationContext.Product;

            if (product.ComplexProduct == null)
            {
                LoadComplexProductAndLegProducts(product);
            }

            List<CalculationDetail> positions = new List<CalculationDetail>();
            CalculationContext context1 = calculationContext.GetNew(
                year: null,
                month: null,
                day: null,
                crudeSwapFactor: null,
                suppressRolloff: null,
                mergedHolidayDays: null,
                product: null,
                sourceProduct: null,
                quantity: null,
                suppressPositionConversionFactor: null,
                suppressContractSize: null,
                balmoCorrection: null,
                dailyDiffMonthShift: Product.NoDailyDiffMonthShift,
                balmoOnCrudeProduct: null,
                calcYear: null,
                calcMonth: null,
                calcDay: null);

            positions.AddRange(CalculateLeg(sourceDetail, context1, LegType.Leg1));

            int dailyDiffMonthShift = sourceDetail.Product.DailyDiffMonthShift;
            CalculationContext context2 = calculationContext.GetNew(
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                dailyDiffMonthShift,
                null,
                null,
                null,
                null);
            positions.AddRange(CalculateLeg(sourceDetail, context2, LegType.Leg2));

            return positions;
        }

        private void LoadComplexProductAndLegProducts(Product product)
        {
            DbEntityEntry<Product> productEntry = _efContext.Entry(product);

            if (_efContext.Entry(product).State == EntityState.Detached)
            {
                _efContext.Products.Attach(product);
            }

            productEntry.Reference(p => p.ComplexProduct).Load();

            DbEntityEntry<ComplexProduct> complexProdEntry = _efContext.Entry(product.ComplexProduct);

            complexProdEntry.Reference(p => p.ChildProduct1).Load();
            complexProdEntry.Reference(p => p.ChildProduct2).Load();

            DbEntityEntry<Product> leg1ProductEntry = _efContext.Entry(product.ComplexProduct.ChildProduct1);

            leg1ProductEntry.Reference(p => p.Category).Load();
            leg1ProductEntry.Reference(p => p.ExpiryCalendar).Load();
            leg1ProductEntry.Reference(p => p.HolidaysCalendar).Load();
            leg1ProductEntry.Reference(p => p.OfficialProduct).Load();

            DbEntityEntry<Product> leg2ProductEntry = _efContext.Entry(product.ComplexProduct.ChildProduct2);

            leg2ProductEntry.Reference(p => p.Category).Load();
            leg2ProductEntry.Reference(p => p.ExpiryCalendar).Load();
            leg2ProductEntry.Reference(p => p.HolidaysCalendar).Load();
            leg2ProductEntry.Reference(p => p.OfficialProduct).Load();
        }

        private List<CalculationDetail> CalculateLeg(SourceDetail sourceDetail, CalculationContext calculationContext, LegType legType)
        {
            Product product = calculationContext.Product;
            decimal qty = calculationContext.Quantity;
            Func<SourceDetail, CalculationContext, List<CalculationDetail>> calculator;

            if (legType == LegType.Leg2)
            {
                sourceDetail.ChangeToSecondLeg();
                sourceDetail.TradeCapture?.ChangeToSecondLeg();
                calculator = _dailySwapPositionCalculator.CalculateSecondLeg;
            }
            else
            {
                calculator = _dailySwapPositionCalculator.CalculateFirstLeg;
            }


            Product legProduct = legType == LegType.Leg1
                ? product.ComplexProduct.ChildProduct1
                : product.ComplexProduct.ChildProduct2;
            Decimal legSignFactor = legType == LegType.Leg1 ? 1M : -1M;
            Decimal legFactor = legType == LegType.Leg1
                ? (product.ComplexProduct.ConversionFactor1 ?? 1M)
                : (product.ComplexProduct.ConversionFactor2 ?? 1M);


            sourceDetail.SetOfficialProductProps(legProduct, legFactor);

            if (sourceDetail.TradeCapture != null)
            {
                sourceDetail.TradeCapture.SetOfficialProductProps(legProduct, legFactor);
            }


            qty = qty * legSignFactor * legFactor * product.ContractSize;

            CalculationContext context = calculationContext.GetNew(
                null,
                null,
                null,
                null,
                null,
                null,
                legProduct,
                product,
                qty,
                true,
                true,
                null,
                calculationContext.DailyDiffMonthShift,
                null,
                null,
                null,
                null);

            return calculator(sourceDetail, context);
        }
    }
}