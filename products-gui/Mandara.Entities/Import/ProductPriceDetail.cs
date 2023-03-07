using System;

namespace Mandara.Entities.Import
{
    public class ProductPriceDetail
    {
        public OfficialProduct OfficialProduct { get; set; }

        public DateTime Date { get; set; }

        public decimal Price { get; set; }
    }
}