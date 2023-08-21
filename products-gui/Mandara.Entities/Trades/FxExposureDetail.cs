using System;
using System.Collections.Generic;
using Mandara.Entities.Calculation;
using Newtonsoft.Json;

namespace Mandara.Entities.Trades
{
    [Serializable]
    public class FxExposureDetail
    {
        public DateTime CalculationDate { get; set; }

        public int PortfolioId { get; set; }

        public decimal Hedge { get; set; }

        public decimal Notional { get; set; }

        public int CurrencyId { get; set; }

        [JsonIgnore]
        public List<CoeffEntityId> EntityIds { get; set; }

        public decimal Combined
        {
            get { return Notional + Hedge; }
        }

        public string Key
        {
            get { return PortfolioId + "_" + CalculationDate.ToShortDateString() + "_" + CurrencyId; }
        }
    }
}
