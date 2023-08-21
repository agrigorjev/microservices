using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Historical;
using Mandara.Business.Json;
using Mandara.Entities.Dto;
using Mandara.Entities.Enums;
using Newtonsoft.Json;

namespace Mandara.Business.Bus.Messages.PricingReport
{
    public class PricingCalculationDetailDto
    {
        public Guid DetailId { get; set; }
        public DateTime CalculationDate { get; set; }
        public DateTime RolloffTime { get; set; }
        public String ProductCategory { get; set; }
        public String Product { get; set; }
        public DateTime Period { get; set; }
        public String Source { get; set; }
        public Decimal Amount { get; set; }
        public Int32 ProductId { get; set; }
        public Int32 SourceProductId { get; set; }
        public List<SourceDetailDto> SourceDetails { get; set; }
        [JsonProperty(ItemConverterType = typeof(SdaKeyValuePairConverter))]
        public List<KeyValuePair<int, decimal>> SourceDetailAmounts { get; set; }
        public ProductDto ProductReference { get; set; }
    }
}