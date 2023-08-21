using Mandara.Date;
using System;

namespace Mandara.Entities.Services
{
    public class CalculationDetailIdentifierWithoutStripNameService : ICalculationDetailIdentifierService
    {
        public string GetKey(
            int productId,
            int sourceProductId,
            string stripName,
            DateTime calculationDate,
            int? portfolioId,
            DateTime? calendarDaySwapSettlementPriceDate)
        {
            return String.Format(
                "{0}_{1}_{2}_{3}_{4}",
                productId.ToString(),
                sourceProductId.ToString(),
                calculationDate.ToShortDateString(),
                portfolioId,
                calendarDaySwapSettlementPriceDate.ToShortDateString());
        }
    }
}