using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.TradeApiService.Data;

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

}
