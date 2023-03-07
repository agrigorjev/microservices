using System;

namespace Mandara.Entities.Dto
{
    [Serializable]
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public ProductType Type { get; set; }
        public ExchangeDto Exchange { get; set; }
        public OfficialProductDto2 OfficialProduct { get; set; }
        public bool IsTasDb { get; set; }
        public bool IsMopsDb { get; set; }
        public bool IsMmDb { get; set; }
    }
}