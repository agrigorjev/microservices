using Mandara.Business.Model;
using Mandara.Entities;
using System.Collections.Generic;

namespace Mandara.Business.Services
{
    public interface ITradeSplitService
    {
        List<SdQuantityModel> SplitQCalTimeSpreadIntoPerMonth(List<SdQuantityModel> sdQuantities);
        List<TradeCapture> SplitQCalTimeSpreadIntoPerMonth(List<TradeCapture> sdQuantities);

        List<TradeModel> SplitHistoricCustomPeriodTrades(List<TradeModel> trades);
        List<TradeModel> SplitLiveCustomPeriodTrades(List<TradeModel> trades);
        List<TradeCapture> SplitCustomPeriodTrades(List<TradeCapture> trades);
        List<TradeCapture> SplitTradeCaptures(List<TradeCapture> liveTrades);
    }
}
