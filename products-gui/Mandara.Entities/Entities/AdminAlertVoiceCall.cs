using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("adm_alert_voice_calls")]
    public class AdminAlertVoiceCall
    {
        [Column("call_id")]
        [Key]
        public int CallId { get; set; }

        [Column("trigger_key")]
        [StringLength(255)]
        public string TriggerKey { get; set; }

        [Column("alert_history_id")]
        public int AlertHistoryId { get; set; }

        [Column("call_phone")]
        [Required]
        [StringLength(50)]
        public string Phone { get; set; }

        [Column("call_status")]
        public int StatusDb { get; set; }

        [Column("message")]
        [Required]
        [StringLength(500)]
        public string Message { get; set; }

        [Column("retry_num")]
        public int? RetryNum { get; set; }

        [Column("call_result")]
        public int? ResultDb { get; set; }

        [Column("last_dialed")]
        public DateTime? DialedAt { get; set; }

        [Column("actual_value")]
        public string ActualValue { get; set; }


        /// <summary>
        /// Enumeration for result of call
        /// </summary>
        public enum CallResult
        {
            NaN,
            Success,
            Fail,
        }

        /// <summary>
        /// Enumeration for status of call
        /// </summary>
        public enum CallState
        {
            Queued,
            /// <summary>
            /// Alert was acknoledged during call processing
            /// </summary>
            Interrupted,
            InProgress,
            InRetry,
            Served
        }


        [NotMapped]
        public CallState Status
        {
            get
            {
                return (CallState)this.StatusDb;
            }
            set
            {
                this.StatusDb = (int)value;
            }
        }

        [NotMapped]
        public CallResult Result
        {
            get
            {
                return (CallResult)this.ResultDb;
            }
            set
            {
                this.ResultDb = (int)value;
            }
        }

    }
}
