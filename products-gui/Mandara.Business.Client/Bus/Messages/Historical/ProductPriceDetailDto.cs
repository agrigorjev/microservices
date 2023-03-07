using System;

namespace Mandara.Business.Bus.Messages.Historical
{
    public class ProductPriceDetailDto
    {
        public OfficialProductDto OfficialProduct { get; set; }

        public DateTime Date { get; set; }

        public decimal Price { get; set; }
    }
}