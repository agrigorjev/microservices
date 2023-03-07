using System;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.ProductBreakdown
{
    public class ProductBreakdownRequestMessage : MessageBase
    {
        public int ProductId { get; set; }
        public DateTime ContractMonth { get; set; }
        public DateTime PurchaseDay { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime? StartDay { get; set; }
        public DateTime? EndDay { get; set; }
    }
}