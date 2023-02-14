using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Mandara.ProductService.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Mandara.ProductService.Data.Entities
{
    [Table("security_definitions")]
    public sealed partial class SecurityDefinition : INewable
    {
        private const int InvalidProductId = -1;

        public SecurityDefinition()
        {
            SecurityDefinitionId = 0;
            //TradeCaptures = new HashSet<TradeCapture>();
            //PrecalcDetails = new HashSet<PrecalcSdDetail>();
            //SourceDetails = new HashSet<SourceDetail>();
        }

        [Column("idSecurityDefinition")]
        [Key]
        public int SecurityDefinitionId { get; set; }

        [Column("UnderlyingSymbol")]
        [StringLength(50)]
        public string? UnderlyingSymbol { get; set; }

        [Column("UnderlyingSecurityID")]
        [StringLength(85)]
        public string? UnderlyingSecurityID { get; set; }

        [Column("UnderlyingSecurityIDSource")]
        [StringLength(5)]
        public string? UnderlyingSecurityIDSource { get; set; }

        [Column("UnderlyingCFICode")]
        [StringLength(10)]
        public string? UnderlyingCFICode { get; set; }

        [Column("UnderlyingSecurityDesc")]
        [StringLength(150)]
        public string? UnderlyingSecurityDesc { get; set; }

        [Column("UnderlyingMaturityDate")]
        [StringLength(12)]
        public string? UnderlyingMaturityDate { get; set; }

        [Column("UnderlyingContractMultiplier")]
        [Precision(28, 6)]
        public decimal? UnderlyingContractMultiplier { get; set; }

        [Column("IncrementPrice")]
        [Precision(28, 6)]
        public decimal? IncrementPrice { get; set; }

        [Column("IncrementQty")]
        [Precision(28, 6)]
        public decimal? IncrementQty { get; set; }

        [Column("LotSize")]
        public int? LotSize { get; set; }

        [Column("NumOfCycles")]
        public int? NumOfCycles { get; set; }

        [Column("LotSizeMultiplier")]
        [Precision(28, 6)]
        public decimal? LotSizeMultiplier { get; set; }

        [Column("Clearable")]
        public bool? Clearable { get; set; }

        [Column("StartDate")]
        [StringLength(12)]
        public string? StartDate { get; set; }

        [Column("EndDate")]
        [StringLength(12)]
        public string? EndDate { get; set; }

        [Column("StripId")]
        public int? StripId { get; set; }

        [Column("StripType")]
        public int? StripType { get; set; }

        [Column("StripName")]
        [StringLength(25)]
        public string? StripName { get; set; }

        [Column("HubId")]
        public int? HubId { get; set; }

        [Column("HubName")]
        [StringLength(85)]
        public string? HubName { get; set; }

        [Column("HubAlias")]
        [StringLength(85)]
        public string? HubAlias { get; set; }

        [Column("UnderlyingUnitOfMeasure")]
        [StringLength(20)]
        public string? UnderlyingUnitOfMeasure { get; set; }

        [Column("PriceDenomination")]
        [StringLength(5)]
        public string? PriceDenomination { get; set; }

        [Column("PriceUnit")]
        [StringLength(25)]
        public string? PriceUnit { get; set; }

        [Column("Granularity")]
        [StringLength(10)]
        public string? Granularity { get; set; }

        [Column("NumOfDecimalPrice")]
        public int? NumOfDecimalPrice { get; set; }

        [Column("NumOfDecimalQty")]
        public int? NumOfDecimalQty { get; set; }

        [Column("ProductId")]
        public int? ProductId { get; set; }

        [Column("ProductName")]
        [StringLength(150)]
        public string? ProductName { get; set; }

        [Column("ProductDescription")]
        [StringLength(85)]
        public string? ProductDescription { get; set; }

        [Column("TickValue")]
        [Precision(28, 6)]
        public decimal? TickValue { get; set; }

        [Column("ImpliedType")]
        [StringLength(1)]
        public string? ImpliedType { get; set; }

        [Column("PrimaryLegSymbol")]
        [StringLength(50)]
        public string? PrimaryLegSymbol { get; set; }

        [Column("SecondaryLegSymbol")]
        [StringLength(50)]
        public string? SecondaryLegSymbol { get; set; }

        [Column("IncrementStrike")]
        [Precision(28, 6)]
        public decimal? IncrementStrike { get; set; }

        [Column("MinStrike")]
        [Precision(28, 6)]
        public decimal? MinStrike { get; set; }

        [Column("MaxStrike")]
        [Precision(28, 6)]
        public decimal? MaxStrike { get; set; }

        [Column("Exchange")]
        [StringLength(50)]
        public string? Exchange { get; set; }

        [NotMapped]
        private DateTime? _startDateAsDate;

        [Column("StartDateAsDate")]
        public DateTime? StartDateAsDate
        {
            get => _startDateAsDate ?? GetStartDateAsDate();
            set => _startDateAsDate = value;
        }

        private DateTime? GetStartDateAsDate()
        {
            if (!string.IsNullOrWhiteSpace(StartDate)
                && DateTime.TryParseExact(
                    StartDate,
                    Formats.SortableShortDate,
                    null,
                    DateTimeStyles.None,
                    out DateTime startDate))
            {
                return startDate;
            }

            return Strip1Date;
        }

        [Column("EndDateAsDate")]
        public DateTime? EndDateAsDate { get; set; }

        [Column("Strip1DateType")]
        public int? Strip1DateTypeDb { get; set; }

        [Column("Strip2DateType")]
        public int? Strip2DateTypeDb { get; set; }

        [Column("product_id")]
        public int product_id
        { get; set; }

        [Column("Strip1Date")]
        public DateTime? Strip1Date { get; set; }

        [Column("Strip2Date")]
        public DateTime? Strip2Date { get; set; }

        //[ForeignKey("product_id")]
        //public Product Product
        //{
        //    get => _product;
        //    set
        //    {
        //        _product = value;
        //        product_id = _product?.ProductId;
        //    }
        //}

        //public ICollection<PrecalcSdDetail> PrecalcDetails { get; set; }

        //public ICollection<TradeCapture> TradeCaptures { get; set; }

        //public ICollection<SourceDetail> SourceDetails { get; set; }

        public override string ToString()
        {
            return $"{ProductDescription} - {StripName}";
        }

        //private Product _product;

        //[NotMapped]
        //public ProductDateType? Strip1DateType
        //{
        //    get
        //    {
        //        if (Strip1DateTypeDb.HasValue)
        //        {
        //            return (ProductDateType)Strip1DateTypeDb.Value;
        //        }

        //        return null;
        //    }
        //    set => Strip1DateTypeDb = (int?)value;
        //}

        //[NotMapped]
        //public ProductDateType? Strip2DateType
        //{
        //    get
        //    {
        //        if (Strip2DateTypeDb.HasValue)
        //        {
        //            return (ProductDateType)Strip2DateTypeDb.Value;
        //        }

        //        return null;
        //    }
        //    set => Strip2DateTypeDb = (int?)value;
        //}

        public override bool Equals(object obj)
        {
            SecurityDefinition anotherSecurityDefinition = obj as SecurityDefinition;

            if (anotherSecurityDefinition != null)
            {
                return UnderlyingSymbol.Equals(anotherSecurityDefinition.UnderlyingSymbol);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return UnderlyingSymbol.GetHashCode();
        }

        //public bool IsNonMonthlySecurityDefinitionStrip()
        //{
        //    bool isCustomStrip = Strip1DateType == ProductDateType.Custom || Strip2DateType == ProductDateType.Custom;
        //    bool isBalmoStrip = Strip1DateType == ProductDateType.Day || Strip2DateType == ProductDateType.Day;
        //    bool isDailyStrip = Strip1DateType == ProductDateType.Daily || Strip2DateType == ProductDateType.Daily;

        //    return isCustomStrip || isBalmoStrip || isDailyStrip;
        //}

        public bool IsNew()
        {
            return 0 == SecurityDefinitionId;
        }

        //public bool HasPrecalcDetails()
        //{
        //    return PrecalcDetails != null && PrecalcDetails.Count > 0;
        //}

        //public void SanitiseCircularReferences()
        //{
        //    TradeCaptures?.Clear();
        //}

        public SecurityDefinition ShallowCopy()
        {
            SecurityDefinition duplicateSecDef = (SecurityDefinition)MemberwiseClone();
            return duplicateSecDef;
        }
    }
}