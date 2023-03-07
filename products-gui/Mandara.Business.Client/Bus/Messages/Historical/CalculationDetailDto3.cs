using AutoMapper;
using Mandara.Business.Json;
using Mandara.Entities;
using Mandara.Entities.Calculation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Mandara.Business.Bus.Messages.Historical
{
    public class CalculationDetailDto3
    {
        public List<SourceDetailDto> SourceDetails { get; set; }
        [JsonProperty(ItemConverterType = typeof(SdaKeyValuePairConverter))]
        public List<KeyValuePair<int, decimal>> SourceDetailAmounts { get; set; }

        public Guid DetailId { get; set; }

        public DateTime CalculationDate { get; set; }
        public String ProductCategory { get; set; }
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

        public List<DailyDetail> DailyDetails { get; set; }

        public string StripName { get; set; }
    }
}