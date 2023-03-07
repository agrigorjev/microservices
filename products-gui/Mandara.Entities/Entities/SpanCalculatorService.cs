using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("span_calculator_service")]
    public partial class SpanCalculatorService
    {
        [Column("message_id")]
        [Key]
        public int MessageId { get; set; }

        [Column("message_date")]
        public DateTime? MessageDate { get; set; }

        [Column("file_name")]
        [StringLength(50)]
        public string FileName { get; set; }

        [Column("exchange")]
        public short? Exchange { get; set; }
    }
}
