using Mandara.TradeApiService.Data;

namespace Mandara.TradeApiService.Repositories.Contracts;

public interface ITradesRepository
{
    //(List<Company>, List<Exchange>, List<TradeTemplate>, List<Unit>) ReadInitialDataBatch(
    //    );

    //Portfolio GetUserPortfolio(int userId);

    //int GetNextSecDefPrimaryKey();

    //List<Company> GetBrokers(DateTime? startDate, DateTime? endDate);

    List<TradeCapture> GetTradesWithSameGroup(int groupId, string ordStatus = null);

    List<TradeCapture> GetFullSpreadTrades(TradeCapture spreadTrade);

    //List<TradeCapture> GetTradesByTradeIds(List<int> tradeIds);

    //List<TradeCapture> GetTradesByGroupIds(List<int> groupIds);

    //List<TradeCapture> GetTimeSpreadComponents(int spreadGroupTradeId);
}