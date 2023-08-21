using Mandara.TradeApiService.Configuration;
using Mandara.TradeApiService.Data;
using Mandara.TradeApiService.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;

namespace Mandara.TradeApiService.Repositories
{
    public class TradesRepository : ITradesRepository
    {
        private readonly DataStoragesSettings _serviceSettings;
        private readonly IDbContextFactory<MandaraEntities> _contextFactory;

        public TradesRepository(IOptions<DataStoragesSettings> serviceSettings, IDbContextFactory<MandaraEntities> contextFactory)
        {
            _serviceSettings = serviceSettings.Value;
            _contextFactory = contextFactory;
        }


        public List<TradeCapture> GetTradesWithSameGroup(int groupId, string ordStatus = null)
        {
            List<TradeCapture> groupTrades;
            using (MandaraEntities dbContext = _contextFactory.CreateDbContext())
            {
                IQueryable<TradeCapture> query =
                    dbContext.TradeCaptures.Include(x => x.Portfolio)
                       .Include(x => x.SellBook)
                       .Include(x => x.BuyBook)
                       .Include(x => x.TradeGroup)
                       .Include(x => x.SecurityDefinition)
                       .Include(x => x.SecurityDefinition.Product)
                       .Where(x => x.GroupId == groupId);

                if (ordStatus != null)
                {
                    query = query.Where(x => x.OrdStatus == ordStatus);
                }

                groupTrades = query.ToList();
            }

            return groupTrades;
        }

        public List<TradeCapture> GetFullSpreadTrades(TradeCapture spreadTrade)
        {
            List<TradeCapture> spreadGroup;

            using (MandaraEntities dbContext = _contextFactory.CreateDbContext())
            {
                spreadGroup =
                    dbContext.TradeCaptures.Include(x => x.Portfolio)
                       .Include(x => x.SellBook)
                       .Include(x => x.BuyBook)
                       .Include(x => x.TradeGroup)
                       .Include(x => x.SecurityDefinition)
                       .Include(x => x.SecurityDefinition.Product)
                       .Where(
                           x =>
                               (x.ExecID == spreadTrade.ExecID) && (x.TradeDate == spreadTrade.TradeDate)
                               && ((x.ClOrdID == spreadTrade.ClOrdID) || (x.ClOrdID == null)))
                       .ToList();
            }

            return spreadGroup;
        }
    }
}
