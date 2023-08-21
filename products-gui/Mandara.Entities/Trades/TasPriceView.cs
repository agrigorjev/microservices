using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;


namespace Mandara.Entities.Trades
{
    public partial class TasPriceView
    {
        [Category("Trade Capture")]
        [DisplayName("TradeId")]
        public int TradeId { get; set; }	
        [Category("Trade Capture")]
        [DisplayName("Side")]
        public string Side { get; set; }	
        [Category("Security Definition")]
        [DisplayName("Product Description")]
        public string ProductDescription { get; set; }
		[Category("Security Definition")]
        [DisplayName("Strip Name")]
        public string StripName { get; set; }	
	    [Category("Trade Capture")]
        [DisplayName("Price")]
        public decimal? Price { get; set; }
		[Category("Trade Capture")]
        [DisplayName("Quantity")]
        public decimal? Quantity { get; set; }
		[Category("Trade Capture")]
        [DisplayName("Timestamp")]
        public DateTime? TimeStamp { get; set; }
		[Category("Trade Capture")]
        [DisplayName("Origination Trader")]
        public string OriginationTrader { get; set; }
		[Category("Trade Capture")]
        [DisplayName("Orig Trade Id")]
        public string OrigTradeID { get; set; }
		[Category("Trade Capture")]
        [DisplayName("Order Id")]
        public string OrderID { get; set; }
        [Category("Trade Capture")]
        [DisplayName("Clearing Account Id")]
        public string ClearingAccountId { get; set; }
		[Category("Trade Capture")]
        [DisplayName("Exchange")]
        public string Exchange { get; set; }
    }
}
