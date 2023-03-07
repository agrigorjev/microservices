using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("complex_products")]
    public class ComplexProduct
    {
        private Product _childProduct1;
        private Product _childProduct2;

        [Column("product_id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }

        [Column("child_product1_id")]
        public int? ChildProduct1_Id { get; set; }

        [Column("child_product2_id")]
        public int? ChildProduct2_Id { get; set; }

        [Column("conversion_factor1")]
        public decimal? ConversionFactor1 { get; set; }

        [Column("conversion_factor2")]
        public decimal? ConversionFactor2 { get; set; }

        [Column("pnl_factor1")]
        public double? PnlFactor1 { get; set; }

        [Column("pnl_factor2")]
        public double? PnlFactor2 { get; set; }

        [Column("pricing_type")]
        public short? PricingTypeDb { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [ForeignKey("ChildProduct1_Id")]
        public virtual Product ChildProduct1
        {
            get { return _childProduct1; }
            set
            {
                _childProduct1 = value;
                ChildProduct1_Id = _childProduct1 != null ? _childProduct1.ProductId : (int?)null;
            }
        }

        [ForeignKey("ChildProduct2_Id")]
        public virtual Product ChildProduct2
        {
            get { return _childProduct2; }
            set
            {
                _childProduct2 = value;
                ChildProduct2_Id = _childProduct2 != null ? _childProduct2.ProductId : (int?) null;
            }
        }

        [NotMapped]
        public bool IsCommonPricing
        {
            get
            {
                if (PricingTypeDb.HasValue)
                    return (PricingType)PricingTypeDb == PricingType.Standard;

                return false;
            }
            set
            {
                if (value)
                {
                    PricingTypeDb = (short?)PricingType.Standard;
                }
                else
                {
                    PricingTypeDb = (short?)PricingType.NonStandard;
                }
            }
        }

        [NotMapped]
        public PricingType PricingType
        {
            get
            {
                if (PricingTypeDb.HasValue)
                    return (PricingType)PricingTypeDb;

                return PricingType.Standard;
            }
            set { PricingTypeDb = (short?)value; }
        }

        [NotMapped]
        public string PricingTypeDisplay
        {
            get
            {
                switch (PricingType)
                {
                    case PricingType.NonStandard:
                        return "Non standard";
                    default:
                        return "Standard";
                }
            }
        }

        public override string ToString()
        {
            return string.Format("Product: {0}, Leg 1 product: {1}, Leg 2 product: {2}.",
                                 Product != null ? Product.Name : "", ChildProduct1 != null ? ChildProduct1.Name : "",
                                 ChildProduct2 != null ? ChildProduct2.Name : "");
        }

    }
}
