using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities.Dto
{
    [Serializable]
    public class SecurityDefinitionDto
    {
        public bool? Clearable { get; set; }
        public string EndDate { get; set; }
        public string Exchange { get; set; }
        public string Granularity { get; set; }
        public string HubAlias { get; set; }
        public int? HubId { get; set; }
        public string HubName { get; set; }
        public string ImpliedType { get; set; }
        public decimal? IncrementPrice { get; set; }
        public decimal? IncrementQty { get; set; }
        public decimal? IncrementStrike { get; set; }
        public int? LotSize { get; set; }
        public decimal? LotSizeMultiplier { get; set; }
        public decimal? MaxStrike { get; set; }
        public decimal? MinStrike { get; set; }
        public int? NumOfDecimalPrice { get; set; }
        public int? NumOfDecimalQty { get; set; }
        public string PriceDenomination { get; set; }
        public string PriceUnit { get; set; }
        public string PrimaryLegSymbol { get; set; }
        public string ProductDescription { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public string SecondaryLegSymbol { get; set; }
        public int SecurityDefinitionId { get; set; }
        public string StartDate { get; set; }
        public int? StripId { get; set; }
        public string StripName { get; set; }
        public decimal? TickValue { get; set; }
        public string UnderlyingCFICode { get; set; }
        public decimal? UnderlyingContractMultiplier { get; set; }
        public string UnderlyingMaturityDate { get; set; }
        public string UnderlyingSecurityDesc { get; set; }
        public string UnderlyingSecurityID { get; set; }
        public string UnderlyingSecurityIDSource { get; set; }
        public string UnderlyingSymbol { get; set; }
        public string UnderlyingUnitOfMeasure { get; set; }

        public int? product_id { get; set; }

        public DateTime? StartDateAsDate { get; set; }

        public ProductDto Product { get; set; }
    }
}