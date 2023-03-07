using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.Services.Prices
{
    public class LegData
    {
        public List<DayPosition> BusinessDaysInMonth { get; set; }
        public int TradeDaysElapsedInMonth { get; set; }
        public List<DayPosition> BusinessDays { get; set; }
        public string MappingColumn { get; set; }
        public string Currency { get; set; }
        public int OfficialProductId { get; set; }
        public int ProductId { get; set; }
        public decimal PositionFactor { get; set; }
        public decimal LegPositionFactor { get; set; }
        public decimal? LegPnlFactor { get; set; }

        public LegData()
        {
            PositionFactor = 1M;
            LegPositionFactor = 1M;
        }

        public static LegData GetDefault()
        {
            return new LegData()
            {
                BusinessDaysInMonth = new List<DayPosition>(),
                TradeDaysElapsedInMonth = Int32.MaxValue,
                MappingColumn = String.Empty,
                Currency = String.Empty,
                OfficialProductId = -1,
                ProductId = -1,
                PositionFactor = Decimal.One,
                LegPositionFactor = Decimal.One,
                LegPnlFactor = Decimal.One,
            };
        }

        public bool IsDefault()
        {
            return !BusinessDaysInMonth.Any() &&
                   TradeDaysElapsedInMonth == Int32.MaxValue &&
                   MappingColumn == String.Empty &&
                   Currency == String.Empty &&
                   OfficialProductId == -1 &&
                   ProductId == -1 &&
                   PositionFactor == Decimal.One &&
                   LegPositionFactor == Decimal.One &&
                   LegPnlFactor == Decimal.One;
        }
    }
}