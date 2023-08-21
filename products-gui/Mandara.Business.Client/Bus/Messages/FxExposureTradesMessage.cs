using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.Trades;

namespace Mandara.Business.Bus.Messages
{
    [Serializable]
    public class FxExposureTradesMessage : SnapshotMessageBase
    {
        public int PortfolioId { get; set; }

        public int CurrencyId { get; set; }

        public List<DateTime> CalculationDates { get; set; }

        public List<FxExposureTradeView> Trades { get; set; }
    }
}
