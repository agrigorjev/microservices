using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages
{
    public class PnlHistoricalInformationMessageArgs : MessageBase
    {
        public DateTime ReportTime { get; set; }

        public int ReportId { get; set; }

        public List<Entities.Portfolio> Portfolios { get; set; }

        public List<int> PortfolioIds { get; set; }
    }

    public class PnlHistoricalInformationMessage : MessageBase
    {
        public PnlHistoricalInformationMessage()
        {
            PnlReportRowCollection = new List<PnlReportRowData>();
        }

        public List<PnlReportRowData> PnlReportRowCollection { get; set; }

    }

    public static class PnlHistoricalInformationHelper
    {
        private static Dictionary<string, int> _currencies = new Dictionary<string, int>();

        public static List<PnlReportRow> PnlHistoricalInformationToReportRow(
            this PnlHistoricalInformationMessage report, List<Entities.Portfolio> portfolios)
        {
            Dictionary<int, PnlReportRow> res = new Dictionary<int, PnlReportRow>();

            if (report == null || report.PnlReportRowCollection.Count == 0)
                return new List<PnlReportRow>();

            foreach (PnlReportRowData row in report.PnlReportRowCollection)
            {
                Entities.Portfolio portfolio = portfolios != null
                    ? portfolios.FirstOrDefault(am => am.PortfolioId == row.PortfolioId)
                    : null;

                PnlReportRow item = new PnlReportRow(portfolio)
                {
                    PortfolioId = row.PortfolioId,
                    Currency = row.Currency,
                    CurrencyId = GetCurrencyHash(row.Currency)
                };

                item.SetLivePnlValue(row.LivePnl);
                item.SetOvernightPnlValue(row.OvernightPnl);
                item.LiveCostsValue = row.LiveCosts;
                item.BrokerageValue = new Money(0M, item.Currency);

                if (!res.ContainsKey(item.ID))
                {
                    res.Add(item.ID, item);
                }
            }

            return res.Values.ToList();
        }

        private static int GetCurrencyHash(string currency)
        {
            if (currency == null)
                return 0;

            int hash;
            if (!_currencies.TryGetValue(currency, out hash))
            {
                hash = _currencies.Count > 0 ? _currencies.Values.Max() + 1 : 1;
                _currencies.Add(currency, hash);
            }

            return hash;
        }
    }
}
