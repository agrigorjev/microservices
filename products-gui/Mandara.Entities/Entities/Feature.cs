using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("features")]
    public partial class Feature
    {
        [Column("feature_id")]
        [Key]
        public int FeatureId { get; set; }

        [Column("name")]
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Column("is_enabled")]
        public bool IsEnabled { get; set; }
    }
}
