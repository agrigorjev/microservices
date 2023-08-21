using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("source_data_messages")]
    public partial class SourceDataMessage
    {
        [Column("message_uid")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UID { get; set; }

        [Column("message_date")]
        public DateTime MessageDate { get; set; }

        [Column("processed_date")]
        public DateTime ProcessedDate { get; set; }

        [Column("filename")]
        [Required]
        [StringLength(50)]
        public string FileName { get; set; }
    }
}
