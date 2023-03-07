using System;
using System.Collections.Generic;
using Mandara.Entities.Services;

namespace Mandara.Entities.Calculation
{
    public class PricingReportSourceData
    {
        public DateTime Month { get; set; }
        public decimal? TradesQuantity { get; set; }
        public List<int?> PortfolioIds { get; set; }
        public string DaysSerialized { get; set; }
        public int ProductId { get; set; }

        public Dictionary<DateTime, decimal?> DaysPositions
        {
            get
            {
                return DayPositionsSerialisation.DeserializeDaysNullablePositions(
                    DaysSerialized,
                    Month,
                    ProductId,
                    "PricingReportSourceData",
                    ProductId);
            }
        }

        public PricingReportSourceData()
        {
            TradesQuantity = 0M;
            PortfolioIds = new List<int?>();
            DaysSerialized = string.Empty;
        }
    }
}