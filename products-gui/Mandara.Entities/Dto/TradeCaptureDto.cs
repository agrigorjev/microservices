using System;

namespace Mandara.Entities.Dto
{
    public class TradeCaptureDto
    {
        //TradeCapture properties
        public TradePortfolioDto BuyBook { get; set; }
        public string CFICode { get; set; }
        public string ClOrdID { get; set; }
        public string ClearingAccountId { get; set; }
        public string ClearingFirm { get; set; }
        public string CreatedBy { get; set; }
        public string EditCancelReason { get; set; }
        public string Exchange { get; set; }
        public string ExecID { get; set; }
        public string ExecType { get; set; }
        public string ExecutingFirm { get; set; }
        public string LegRefID { get; set; }
        public int? NumOfCycles { get; set; }
        public int? NumOfLots { get; set; }
        public string OrdStatus { get; set; }
        public string OrderID { get; set; }
        public string OrigTradeID { get; set; }
        public string OriginationFirm { get; set; }
        public string OriginationTrader { get; set; }
        public int? ParentID { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? TotalQty { get; set; }
        public decimal? AveragePx { get; set; }
        public string SecurityID { get; set; }
        public string SecurityIDSource { get; set; }
        public TradePortfolioDto SellBook { get; set; }
        public string Side { get; set; }
        public string Symbol { get; set; }
        public DateTime? TimeStamp { get; set; }
        public DateTime? TradeDate { get; set; }
        public DateTime? TradeEndDate { get; set; }
        public TradeGroupDto TradeGroup { get; set; }
        public int TradeId { get; set; }
        public string TradeReportID { get; set; }
        public string TradeReportTransType { get; set; }
        public DateTime? TradeStartDate { get; set; }
        public int? TradeType { get; set; }
        public DateTime? TransactTime { get; set; }
        public DateTime UtcTransactTime { get; set; }
        public string TrdType { get; set; }
        public bool? Pending { get; set; }

        public decimal? FeeExchangeDb { get; set; }
        public decimal? FeeNfaDb { get; set; }
        public decimal? FeeCommissionDb { get; set; }
        public decimal? FeeClearingDb { get; set; }
        public decimal? FeeBlockDb { get; set; }
        public decimal? FeePlattsDb { get; set; }
        public decimal? IceSpreadRebateDb { get; set; }

        public TradePortfolioDto Portfolio { get; set; }

        public bool? IsParentTimeSpread { get; set; }
        public bool? IsParentTrade { get; set; }
        public bool? IsTransferSell { get; set; }
        
        public SecurityDefinitionDto SecurityDefinition { get; set; }

        public decimal? Brokerage { get; set; }
        //04-25-2013 EZ:Add GMI Codes
        public string GMICode { get; set; }

        public string ExchangeOverride { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }
}