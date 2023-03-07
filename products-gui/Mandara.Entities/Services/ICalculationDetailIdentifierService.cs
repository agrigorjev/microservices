using System;

namespace Mandara.Entities.Services
{
    public interface ICalculationDetailIdentifierService
    {
        string GetKey(
            int productId,
            int sourceProductId,
            string stripName,
            DateTime calculationDate,
            int? portfolioId,
            DateTime? calendarDaySwapSettlementPriceDate);
    }
}