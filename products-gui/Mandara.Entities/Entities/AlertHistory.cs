using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("alerts_history")]
    public class AlertHistory
    {
        [Column("alert_history_id")]
        [Key]
        public int AlertHistoryId { get; set; }

        [Column("triggered_at")]
        public DateTime TriggeredAt { get; set; }

        [Column("alert_level")]
        public int Level { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("acknowledged_by")]
        [StringLength(100)]
        public string AcknowledgedBy { get; set; }

        [Column("acknowledged_ip")]
        [StringLength(50)]
        public string AcknowledgedIp { get; set; }

        [Column("actual_value")]
        public string ActualValue { get; set; }

        [Column("alert_id")]
        public int AlertId { get; set; }

        [Column("subject")]
        [StringLength(255)]
        public string Subject { get; set; }

        [Column("message")]
        public string Message { get; set; }

        [Column("threshold_value")]
        [StringLength(255)]
        public string ThresholdValue { get; set; }

        [Column("alert_type")]
        [StringLength(255)]
        public string AlertType { get; set; }

        [Column("book_name")]
        [StringLength(255)]
        public string BookName { get; set; }

        [Column("trigger_key")]
        [StringLength(255)]
        public string TriggerKey { get; set; }

        [Column("serialized_value")]
        public string SerializedValue { get; set; }


        [NotMapped]
        public bool IsAcknowledged { get { return !string.IsNullOrEmpty(AcknowledgedIp); } }

    }
}
