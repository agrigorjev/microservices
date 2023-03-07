using System.Collections.Generic;
using Mandara.Entities;

namespace Mandara.Business.Cache.EventArgs
{
    public class PortfoliosChangedEventArgs : System.EventArgs
    {
        public List<Portfolio> Portfolios { get; set; }

        public PortfoliosChangedEventArgs(List<Portfolio> portfolios)
        {
            Portfolios = portfolios;
        }
    }
}