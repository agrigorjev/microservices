using System.Collections.Generic;
using System.Linq;
using Mandara.Business.Bus.Messages.Pnl;
using Mandara.Entities;
using Mandara.Entities.Enums;

namespace Mandara.Business.Client.Managers
{
    public static class PnlCumulativeManager
    {
     
        public static List<PnlReportRow> ConvertCumulativeToReportRow(this PnlCumulativeReportData report, List<Portfolio> portfolios)
        {
            var res = new List<PnlReportRow>();

            if (report == null || report.PnlCumulativeCollection.Count == 0)
                return res;

            foreach (var pnlCumulativeReportRow in report.PnlCumulativeCollection)
            {
                var portfolio = portfolios != null
                                    ? portfolios.FirstOrDefault(
                                        am => am.PortfolioId == pnlCumulativeReportRow.PortfolioId)
                                    : null;

                var item = new PnlReportRow(portfolio)
                {
                    PortfolioId = pnlCumulativeReportRow.PortfolioId
                };

                item.SetLivePnlValue(pnlCumulativeReportRow.LivePnl);
                item.SetOvernightPnlValue(pnlCumulativeReportRow.OvernightPnl);
                item.LiveCostsValue = pnlCumulativeReportRow.LiveCosts ?? new Money(0M, CurrencyCodes.USD);
                item.BrokerageValue = new Money(0M, CurrencyCodes.USD);
                res.Add(item);
            }

            return res;
        }
    }

}
