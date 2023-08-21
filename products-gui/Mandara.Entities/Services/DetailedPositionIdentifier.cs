using Mandara.Date;
using System;

namespace Mandara.Entities.Services
{
    public class DetailedPositionIdentifier : ICalculationDetailIdentifierService
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
                "{0}_{1}_{2}_{3}_{4}_{5}",
                productId,
                sourceProductId,
                (stripName ?? String.Empty),
                calculationDate.ToShortDateString(),
                portfolioId,
                calendarDaySwapSettlementPriceDate.ToShortDateString());
        }
    }
}
