using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("app_settings")]
    public partial class AppSetting
    {
        [Column("setting_id")]
        [Key]
        public int SettingId { get; set; }

        [Column("name")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Column("value")]
        [StringLength(255)]
        public string Value { get; set; }
    }
}
