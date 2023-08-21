using Mandara.Business.Services.Prices;
using Mandara.Entities;
using Mandara.Entities.EntitiesCustomization;
using System;
using System.Collections.Generic;

namespace Mandara.Business.Contracts
{
    public interface ITradesBusinessService
    {
        void PausePositionsCalculation();
        void ContinuePositionsCalculation(int lastProcessedTradeId, int lastProcessedTradeChangeId);

        void ProcessStartupTrades(TradesQuantityData tradesQuantities);

        void CalculateLivePnl(
            TradeCapture trade,
            IPricesProvider pricesProvider,
            out Money? livePnl,
            out Money? livePrice);

        void CalculatePastLivePnl(
            TradeCapture trade,
            DateTime riskDate,
            IPricesProvider pricesProvider,
            out Money? livePnl);

        List<TradeCapture> AddOrUpdateLiveTrade(TradeCapture trade, bool returnAllAffectedTrades);

        List<string> HandleAddedTrade(TradeCapture trade, bool ignoreLastProcessedTradeCheck = false);
        List<string> HandleCancelledTrade(int changeId, TradeCapture trade, out bool rescheduleChange);

        List<string> HandleAssignedTrade(
            int changeId,
            TradeCapture trade,
            int? fromPortfolioId,
            out bool rescheduleChange);

        List<string> HandleReplaceTrade(
            int changeId,
            TradeCapture trade,
            decimal oldQuantity,
            out bool rescheduleChange);

        void SendPositionsUpdateCommand(List<string> updatedKeys);
        void SetTradeMasterProperties(TradeCapture trade);

        void SendTradesUpdateCommand(List<TradeCapture> updatedTrades);
        void SetMinimumTradeIdForProcessing(int tradeId);
        void AddProcessedTradeId(int tradeId);

        List<string> HandleAddedFxExposure(TradeCapture trade, bool ignoreLastProcessedTradeCheck = false);
        List<string> HandleCancelledFxExposure(int changeId, TradeCapture trade);
        List<string> HandleAssignedFxExposure(int changeId, TradeCapture trade, int? fromPortfolioId);
        List<string> HandleReplaceFxExposure(int changeId, TradeCapture trade, decimal oldQuantity);
        void SendFxExposureUpdateCommand(List<string> fxKeys);
        TradeCapture GetLegTrade(TradeCapture parentTrade, StripPart stripPart);
        void SetLastProcessedTradeChangeId(int tradeChangeId);
    }
}