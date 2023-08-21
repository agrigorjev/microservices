using System;

namespace Mandara.Business.Bus.Messages.Historical
{
    public class SourceDetailDto
    {
        public int SourceDetailId { get; set; }
        public decimal TradePrice { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public System.DateTime ProductDate { get; set; }
        public string InstrumentDescription { get; set; }
        public string AccountNumber { get; set; }
        public string ExchangeCode { get; set; }
        public string OrderNumber { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public Nullable<decimal> FeeExchange { get; set; }
        public Nullable<decimal> FeeNfa { get; set; }
        public Nullable<decimal> FeeClearing { get; set; }
        public Nullable<decimal> FeeCommission { get; set; }
        //Seals Extension according IRM-237
        public string BusinessStateName { get; set; }
        public string TradeId { get; set; }
        public string ExchangeId { get; set; }
        public string Volume { get; set; }
        public string Market_Expiry { get; set; }
        public string Market_Product { get; set; }
        public string BuySell { get; set; }
        public string OptionType { get; set; }
        public string Strike { get; set; }
        public string Price { get; set; }
        public string Expr1 { get; set; }
        public string Reference1 { get; set; }
        public string ToMemberId { get; set; }
        public string ToMember { get; set; }
        public string ExchangeOrderNumber { get; set; }
        public string ExchangeTradeNumber { get; set; }
        public string Reference2 { get; set; }
        public string Reference3 { get; set; }
        public string GiveupRef1 { get; set; }
        public string GiveupRef2 { get; set; }
        public string GiveupRef3 { get; set; }
        public string Trader { get; set; }
        public string AllocationId { get; set; }
        public string ExchangeMember { get; set; }
        public string TradingDay { get; set; }
        public string ClearingDay { get; set; }
        public string CounterPartyMember { get; set; }
        public string OpenClose { get; set; }
        public string PositionAccount { get; set; }
        public string BOAccount { get; set; }
        public string BOReference { get; set; }
        public string OriginTransferType { get; set; }
        public string OriginFromMember { get; set; }
        public string TradingTradeType { get; set; }
        public string TradePriceType { get; set; }
        public string Venue { get; set; }
        public string ComboType { get; set; }
        public string OriginalClearingDay { get; set; }
        public string Note { get; set; }
        public string TradeGroupId { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }

    }
}