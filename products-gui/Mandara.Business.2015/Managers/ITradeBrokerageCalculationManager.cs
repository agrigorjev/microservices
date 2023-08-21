using Mandara.Entities;
using Mandara.Entities.EntityPieces;
using System;
using System.Collections.Generic;

namespace Mandara.Business
{
    public interface ITradeBrokerageCalculationManager
    {
        decimal CalculateTradeBrokerage(
            string executingFirm,
            int? tradeType,
            int? numOfLots,
            decimal? quantity,
            string stripName,
            DateTime? tradeStartDate,
            DateTime? tradeEndDate,
            int productId,
            Unit unit);

        decimal CalculateTradeBrokerage(TradeCapture trade);
        decimal CalculateTradeBrokerage(TradePieces trade);
        void CalculateTradesBrokerages(List<TradeCapture> trades);
    }
}