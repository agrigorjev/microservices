using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandara.Entities.Entities
{
    [Table("platts_symbol_config")]
    public class PlattsSymbolConfig
    {
        [ForeignKey("OfficialProduct")]
        [Key]
        [Column("official_product_id")]
        public int OfficialProductId { get; set; }

        [Column("platts_symbol")]
        [Required]
        [StringLength(100)]
        public string PlattsSymbol { get; set; }

        [Column("mul")]
        [Required]
        public decimal Mul { get; set; }

        [Column("div")]
        [Required]
        public decimal Div { get; set; }

        [Column("order")]
        [Required]
        public int Order { get; set; }

        public virtual OfficialProduct OfficialProduct { get; set; }

        public static PlattsSymbolConfig CreateDefaultValue(OfficialProduct product)
        {
            return new PlattsSymbolConfig()
            {
                Div = 1,
                Mul = 1,
                OfficialProductId = product.OfficialProductId,
                Order = 100,
                PlattsSymbol = ""
            };
        }

        public void Update(PlattsSymbolConfig newConfig)
        {
            PlattsSymbol = newConfig.PlattsSymbol;
            Order = newConfig.Order;
            Mul = newConfig.Mul;
            Div = newConfig.Div;
        }
    }
}
