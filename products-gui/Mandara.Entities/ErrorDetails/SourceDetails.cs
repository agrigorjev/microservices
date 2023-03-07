using System;
using System.ComponentModel;

namespace Mandara.Entities.ErrorDetails
{
    public class SourceDetails : TradeDetails
    {
        [Category("Source Details")]
        [DisplayName("Account Number")]
        public string AccountNumber { get; set; }
        [Category("Source Details")]
        [DisplayName("Exchange Code")]
        public string ExchangeCode { get; set; }
        [Category("Source Details")]
        [DisplayName("Instrument Description")]
        public string InstrumentDescription { get; set; }
        [Category("Source Details")]
        [DisplayName("Product Id")]
        public new int ProductId { get; set; }
        [Category("Source Details")]
        [DisplayName("Product Name")]
        public new string ProductName { get; set; }
        [Category("Source Details")]
        [DisplayName("Product Date")]
        public DateTime ProductDate { get; set; }
        [Category("Source Details")]
        [DisplayName("Quantity")]
        public decimal Quantity { get; set; }
        [Category("Source Details")]
        [DisplayName("Trade Price")]
        public decimal TradePrice { get; set; }
        [Category("Source Details")]
        [DisplayName("Date Type")]
        public string DateType { get; set; }


        public SourceDetails()
        {
        }

        public SourceDetails(SourceDetail o) : base(o.TradeCapture)
        {
            int productId = o.Product == null ? 0 : o.Product.ProductId;
            string productName = o.Product == null ? "" : o.Product.Name;

            AccountNumber = o.AccountNumber;
            ExchangeCode = o.ExchangeCode;
            InstrumentDescription = o.InstrumentDescription;
            ProductId = productId;
            ProductName = productName;
            ProductDate = o.ProductDate;
            Quantity = o.Quantity ?? 0;
            TradePrice = o.TradePrice;
            DateType = o.DateType.ToString();
        }
    }
}
