using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.TradeApiService.Data
{
    [Table("parser_default_products")]
    public class ParserDefaultProduct
    {
        private OfficialProduct _officialProduct;

        [Column("broker_id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BrokerId { get; set; }

        [Column("official_product_id")]
        public int? OfficialProductId { get; set; }

        [ForeignKey("OfficialProductId")]
        public virtual OfficialProduct OfficialProduct
        {
            get { return _officialProduct; }
            set
            {
                _officialProduct = value;
                OfficialProductId = _officialProduct != null ? _officialProduct.OfficialProductId : (int?) null;
            }
        }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Unit return false.
            var p = obj as ParserDefaultProduct;
            if ((System.Object)p == null)
            {
                return false;
            }

            return BrokerId == p.BrokerId;
        }

        public override int GetHashCode()
        {
            return BrokerId.GetHashCode();
        }

    }
}
