using System;
using System.Collections.Generic;
using Mandara.Business.Json;
using Mandara.Entities;
using Newtonsoft.Json;

namespace Mandara.Business.Bus.Messages.Positions
{
    public class CalculationDetailDto2
    {
        [JsonProperty(ItemConverterType = typeof(SdaKeyValuePairConverter))]
        public List<KeyValuePair<int, decimal>> SourceDetailAmounts { get; set; }

        public Guid DetailId { get; set; }

        public DateTime CalculationDate { get; set; }
        public Int32 ProductCategoryId { get; set; }
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

        public string StripName { get; set; }
    }
}