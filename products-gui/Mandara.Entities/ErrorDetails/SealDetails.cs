using System;
using System.ComponentModel;

namespace Mandara.Entities.ErrorDetails
{
    public class SealDetails : ErrorDetails
    {
            [Category("Source Details")]
            [DisplayName("BusinessStateName")]
            public string BusinessStateName{get;set;}
            [Category("Source Details")]
            [DisplayName("TradeId")]
            public string TradeId{get;set;}
            [Category("Source Details")]
            [DisplayName("ExchangeId")]
            public string ExchangeId{get;set;}
            [Category("Source Details")]
            [DisplayName("Volume")]
            public string Volume{get;set;}
            [Category("Source Details")]
            [DisplayName("Market_Expiry")]	
            public string Market_Expiry{get;set;}
            [Category("Source Details")]
            [DisplayName("Market_Product")]
            public string Market_Product{get;set;}
            [Category("Source Details")]
            [DisplayName("ProductName")]
            public string ProductName{get;set;}
            [Category("Source Details")]
            [DisplayName("BuySell")]	
            public string BuySell{get;set;}
            [Category("Source Details")]
            [DisplayName("OptionType")]	
            public string OptionType{get;set;}
            [Category("Source Details")]
            [DisplayName("Strike")]
            public string Strike{get;set;}
            [Category("Source Details")]	
            [DisplayName("Price")]
            public string Price{get;set;}
            [Category("Source Details")]	
            [DisplayName("Expr1")]
            public string Expr1{get;set;}
            [Category("Source Details")]	
            [DisplayName("Reference1")]	
            public string Reference1{get;set;}
            [Category("Source Details")]
            [DisplayName("ToMemberId")]	
            public string ToMemberId{get;set;}
            [Category("Source Details")]
            [DisplayName("ToMember")]
            public string ToMember{get;set;}
            [Category("Source Details")]
            [DisplayName("ExchangeOrderNumber")]
            public string ExchangeOrderNumber{get;set;}
            [Category("Source Details")]
            [DisplayName("ExchangeTradeNumber")]
            public string ExchangeTradeNumber{get;set;}
            [Category("Source Details")]
            [DisplayName("Reference2")]
            public string Reference2{get;set;}
            [Category("Source Details")]
            [DisplayName("Reference3")]
            public string Reference3{get;set;}
            [Category("Source Details")]
            [DisplayName("GiveupRef1")]
            public string GiveupRef1{get;set;}
            [Category("Source Details")]	
            [DisplayName("GiveupRef2")]	
            public string GiveupRef2{get;set;}
            [Category("Source Details")]
            [DisplayName("GiveupRef3")]	
            public string GiveupRef3{get;set;}
            [Category("Source Details")]
            [DisplayName("Trader")]	
            public string Trader{get;set;}
            [Category("Source Details")]
            [DisplayName("AllocationId")]
            public string AllocationId{get;set;}
            [Category("Source Details")]	
            [DisplayName("ExchangeMember")]	
            public string ExchangeMember{get;set;}
            [Category("Source Details")]
            [DisplayName("TradingDay")]
            public string TradingDay{get;set;}
            [Category("Source Details")]
            [DisplayName("ClearingDay")]
            public string ClearingDay{get;set;}
            [Category("Source Details")]
            [DisplayName("CounterPartyMember")]
            public string CounterPartyMember{get;set;}
            [Category("Source Details")]
            [DisplayName("OpenClose")]
            public string OpenClose{get;set;}
            [Category("Source Details")]
            [DisplayName("PositionAccount")]
            public string PositionAccount{get;set;}
            [Category("Source Details")]	
            [DisplayName("BOAccount")]
            public string BOAccount{get;set;}
            [Category("Source Details")]
            [DisplayName("BOReference")]
            public string BOReference{get;set;}
            [Category("Source Details")]
            [DisplayName("OriginTransferType")]
            public string OriginTransferType{get;set;}
            [Category("Source Details")]
            [DisplayName("OriginFromMember")]	
            public string OriginFromMember{get;set;}
            [Category("Source Details")]
            [DisplayName("TradingTradeType")]
            public string TradingTradeType{get;set;}
            [Category("Source Details")]	
            [DisplayName("TradePriceType")]	
            public string TradePriceType{get;set;}
            [Category("Source Details")]	
            [DisplayName("Venue")]
            public string Venue{get;set;}
            [Category("Source Details")]
            [DisplayName("ComboType")]
            public string ComboType{get;set;}
            [Category("Source Details")]
            [DisplayName("OriginalClearingDay")]
            public string OriginalClearingDay{get;set;}
            [Category("Source Details")]
            [DisplayName("Note")]
            public string Note{get;set;}
            [Category("Source Details")]
            [DisplayName("TradeGroupId")]
            public string TradeGroupId{get;set;}
            [Category("Source Details")]
            [DisplayName("ClientId")]	
            public string ClientId{get;set;}
            [Category("Source Details")]
            [DisplayName("ClientName")]
            public string ClientName{get;set;}
            


        public SealDetails()
        {
        }

        public SealDetails(SourceDetail o)
         {
            string productName = o.Product == null ? "" : o.Product.Name;
            ProductName = productName;
            BusinessStateName = o.BusinessStateName;
            TradeId = o.TradeId;
            ExchangeId = o.ExchangeId;
            Volume = o.Volume;
            Market_Expiry = o.Market_Expiry;
            Market_Product = o.Market_Product;
            BuySell = o.BuySell;
            OptionType = o.OptionType;
            Strike = o.Strike;
            Price = o.Price;
            Expr1 = o.Expr1;
            Reference1 = o.Reference1;
            ToMemberId = o.ToMemberId;
            ToMember = o.ToMember;
            ExchangeOrderNumber = o.ExchangeOrderNumber;
            ExchangeTradeNumber = o.ExchangeTradeNumber;
            Reference2 = o.Reference2;
            Reference3 = o.Reference3;
            GiveupRef1 = o.GiveupRef1;
            GiveupRef2 = o.GiveupRef2;
            GiveupRef3 = o.GiveupRef3;
            Trader = o.Trader;
            AllocationId = o.AllocationId;
            ExchangeMember = o.ExchangeMember;
            TradingDay = o.TradingDay;
            ClearingDay = o.ClearingDay;
            CounterPartyMember = o.CounterPartyMember;
            OpenClose = o.OpenClose;
            PositionAccount = o.PositionAccount;
            BOAccount = o.BOAccount;
            BOReference = o.BOReference;
            OriginTransferType = o.OriginTransferType;
            OriginFromMember = o.OriginFromMember;
            TradingTradeType = o.TradingTradeType;
            TradePriceType = o.TradePriceType;
            Venue = o.Venue;
            ComboType = o.ComboType;
            OriginalClearingDay = o.OriginalClearingDay;
            Note = o.Note;
            TradeGroupId = o.TradeGroupId;
            ClientId = o.ClientId;
            ClientName = o.ClientName;

        }
    }
}
