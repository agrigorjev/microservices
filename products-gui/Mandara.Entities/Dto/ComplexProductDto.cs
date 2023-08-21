namespace Mandara.Entities.Dto
{
    public class ComplexProductDto
    {
        public decimal? ConversionFactor1 { get; set; }
        public decimal? ConversionFactor2 { get; set; }
        public ProductDto3 ChildProduct1 { get; set; }
        public ProductDto3 ChildProduct2 { get; set; }
    }
}