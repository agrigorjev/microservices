using System.Collections.Generic;
using System.Linq;
using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages.Pnl;
using Mandara.Entities.Calculation;

namespace Mandara.Business.Bus.Handlers
{
    public class PnlUpdateHandler : MessageHandler<PnlUpdateMessage>
    {
        protected override void Handle(PnlUpdateMessage message)
        {
            if (message.SnapshotDatetime.HasValue)
            {
                BusClient.Instance.OnPnlSnapshotDatetime = message.SnapshotDatetime.Value.ToLocalTime();
            }

            if (message.PnlByCurrencyByPortfolio != null)
            {
                BusClient.Instance.PnlDictionary = GetCurrencyPnLs(message);
            }

            BusClient.Instance.OvernightAbnPnl = message.OvernightAbnPnl;
        }

        private static Dictionary<int, Dictionary<string, PnlData>> GetCurrencyPnLs(PnlUpdateMessage message)
        {
            return message.PnlByCurrencyByPortfolio.Aggregate(
                new Dictionary<int, Dictionary<string, PnlData>>(),
                (currencyPnLPerPortfolio, nextCurrencyPnL) =>
                {
                    currencyPnLPerPortfolio.Add(
                        nextCurrencyPnL.Key,
                        nextCurrencyPnL.Value.ToDictionary(currPnL => currPnL.Key, currPnL => currPnL.Value));
                    return currencyPnLPerPortfolio;
                });
        }
    }
}