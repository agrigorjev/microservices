using Mandara.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.Client.Managers
{
    public class PortfolioManager
    {
        public static string RootPortfolioName = "Top";

        public static bool IsRootPortfolio(Portfolio portfolio)
        {
            return portfolio == null || portfolio.PortfolioId == 0  || portfolio.Name == RootPortfolioName;
        }

        public static IEnumerable<int> GetHierarchyPortfolioIds(Portfolio portfolio, bool includeErrorBook = true)
        {
            if (includeErrorBook)
            {
                return portfolio.Portfolios.SelectMany(p => GetHierarchyPortfolioIds(p, includeErrorBook)).Concat(new[] { portfolio.PortfolioId });
            }
            else
            {
                return portfolio.Portfolios.Where(p => !p.IsErrorBook).SelectMany(p => GetHierarchyPortfolioIds(p, includeErrorBook)).Concat(new[] { portfolio.PortfolioId });
            }
        }

        public static bool IsLeafPortfolio(Portfolio portfolio)
        {
            return portfolio.Portfolios == null || portfolio.Portfolios.Count == 0;
        }

        public string GetFullPortfolioName(Portfolio portfolio)
        {
            if (portfolio == null)
                return String.Empty;

            Portfolio p = portfolio;
            List<string> path = new List<string>();

            do
            {
                path.Insert(0, p.Name);
                p = p.ParentPortfolio;
            } while (p != null);

            return string.Join(" / ", path);
        }
    }
}
