using AutoMapper;
using Mandara.Entities;
using Mandara.Entities.Calculation;
using System;
using System.Collections.Generic;
using Mandara.Entities.Positions;

namespace Mandara.Business.Bus.Messages.Positions
{
    public class CalculationDetailDto
    {
        public Guid DetailId { get; set; }

        public DateTime CalculationDate { get; set; }
        public Int32 ProductCategoryId { get; set; }
        public String ProductCategory { get; set; }
        public String ProductCategoryAbbreviation { get; set; }
        public String Product { get; set; }
        public String Source { get; set; }
        public Decimal Amount { get; set; }
        public Int32 ProductId { get; set; }
        public Int32 SourceProductId { get; set; }
        public DateTime ProductDate { get; set; }

        public ProductDateType ProductDateType { get; set; }
        public string MappingColumn { get; set; }
        public decimal? PnlFactor { get; set; }
        public decimal? PositionFactor { get; set; }

        public decimal HistoricalAmount { get; set; }
        public String DataType { get; set; }

        public Decimal AmountInner { get; set; }

        public int? PortfolioId { get; set; }

        public SortedList<DateTime, DateQuantity> QuantityByDate { get; set; }
        public List<DailyDetail> DailyDetails { get; set; }

        public string StripName { get; set; }
    }
}