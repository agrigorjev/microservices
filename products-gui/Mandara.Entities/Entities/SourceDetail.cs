using Mandara.Entities.EntitiesCustomization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace Mandara.Entities
{
    [Table("source_data_details")]
    public partial class SourceDetail
    {
        [Column("source_detail_id")]
        [Key]
        public int SourceDetailId { get; set; }

        [Column("source_data_id")]
        public int SourceDataId { get; set; }

        [Column("trade_price")]
        public decimal TradePrice { get; set; }

        [Column("quantity")]
        public decimal? Quantity { get; set; }

        public decimal GetQuantity()
        {
            return Quantity ?? 0M;
        }

        [Column("product_id")]
        public int product_id { get; set; }

        [Column("product_date", TypeName = "date")]
        public DateTime ProductDate { get; set; }

        [Column("date_type")]
        public short DateTypeDb { get; set; }

        [Column("account_number")]
        [StringLength(50)]
        public string AccountNumber { get; set; }

        [Column("exchange_code")]
        [StringLength(10)]
        public string ExchangeCode { get; set; }

        [Column("instrument_description")]
        [StringLength(255)]
        public string InstrumentDescription { get; set; }

        [Column("expiry_date", TypeName = "date")]
        public DateTime? ExpiryDate { get; set; }

        [Column("fee_exchange")]
        public decimal? FeeExchange { get; set; }

        [Column("fee_nfa")]
        public decimal? FeeNfa { get; set; }

        [Column("fee_commission")]
        public decimal? FeeCommission { get; set; }

        [Column("fee_clearing")]
        public decimal? FeeClearing { get; set; }

        [Column("future_code")]
        [StringLength(50)]
        public string FutureCode { get; set; }

        [Column("trade_date")]
        [StringLength(50)]
        public string TradeDate { get; set; }

        [Column("trade_type")]
        [StringLength(10)]
        public string TradeType { get; set; }

        [ForeignKey("product_id")]
        public virtual Product Product
        {
            get { return _product; }
            set
            {
                _product = value;
                product_id = _product != null ? _product.ProductId : 0;
            }
        }

        [ForeignKey("SourceDataId")]
        public virtual SourceData SourceData
        {
            get { return _sourceData; }
            set
            {
                _sourceData = value;
                SourceDataId = _sourceData != null ? _sourceData.SourceDataId : 0;
            }
        }

        [Column("security_definition_id")]
        public virtual int? SecurityDefinitionId { get; set; }

        [ForeignKey("SecurityDefinitionId")]
        public virtual SecurityDefinition SecurityDefinition
        {
            get { return _securityDefinition; }
            set
            {
                _securityDefinition = value;
                SecurityDefinitionId = _securityDefinition != null ? _securityDefinition.SecurityDefinitionId : 0;
            }
        }

        public virtual ICollection<PrecalcSourcedetailsDetail> PrecalcDetails { get; set; }

        // This is not ForeignKey.
        [NotMapped]
        public TradeCapture TradeCapture { get; set; }

        private Strip _strip = Strip.Default;

        [NotMapped]
        public Strip Strip
        {
            get
            {
                if (_strip.IsDefault())
                {
                    _strip = StripParser.Parse(this);
                }

                return _strip;
            }
        }

        [NotMapped]
        [DataMember]
        public bool IsTimeSpread { get; set; }

        [NotMapped]
        [DataMember]
        public ProductDateType DateType
        {
            get
            {
                return (ProductDateType)DateTypeDb;
            }
            set
            {
                DateTypeDb = (Int16)value;
            }
        }

        [NotMapped]
        [DataMember]
        public DateTime RawDate { get; set; }

        [NotMapped]
        [DataMember]
        public DateTime TradeEndDate { get; set; }

        [NotMapped]
        [DataMember]
        public DateTime? TransactTime { get; set; }

        [NotMapped]
        [DataMember]
        public int TradeCaptureId { get; set; }

        [NotMapped]
        [DataMember]
        public int? PortfolioId { get; set; }

        [NotMapped]
        [DataMember]
        public DateTime? MaturityDate { get; set; }

        [NotMapped]
        [DataMember]
        public string StripName { get; set; }

        [NotMapped]
        [DataMember]
        public string OrderNumber { get; set; }

        [NotMapped]
        [DataMember]
        public int ProductId { get; set; }

        [NotMapped]
        [DataMember]
        public DateTime ProductDate1 { get; set; }

        [NotMapped]
        [DataMember]
        public ProductDateType DateType1 { get; set; }

        [NotMapped]
        [DataMember]
        public DateTime ProductDate2 { get; set; }

        [NotMapped]
        [DataMember]
        public ProductDateType DateType2 { get; set; }

        /// <summary>
        /// ExchangeCode code for ABN mapping as ExchangeCode. Trimmed because of nchar in db
        /// </summary>
        [NotMapped]
        [DataMember]
        public string Exchange_Code
        {
            get
            {
                return (ExchangeCode ?? "").Trim();
            }
            set
            {
                ExchangeCode = value;
            }
        }

        //Seals Extension according IRM-237
        [NotMapped]
        [DataMember]
        public string BusinessStateName { get; set; }

        [NotMapped]
        [DataMember]
        public string TradeId { get; set; }

        [NotMapped]
        [DataMember]
        public string ExchangeId { get; set; }

        [NotMapped]
        [DataMember]
        public string Volume { get; set; }

        [NotMapped]
        [DataMember]
        public string Market_Expiry { get; set; }

        [NotMapped]
        [DataMember]
        public string Market_Product { get; set; }

        [NotMapped]
        [DataMember]
        public string BuySell { get; set; }

        [NotMapped]
        [DataMember]
        public string OptionType { get; set; }

        [NotMapped]
        [DataMember]
        public string Strike { get; set; }

        [NotMapped]
        [DataMember]
        public string Price { get; set; }

        [NotMapped]
        [DataMember]
        public string Expr1 { get; set; }

        [NotMapped]
        [DataMember]
        public string Reference1 { get; set; }

        [NotMapped]
        [DataMember]
        public string ToMemberId { get; set; }

        [NotMapped]
        [DataMember]
        public string ToMember { get; set; }

        [NotMapped]
        [DataMember]
        public string ExchangeOrderNumber { get; set; }

        [NotMapped]
        [DataMember]
        public string ExchangeTradeNumber { get; set; }

        [NotMapped]
        [DataMember]
        public string Reference2 { get; set; }

        [NotMapped]
        [DataMember]
        public string Reference3 { get; set; }

        [NotMapped]
        [DataMember]
        public string GiveupRef1 { get; set; }

        [NotMapped]
        [DataMember]
        public string GiveupRef2 { get; set; }

        [NotMapped]
        [DataMember]
        public string GiveupRef3 { get; set; }

        [NotMapped]
        [DataMember]
        public string Trader { get; set; }

        [NotMapped]
        [DataMember]
        public string AllocationId { get; set; }

        [NotMapped]
        [DataMember]
        public string ExchangeMember { get; set; }

        [NotMapped]
        [DataMember]
        public string TradingDay { get; set; }

        [NotMapped]
        [DataMember]
        public string ClearingDay { get; set; }

        [NotMapped]
        [DataMember]
        public string CounterPartyMember { get; set; }

        [NotMapped]
        [DataMember]
        public string OpenClose { get; set; }

        [NotMapped]
        [DataMember]
        public string PositionAccount { get; set; }

        [NotMapped]
        [DataMember]
        public string BOAccount { get; set; }

        [NotMapped]
        [DataMember]
        public string BOReference { get; set; }

        [NotMapped]
        [DataMember]
        public string OriginTransferType { get; set; }

        [NotMapped]
        [DataMember]
        public string OriginFromMember { get; set; }

        [NotMapped]
        [DataMember]
        public string TradingTradeType { get; set; }

        [NotMapped]
        [DataMember]
        public string TradePriceType { get; set; }

        [NotMapped]
        [DataMember]
        public string Venue { get; set; }

        [NotMapped]
        [DataMember]
        public string ComboType { get; set; }

        [NotMapped]
        [DataMember]
        public string OriginalClearingDay { get; set; }

        [NotMapped]
        [DataMember]
        public string Note { get; set; }

        [NotMapped]
        [DataMember]
        public string TradeGroupId { get; set; }

        [NotMapped]
        [DataMember]
        public string ClientId { get; set; }

        [NotMapped]
        [DataMember]
        public string ClientName { get; set; }

        [NotMapped]
        public List<DateTime> BusinessDays1 { get; set; }

        [NotMapped]
        public int BusinessDaysElapsed1 { get; set; }

        [NotMapped]
        public int PricingStartDay1 { get; set; }

        [NotMapped]
        public List<DateTime> BusinessDays2 { get; set; }

        [NotMapped]
        public int BusinessDaysElapsed2 { get; set; }

        [NotMapped]
        public int PricingStartDay2 { get; set; }

        private bool _useFirstLegForBusinessDays = true;
        private Product _product;
        private SourceData _sourceData;
        private SecurityDefinition _securityDefinition;

        public void SetBusinessDaysAndElapsed(List<DateTime> businessDaysArray, int businessDaysElapsed,
                                              int pricingStartDay = 0)
        {
            if (_useFirstLegForBusinessDays)
            {
                BusinessDays1 = businessDaysArray.ToList();
                BusinessDaysElapsed1 = businessDaysElapsed;
                PricingStartDay1 = pricingStartDay;
            }
            else
            {
                BusinessDays2 = businessDaysArray.ToList();
                BusinessDaysElapsed2 = businessDaysElapsed;
                PricingStartDay2 = pricingStartDay;
            }
        }

        public void SetElapsedToMax()
        {
            if (_useFirstLegForBusinessDays)
            {
                BusinessDaysElapsed1 = BusinessDays1 == null ? 0 : BusinessDays1.Count;
            }
            else
            {
                BusinessDaysElapsed2 = BusinessDays2 == null ? 0 : BusinessDays2.Count;
            }
        }

        public bool BusinessDaysNotSet()
        {
            if (_useFirstLegForBusinessDays)
            {
                return BusinessDays1 == null;
            }

            return BusinessDays2 == null;
        }

        public void SetOfficialProductProps(Product legProduct, decimal legFactor)
        {
            if (_useFirstLegForBusinessDays)
            {
                OfficialProductId1 = legProduct.OfficialProduct.OfficialProductId;
                MappingColumn1 = legProduct.OfficialProduct.MappingColumn;
                ProductId1 = legProduct.ProductId;
                PositionFactor1 = legProduct.PositionFactor ?? 1M;
                LegFactor1 = legFactor;
            }
            else
            {
                OfficialProductId2 = legProduct.OfficialProduct.OfficialProductId;
                MappingColumn2 = legProduct.OfficialProduct.MappingColumn;
                ProductId2 = legProduct.ProductId;
                PositionFactor2 = legProduct.PositionFactor ?? 1M;
                LegFactor2 = legFactor;
            }
        }

        [NotMapped]
        public decimal LegFactor1 { get; set; }

        [NotMapped]
        public decimal LegFactor2 { get; set; }

        [NotMapped]
        public string MappingColumn1 { get; set; }

        [NotMapped]
        public int OfficialProductId1 { get; set; }

        [NotMapped]
        public int ProductId1 { get; set; }

        [NotMapped]
        public decimal PositionFactor1 { get; set; }

        [NotMapped]
        public string MappingColumn2 { get; set; }

        [NotMapped]
        public int OfficialProductId2 { get; set; }

        [NotMapped]
        public int ProductId2 { get; set; }

        [NotMapped]
        public decimal PositionFactor2 { get; set; }

        [NotMapped]
        public bool UseExpiryCalendar { get; set; }

        [NotMapped]
        public bool UseFirstLegForBusinessDays
        {
            get { return _useFirstLegForBusinessDays; }
        }

        [NotMapped]
        public bool CantCalculatePositions { get; set; }

        public void ChangeToFirstLeg()
        {
            _useFirstLegForBusinessDays = true;
        }

        public void ChangeToSecondLeg()
        {
            _useFirstLegForBusinessDays = false;
        }
    }
}
