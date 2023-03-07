using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("products_delivered")]
    public partial class DeliveredProduct
    {
        [Column("security_definition_id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SecurityDefinitionId { get; set; }

        [Column("delivered_at")]
        public DateTime DeliveredAt { get; set; }

        [Column("delivered_by")]
        [Required]
        [StringLength(255)]
        public string DeliveredBy { get; set; }
    }
}
