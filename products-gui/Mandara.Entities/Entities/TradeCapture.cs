using Mandara.Entities.EntitiesCustomization;
using Mandara.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Mandara.Data;
using Mandara.Date;

namespace Mandara.Entities
{
    [Serializable]
    [Table("trade_captures")]
    public partial class TradeCapture : INewable
    {
        public TradeCapture()
        {
            TradeChanges = new HashSet<TradeChange>();
            PrecalcDetails = new HashSet<PrecalcTcDetail>();
            // TODO: It's not known whether it's safe to do this.  Lots of code in IRM relies on the possibility of 
            // pretty much anything being null.
            //BusinessDays1 = new List<DateTime>();
            //BusinessDays2 = new List<DateTime>();
            //PrecalcDetails = new List<PrecalcTcDetail>();
            //PrecalcPositions = new List<PrecalcDetail>();
        }

        [Column("idTradeCapture")]
        [Key]
        public int TradeId { get; set; }

        [Column("TradeReportID")]
        [StringLength(50)]
        public string TradeReportID { get; set; }

        [Column("TradeReportTransType")]
        [StringLength(5)]
        public string TradeReportTransType { get; set; }

        [Column("TrdType")]
        [StringLength(25)]
        public string TrdType { get; set; }

        [Column("OrigTradeID")]
        [StringLength(50)]
        public string OrigTradeID { get; set; }

        [Column("ExecID")]
        [StringLength(30)]
        public string ExecID { get; set; }

        [Column("ExecType")]
        [StringLength(15)]
        public string ExecType { get; set; }

        [Column("OrdStatus")]
        [StringLength(15)]
        public string OrdStatus { get; set; }

        [Column("Symbol")]
        [StringLength(50)]
        public string Symbol { get; set; }

        [Column("SecurityID")]
        [StringLength(45)]
        public string SecurityID { get; set; }

        [Column("SecurityIDSource")]
        [StringLength(12)]
        public string SecurityIDSource { get; set; }

        [Column("CFICode")]
        [StringLength(10)]
        public string CFICode { get; set; }

        [Column("LastQty")]
        public decimal? Quantity { get; set; }

        [Column("LastPx")]
        public decimal? Price { get; set; }

        [Column("NumOfLots")]
        public int? NumOfLots { get; set; }

        [Column("NumOfCycles")]
        public int? NumOfCycles { get; set; }

        [Column("TradeDate")]
        public DateTime? TradeDate { get; set; }

        [Column("Side")]
        [StringLength(6)]
        public string Side { get; set; }

        [Column("OrderID")]
        [StringLength(50)]
        public string OrderID { get; set; }

        [Column("ClOrdID")]
        [StringLength(50)]
        public string ClOrdID { get; set; }

        [Column("TimeStamp")]
        public DateTime? TimeStamp { get; set; }

        [Column("OriginationTrader")]
        [StringLength(50)]
        public string OriginationTrader { get; set; }

        [Column("ClearingAccountId")]
        [StringLength(50)]
        public string ClearingAccountId { get; set; }

        [Column("ClearingFirm")]
        [StringLength(50)]
        public string ClearingFirm { get; set; }

        [Column("OriginationFirm")]
        [StringLength(50)]
        public string OriginationFirm { get; set; }

        [Column("TransactTime")]
        public DateTime? TransactTime { get; set; }

        [Column("UtcTransactTime")]
        public DateTime UtcTransactTime { get; set; }

        [Column("ExecutingFirm")]
        [StringLength(150)]
        public string ExecutingFirm { get; set; }

        [Column("Exchange")]
        [StringLength(50)]
        public string Exchange { get; set; }

        [Column("LegRefID")]
        [StringLength(50)]
        public string LegRefID { get; set; }

        [Column("TradeType")]
        public int? TradeType { get; set; }

        [Column("CreatedBy")]
        [StringLength(255)]
        public string CreatedBy { get; set; }

        [Column("TradeStartDate")]
        public DateTime? TradeStartDate { get; set; }

        [Column("TradeEndDate")]
        public DateTime? TradeEndDate { get; set; }

        [Column("PortfolioId")]
        public int? PortfolioId { get; set; }

        [Column("IsParentTrade")]
        public bool? IsParentTrade { get; set; }

        [Column("BuyBook")]
        public int? BuyBookID { get; set; }

        [Column("SellBook")]
        public int? SellBookID { get; set; }

        [Column("EditCancelReason")]
        [StringLength(400)]
        public string EditCancelReason { get; set; }

        [Column("GroupId")]
        public int? GroupId { get; set; }

        [Column("idSecurityDefinition")]
        public int SecurityDefinitionId { get; set; }

        [Column("IsTransferSell")]
        public bool? IsTransferSell { get; set; }

        [Column("fee_exchange")]
        public decimal? FeeExchangeDb { get; set; }

        [Column("fee_nfa")]
        public decimal? FeeNfaDb { get; set; }

        [Column("fee_commission")]
        public decimal? FeeCommissionDb { get; set; }

        [Column("fee_clearing")]
        public decimal? FeeClearingDb { get; set; }

        [Column("Pending")]
        public bool? Pending { get; set; }

        [Column("TotalQty")]
        public decimal? TotalQty { get; set; }

        [Column("AveragePx")]
        public decimal? AveragePx { get; set; }

        [Column("Brokerage")]
        public decimal? Brokerage { get; set; }

        [Column("fee_block_trade")]
        public decimal? FeeBlockDb { get; set; }

        [Column("fee_platts_trade")]
        public decimal? FeePlattsDb { get; set; }

        [Column("ExpiryDate")]
        public DateTime? ExpiryDate { get; set; }

        [Column("IceSpreadRebate")]
        public decimal? IceSpreadRebateDb { get; set; }

        [Column("ExchangeOverride")]
        [StringLength(50)]
        public string ExchangeOverride { get; set; }

        [ForeignKey("BuyBookID")]
        public virtual Portfolio BuyBook
        {
            get { return _buyBook; }
            set
            {
                _buyBook = value;
                BuyBookID = _buyBook != null ? _buyBook.PortfolioId : (int?)null;
            }
        }

        [ForeignKey("PortfolioId")]
        public virtual Portfolio Portfolio
        {
            get { return _portfolio; }
            set
            {
                _portfolio = value;
                PortfolioId = _portfolio != null ? _portfolio.PortfolioId : (int?)null;
            }
        }

        [ForeignKey("SellBookID")]
        public virtual Portfolio SellBook
        {
            get { return _sellBook; }
            set
            {
                _sellBook = value;
                SellBookID = _sellBook != null ? _sellBook.PortfolioId : (int?)null;
            }
        }

        [ForeignKey("GroupId")]
        public virtual TradeGroup TradeGroup
        {
            get { return _tradeGroup; }
            set
            {
                _tradeGroup = value;
                GroupId = _tradeGroup != null ? _tradeGroup.GroupId : (int?)null;
            }
        }

        [ForeignKey("SecurityDefinitionId")]
        public virtual SecurityDefinition SecurityDefinition
        {
            get { return _securityDefinition; }
            set
            {
                _securityDefinition = value;
                SecurityDefinitionId = _securityDefinition != null ? _securityDefinition.SecurityDefinitionId : 0;

                LoggerHelper.LogSecDef(_securityDefinition);
            }
        }

        public bool HasProduct => null != SecurityDefinition?.Product && !SecurityDefinition.Product.IsDefault;

        public void ClearSecurityDefinition()
        {
            _securityDefinition = null;
        }

        public const int NoId = -1;
        public const decimal DefaultQuantity = 0M;
        public const decimal DefaultPrice = 0M;
        public const decimal DefaultCost = 0M;
        public const string DefaultStringField = "";
        public const string DefaultOrderStatus = "Fake";
        public const string NoSide = "NoSide";
        public static readonly DateTime ItNeverHappened = DateTime.MinValue;

        public static TradeCapture Default = new TradeCapture()
        {
            TradeId = NoId,
            SecurityDefinitionId = NoId,
            Quantity = DefaultQuantity,
            TotalQty = DefaultQuantity,
            Price = DefaultPrice,
            AveragePx = DefaultPrice,
            Exchange = Mandara.Entities.Exchange.IceExchangeName,
            Brokerage = DefaultCost,
            BusinessDays1 = new List<DateTime>(),
            BusinessDays2 = new List<DateTime>(),
            BuyBookID = NoId,
            CFICode = DefaultStringField,
            ClOrdID = DefaultStringField,
            ExecID = DefaultStringField,
            ExpiryDate = ItNeverHappened,
            IsParentTrade = true,
            LegRefID = DefaultStringField,
            LivePrice = DefaultPrice,
            OrdStatus = DefaultOrderStatus,
            ParentID = NoId,
            PortfolioId = NoId,
            PrecalcDetails = new List<PrecalcTcDetail>(),
            PrecalcPositions = new List<PrecalcDetail>(),
            Side = NoSide,
            TimeStamp = ItNeverHappened,
            TradeDate = ItNeverHappened,
            TradeEndDate = ItNeverHappened,
            TradeStartDate = ItNeverHappened,
            TransactTime = ItNeverHappened,
            UtcTransactTime = ItNeverHappened,
        };

        public bool IsDefault()
        {
            return NoId == TradeId
                   && NoId == SecurityDefinitionId
                   && DefaultQuantity == Quantity
                   && DefaultQuantity == TotalQty
                   && DefaultPrice == Price
                   && DefaultPrice == AveragePx
                   && Mandara.Entities.Exchange.IceExchangeName == Exchange
                   && DefaultCost == Brokerage
                   && !(BusinessDays1?.Any() ?? false)
                   && !(BusinessDays2?.Any() ?? false)
                   && NoId == BuyBookID
                   && DefaultStringField == CFICode
                   && DefaultStringField == ClOrdID
                   && DefaultStringField == ExecID
                   && ItNeverHappened == ExpiryDate
                   && (IsParentTrade ?? true)
                   && DefaultStringField == LegRefID
                   && DefaultPrice == LivePrice
                   && DefaultOrderStatus == OrdStatus
                   && NoId == ParentID
                   && NoId == PortfolioId
                   && !(PrecalcDetails?.Any() ?? false)
                   && !(PrecalcPositions?.Any() ?? false)
                   && NoSide == Side
                   && ItNeverHappened == TimeStamp
                   && ItNeverHappened == TradeDate
                   && ItNeverHappened == TradeEndDate
                   && ItNeverHappened == TradeStartDate
                   && ItNeverHappened == TransactTime
                   && ItNeverHappened == UtcTransactTime;
        }

        public virtual ICollection<TradeChange> TradeChanges { get; set; }
        public virtual ICollection<PrecalcTcDetail> PrecalcDetails { get; set; }


        [NotMapped]
        public TradeCaptureMessageType MessageType { get; set; }

        [NotMapped]
        public decimal? PnL { get; set; }

        [NotMapped]
        public decimal? LivePrice { get; set; }

        [NotMapped]
        public decimal? OvernightPrice { get; set; }

        [NotMapped]
        public bool IsChild
        {
            get
            {
                return !String.IsNullOrEmpty(LegRefID) && (ExecID != LegRefID);
            }
        }

        [NotMapped]
        public int? ParentID
        {
            get;
            set;
        }

        [NotMapped]
        public bool? IsParentTimeSpread { get; set; }
        //04-25-2013 EZ:Add GMI Codes for transfet trade

        [NotMapped]
        public string GMICode { get; set; }

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
                OfficialProductId1 = legProduct.OfficialProduct.SettlementProductId ?? legProduct.OfficialProduct.OfficialProductId;
                MappingColumn1 = legProduct.OfficialProduct.MappingColumn;
                ProductId1 = legProduct.ProductId;
                PositionFactor1 = legProduct.PositionFactor ?? 1M;
                LegFactor1 = legFactor;
            }
            else
            {
                OfficialProductId2 = legProduct.OfficialProduct.SettlementProductId ?? legProduct.OfficialProduct.OfficialProductId;
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

        public void ChangeToFirstLeg()
        {
            _useFirstLegForBusinessDays = true;
        }

        public void ChangeToSecondLeg()
        {
            _useFirstLegForBusinessDays = false;
        }

        [NotMapped]
        public decimal FeeClearing
        {
            get
            {
                decimal coefficient = IsParentTimeSpread == true ? 2M : 1M;

                return coefficient * (FeeClearingDb ?? 0M);
            }
        }

        [NotMapped]
        public decimal FeePlatts
        {
            get
            {
                decimal coefficient = IsParentTimeSpread == true ? 2M : 1M;

                return coefficient * (FeePlattsDb ?? 0M);
            }
        }

        [NotMapped]
        public decimal FeeCommission
        {
            get
            {
                decimal coefficient = IsParentTimeSpread == true ? 2M : 1M;

                return coefficient * (FeeCommissionDb ?? 0M);
            }
        }

        [NotMapped]
        public decimal FeeExchange
        {
            get
            {
                decimal coefficient = IsParentTimeSpread == true ? 2M : 1M;

                return coefficient * (FeeExchangeDb ?? 0M);
            }
        }

        public decimal FeeNfa
        {
            get
            {
                decimal coefficient = IsParentTimeSpread == true ? 2M : 1M;

                return coefficient * (FeeNfaDb ?? 0M);
            }
        }

        [NotMapped]
        public decimal FeeBlock
        {
            get
            {
                decimal coefficient = IsParentTimeSpread == true ? 2M : 1M;

                return coefficient * (FeeBlockDb ?? 0M);
            }
        }

        public decimal IceSpreadRebate
        {
            get
            {
                return IceSpreadRebateDb ?? 0M;
            }
        }

        [NotMapped]
        public bool IsTasTrade
        {
            get { return SecurityDefinition.Product.TasType == TasType.Tas; }
        }

        private Strip _strip = Strip.Default;
        private Portfolio _buyBook;
        private Portfolio _portfolio;
        private Portfolio _sellBook;
        private TradeGroup _tradeGroup;
        private SecurityDefinition _securityDefinition;

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

        private List<PrecalcDetail> _daysPositions;

        [NotMapped]
        public List<PrecalcDetail> PrecalcPositions
        {
            get
            {
                if (_daysPositions != null)
                    return _daysPositions;

                if (SecurityDefinition == null)
                    return null;

                if (SecurityDefinition.HasPrecalcDetails())
                {
                    _daysPositions = SecurityDefinition.PrecalcDetails
                        .Select(x => new PrecalcDetail
                        {
                            Month = x.Month,
                            Product = x.Product,
                            ProductId = x.ProductId,
                            DaysPositions = x.DaysPositions
                        }).ToList();
                }

                if (HasPrecalcDetailsOnTrade())
                {
                    _daysPositions = PrecalcDetails
                        .Select(x => new PrecalcDetail
                        {
                            Month = x.Month,
                            Product = x.Product,
                            ProductId = x.ProductId,
                            DaysPositions = x.DaysPositions
                        }).ToList();
                }

                return _daysPositions;
            }
            set { _daysPositions = value; }
        }

        [NotMapped]
        public DateTime? LastReplacedTimestamp { get; set; }

        [NotMapped]
        public string StripName => SecurityDefinition?.Product != null ? GetProductTypeStripName() : null;

        private string GetProductTypeStripName()
        {
            return SecurityDefinition.Product.Type.IsDaily()
                ? GetDailyProductStripName()
                : SecurityDefinition.StripName;
        }

        private string GetDailyProductStripName()
        {
            string stripStart = TradeStartDate?.ToString("MMMdd", DateTimeFormatInfo.InvariantInfo) ?? String.Empty;
            string stripEnd = TradeEndDate?.ToString("MMMdd", DateTimeFormatInfo.InvariantInfo) ?? String.Empty;
            string hubAlias = TradeStartDate != null ? GetStripHubAlias() : String.Empty;

            return $"{stripStart}-{stripEnd}({hubAlias})";
        }

        private string GetStripHubAlias()
        {
            return AddTimeSpanFromHubAlias(TradeStartDate.Value, SecurityDefinition.HubAlias)
                .ToString("MMM", DateTimeFormatInfo.InvariantInfo);
        }

        [NotMapped]
        public TradeCapture Leg1Trade { get; set; }

        [NotMapped]
        public TradeCapture Leg2Trade { get; set; }

        private DateTime AddTimeSpanFromHubAlias(DateTime dateTime, string hubAlias)
        {
            if (!string.IsNullOrEmpty(hubAlias))
            {
                Match match = Regex.Match(
                    hubAlias,
                    @"(?<count>[0-9]+)(st|nd|rd|th)\ *(?<period>((D|d)ay|(M|m)onth|(Y|y)ear))");

                if (match.Success)
                {
                    int count;
                    if (!int.TryParse(match.Groups["count"].Value, out count))
                        count = 0;

                    string period = match.Groups["period"].Value;
                    if (string.Equals(period, "Day", StringComparison.OrdinalIgnoreCase))
                    {
                        return dateTime.AddDays(count);
                    }
                    if (string.Equals(period, "Month", StringComparison.OrdinalIgnoreCase))
                    {
                        return dateTime.AddMonths(count);
                    }
                    if (string.Equals(period, "Year", StringComparison.OrdinalIgnoreCase))
                    {
                        return dateTime.AddYears(count);
                    }
                }
            }

            return dateTime;
        }

        public List<Money> GetLiveCosts(Product product)
        {
            List<Money> costs = new List<Money>();
            costs.Add(new Money(FeeClearing, product.FeeClearingCurrency != null ? product.FeeClearingCurrency.IsoName : "USD"));
            costs.Add(new Money(FeeCommission, product.FeeCommissionCurrency != null ? product.FeeCommissionCurrency.IsoName : "USD"));
            costs.Add(new Money(FeeExchange, product.FeeExchangeCurrency != null ? product.FeeExchangeCurrency.IsoName : "USD"));
            costs.Add(new Money(FeeNfa, product.FeeNfaCurrency != null ? product.FeeNfaCurrency.IsoName : "USD"));
            costs.Add(new Money(FeeBlock, product.FeeBlockCurrency != null ? product.FeeBlockCurrency.IsoName : "USD"));
            costs.Add(new Money(FeePlatts, product.FeePlattsCurrency != null ? product.FeePlattsCurrency.IsoName : "USD"));
            costs.Add(new Money(-IceSpreadRebate, product.FeeExchangeCurrency != null ? product.FeeExchangeCurrency.IsoName : "USD"));

            costs =
                costs.GroupBy(x => x.Currency)
                    .Select(group => new Money { Currency = group.Key, Amount = group.Sum(a => a.Amount) }).ToList();

            return costs;
        }

        public override string ToString()
        {
            return string.Format(
                "Product Description: {0}, Strip Name: {1}, Quantity: {2}.",
                SecurityDefinition != null ? SecurityDefinition.ProductDescription : "",
                SecurityDefinition != null ? SecurityDefinition.StripName : "",
                Quantity != null ? Quantity.ToString() : "NULL");
        }

        public Strip GetStrip(Product product)
        {
            if (_strip.IsDefault())
            {
                _strip = StripParser.Parse(this, product);
            }

            return _strip;
        }

        public void UpdateStrip()
        {
            _strip = StripParser.Parse(this);
        }

        public static bool IsNonMonthlyStrip(
            IList<Product> products,
            SecurityDefinition securityDefinition,
            ProductDateType stripPart1DateType)
        {
            bool isBalmoOrDailyProduct = IsBalmoOrDailyProduct(products);
            bool isSecurityDefinitionNontriptrip = false;
            bool isCustomStrip = false;

            if (securityDefinition.Strip1DateType != null)
            {
                isSecurityDefinitionNontriptrip = securityDefinition.IsNonMonthlySecurityDefinitionStrip();
            }
            else
            {
                isCustomStrip = stripPart1DateType == ProductDateType.Custom;
            }

            return isBalmoOrDailyProduct || isCustomStrip || isSecurityDefinitionNontriptrip;
        }

        private static bool IsBalmoOrDailyProduct(IList<Product> products)
        {
            return products.Any(product => product.Type == ProductType.Balmo || product.IsProductDaily);
        }

        public bool IsTas()
        {
            return SecurityDefinition.Product.TasType != TasType.NotTas && (IsIce() || IsNymex());
        }

        public bool IsIce()
        {
            return "ice".Equals(Exchange, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsNymex()
        {
            return SecurityDefinition.Product.IsNymex();
        }

        private string _key = null;
        public string Key
        {
            get
            {
                if (_key == null)
                {
                    _key = string.Format(
                    "{0}_{1}_{2}_{3}",
                    ExecID,
                    (TradeDate ?? DateTime.MinValue).ToSortableShortDate(),
                    ClOrdID ?? "",
                    PortfolioId ?? 0);
                }

                return _key;
            }
        }

        public bool IsNew()
        {
            return 0 == TradeId;
        }

        public string GetCurrency()
        {
            return SecurityDefinition?.Product?.OfficialProduct?.Currency?.IsoName ?? string.Empty;
        }

        public bool HasPrecalcDetailsOnTrade()
        {
            return PrecalcDetails != null && PrecalcDetails.Count > 0;
        }

        public bool HasPrecalcDetails()
        {
            return HasPrecalcDetailsOnTrade() ||
                   (SecurityDefinition != null && SecurityDefinition.HasPrecalcDetails());
        }

        public bool IsManualTrade()
        {
            return null != TradeType;
        }

        public bool ReferencesNewSecurityDef()
        {
            return 0 == SecurityDefinitionId;
        }

        public bool ReferencesExistingSecurityDef()
        {
            return !ReferencesNewSecurityDef() || (null != SecurityDefinition && !SecurityDefinition.IsNew());
        }
    }
}
