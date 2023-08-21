using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("ice_balmo_mappings")]
    public partial class IceBalmoMapping : BalmoCode
    {
        private Product _product;

        [Column("ice_balmo_mapping_id")]
        [Key]
        public int IceBalmoMappingId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("prefix_char")]
        [Required]
        [StringLength(10)]
        public string PrefixChar
        {
            get
            {
                return prefixChar;
            }
            set
            {
                prefixChar = value;
            }
        }

        [Column("start_char")]
        [Required]
        [StringLength(1)]
        public string StartChar
        {
            get
            {
                return startChar;
            }
            set
            {
                startChar = value;
            }
        }

        [Column("end_char")]
        [Required]
        [StringLength(1)]
        public string EndChar
        {
            get
            {
                return endChar;
            }
            set
            {
                endChar = value;
            }
        }

        [Column("start_day")]
        public int StartDay { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product
        {
            get { return _product; }
            set
            {
                _product = value;
                ProductId = _product?.ProductId ?? 0;
            }
        }
    }
}