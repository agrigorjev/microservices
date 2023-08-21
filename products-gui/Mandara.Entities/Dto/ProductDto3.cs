using System;

namespace Mandara.Entities.Dto
{
    public class ProductDto3
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal ContractSize { get; set; }
        public decimal? PnlFactor { get; set; }
        public decimal? PositionFactor { get; set; }
        public CalendarDto ExpiryCalendar { get; set; }
        public CalendarDto HolidaysCalendar { get; set; }
        public short ProductTypeDb { get; set; }
        public bool? UseExpiryCalendar { get; set; }
        public bool? UseRolloffSettings { get; set; }
        public DateTime? RolloffTime { get; set; }
        public ComplexProductDto ComplexProduct { get; set; }
        public ProductType Type { get; set; }
        public ProductDto3 BalmoOnComplexProduct { get; set; }
        public ProductDto3 BalmoOnCrudeProduct { get; set; }
        public ProductCategoryDto Category { get; set; }
        public OfficialProductDto2 OfficialProduct { get; set; }
        public string TimezoneId { get; set; }
    }
}