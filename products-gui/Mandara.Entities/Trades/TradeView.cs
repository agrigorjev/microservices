using System;
using System.ComponentModel;

namespace Mandara.Entities.Trades
{
    [Serializable]
    public class TradeView
    {
        //TradeCapture properties
        [Category("Trade Capture")]
        [DisplayName("Brokerage")]
        public decimal? Brokerage { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Buy Book")]
        public string BuyBookName { get; set; }
        [Category("Trade Capture")]
        [DisplayName("CFI Code")]
        public string CFICode { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Cl Order Id")]
        public string ClOrdID { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Clearing Account Id")]
        public string ClearingAccountId { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Clearing Firm")]
        public string ClearingFirm { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Edit/Cancel Reason")]
        public string EditCancelReason { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Exchange")]
        public string Exchange { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Exec Id")]
        public string ExecID { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Exec Type")]
        public string ExecType { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Executing Firm")]
        public string ExecutingFirm { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Leg Ref Id")]
        public string LegRefID { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Number Of Cycles")]
        public int? NumOfCycles { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Number Of Lots")]
        public int? NumOfLots { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Order Status")]
        public string OrdStatus { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Order Id")]
        public string OrderID { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Orig Trade Id")]
        public string OrigTradeID { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Origination Firm")]
        public string OriginationFirm { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Origination Trader")]
        public string OriginationTrader { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Parent Id")]
        public int? ParentID { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Price")]
        public decimal? Price { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Quantity")]
        public decimal? Quantity { get; set; }
        [Category("Trade Capture")]
        [DisplayName("")]
        public string SecurityID { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Security Id Source")]
        public string SecurityIDSource { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Sell Book")]
        public string SellBookName { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Side")]
        public string Side { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Symbol")]
        public string Symbol { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Timestamp")]
        public DateTime? TimeStamp { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Trade Date")]
        public DateTime? TradeDate { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Trade End Date")]
        public DateTime? TradeEndDate { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Group")]
        public int? Group { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Trade Capture Id")]
        public int TradeCaptureId { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Trade Report Id")]
        public string TradeReportID { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Trade Report Transaction Type")]
        public string TradeReportTransType { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Trade Start Date")]
        public DateTime? TradeStartDate { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Trade Type")]
        public int? TradeType { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Transact Time")]
        public DateTime? TransactTime { get; set; }
        [Category("Trade Capture")]
        [DisplayName("UTC Transact Time")]
        public DateTime UtcTransactTime { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Trd Type")]
        public string TrdType { get; set; }
        [Category("Trade Capture")]
        [DisplayName("TotalQty")]
        public decimal? TotalQty { get; set; }
        [Category("Trade Capture")]
        [DisplayName("AveragePx")]
        public decimal? AveragePx { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Expiry Date")]
        public DateTime? ExpiryDate { get; set; }
        [Category("Trade Capture")]
        [DisplayName("PnL Currency")]
        public string Currency { get; set; }

        //SecurityDefinition properties
        [Category("Security Definition")]
        [DisplayName("Clearable")]
        public bool? Clearable { get; set; }
        [Category("Security Definition")]
        [DisplayName("End Date")]
        public string EndDate { get; set; }
        [Category("Security Definition")]
        [DisplayName("Security Definition Exchange")]
        public string SecurityDefinitionExchange { get; set; }
        [Category("Security Definition")]
        [DisplayName("Granularity")]
        public string Granularity { get; set; }
        [Category("Security Definition")]
        [DisplayName("Hub Alias")]
        public string HubAlias { get; set; }
        [Category("Security Definition")]
        [DisplayName("Hub Id")]
        public int? HubId { get; set; }
        [Category("Security Definition")]
        [DisplayName("Hub Name")]
        public string HubName { get; set; }
        [Category("Security Definition")]
        [DisplayName("Implied Type")]
        public string ImpliedType { get; set; }
        [Category("Security Definition")]
        [DisplayName("Increment Price")]
        public decimal? IncrementPrice { get; set; }
        [Category("Security Definition")]
        [DisplayName("Increment Qty")]
        public decimal? IncrementQty { get; set; }
        [Category("Security Definition")]
        [DisplayName("Increment Strike")]
        public decimal? IncrementStrike { get; set; }
        [Category("Security Definition")]
        [DisplayName("Lot Size")]
        public int? LotSize { get; set; }
        [Category("Security Definition")]
        [DisplayName("Lot Size Multiplier")]
        public decimal? LotSizeMultiplier { get; set; }
        [Category("Security Definition")]
        [DisplayName("Max Strike")]
        public decimal? MaxStrike { get; set; }
        [Category("Security Definition")]
        [DisplayName("Min Strike")]
        public decimal? MinStrike { get; set; }
        [Category("Security Definition")]
        [DisplayName("Number Of Decimal Price")]
        public int? NumOfDecimalPrice { get; set; }
        [Category("Security Definition")]
        [DisplayName("Number Of Decimal Qty")]
        public int? NumOfDecimalQty { get; set; }
        [Category("Security Definition")]
        [DisplayName("Price Denomination")]
        public string PriceDenomination { get; set; }
        [Category("Security Definition")]
        [DisplayName("Price Unit")]
        public string PriceUnit { get; set; }
        [Category("Security Definition")]
        [DisplayName("Primary Leg Symbol")]
        public string PrimaryLegSymbol { get; set; }
        [Category("Security Definition")]
        [DisplayName("Product Description")]
        public string ProductDescription { get; set; }
        [Category("Security Definition")]
        [DisplayName("Product Id")]
        public int? ProductId { get; set; }
        [Category("Security Definition")]
        [DisplayName("Product Name")]
        public string ProductName { get; set; }
        [Category("Security Definition")]
        [DisplayName("Secondary Leg Symbol")]
        public string SecondaryLegSymbol { get; set; }
        [Category("Security Definition")]
        [DisplayName("Security Definition Id")]
        public int SecurityDefinitionId { get; set; }
        [Category("Security Definition")]
        [DisplayName("Start Date")]
        public string StartDate { get; set; }
        [Category("Security Definition")]
        [DisplayName("Strip Id")]
        public int? StripId { get; set; }
        [Category("Security Definition")]
        [DisplayName("Strip Name")]
        public string StripName { get; set; }
        [Category("Security Definition")]
        [DisplayName("Tick Value")]
        public decimal? TickValue { get; set; }
        [Category("Security Definition")]
        [DisplayName("Underlying CFI Code")]
        public string UnderlyingCFICode { get; set; }
        [Category("Security Definition")]
        [DisplayName("Underlying Contract Multiplier")]
        public decimal? UnderlyingContractMultiplier { get; set; }
        [Category("Security Definition")]
        [DisplayName("Underlying Maturity Date")]
        public string UnderlyingMaturityDate { get; set; }
        [Category("Security Definition")]
        [DisplayName("Underlying Security Desc")]
        public string UnderlyingSecurityDesc { get; set; }
        [Category("Security Definition")]
        [DisplayName("Underlying Security Id")]
        public string UnderlyingSecurityID { get; set; }
        [Category("Security Definition")]
        [DisplayName("Underlying Security Id Source")]
        public string UnderlyingSecurityIDSource { get; set; }
        [Category("Security Definition")]
        [DisplayName("Underlying Symbol")]
        public string UnderlyingSymbol { get; set; }
        [Category("Security Definition")]
        [DisplayName("Underlying Unit Of Measure")]
        public string UnderlyingUnitOfMeasure { get; set; }
        [Category("Security Definition")]
        [DisplayName("GMI Code")]
        public string GmICode { get; set; }

        [Category("Fee")]
        [DisplayName("Exchange Fee")]
        public decimal? FeeExchange { get; set; }
        [Category("Fee")]
        [DisplayName("NFA Fee")]
        public decimal? FeeNfa { get; set; }
        [Category("Fee")]
        [DisplayName("ABN Commission")]
        public decimal? FeeCommision { get; set; }
        [Category("Fee")]
        [DisplayName("Clearing Fee")]
        public decimal? FeeClearing { get; set; }
        [Category("Fee")]
        [DisplayName("Portfolio")]
        public string Portfolio { get; set; }

        [Category("Fee")]
        [DisplayName("Block Fee")]
        public decimal? BlockFee { get; set; }

        [Category("Fee")]
        [DisplayName("Platts Fee")]
        public decimal? PlattsFee { get; set; }

        [Category("Fee")]
        [DisplayName("ICE Spread Rebate")]
        public decimal? IceSpreadRebate { get; set; }

        [Category("Trade Capture")]
        [DisplayName("Portfolio Id")]
        public int PortfolioId { get; set; }

        [Category("Fx Trades")]
        [DisplayName("Fx Trade Id")]
        public int? FxTradeId { get; set; }

        [Category("Fx Trades")]
        [DisplayName("Product Type")]
        public string ProductType { get; set; }

        [Category("Fx Trades")]
        [DisplayName("Specified Amount")]
        public decimal? SpecifiedAmount { get; set; }

        [Category("Fx Trades")]
        [DisplayName("Against Amount")]
        public decimal? AgainstAmount { get; set; }

        [Category("Fx Trades")]
        [DisplayName("Rate")]
        public decimal? Rate { get; set; }

        [Category("Fx Trades")]
        [DisplayName("Spot Rate")]
        public decimal? SpotRate { get; set; }

        [Category("Fx Trades")]
        [DisplayName("Tenor")]
        public string Tenor { get; set; }

        [Category("Fx Trades")]
        [DisplayName("Link Trade Report Id")]
        public string LinkTradeReportId { get; set; }

        [Category("Fx Trades")]
        [DisplayName("Link Type")]
        public string LinkType { get; set; }

        [Category("Fx Trades")]
        [DisplayName("Value Date")]
        public DateTime? ValueDate { get; set; }

        [Category("Fx Trades")]
        [DisplayName("Against Currency")]
        public string AgainstCurrency { get; set; }

        [Category("Fx Trades")]
        [DisplayName("Specified Currency")]
        public string SpecifiedCurrency { get; set; }
    }
}