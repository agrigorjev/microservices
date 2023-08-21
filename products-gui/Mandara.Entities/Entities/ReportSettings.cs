using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("scheduled_report_settings")]
    public partial class ReportSettings
    {
        [Column("report_setting_id")]
        [Key]
        public int ReportSettingId { get; set; }

        [Column("report_enabled")]
        public bool ReportEnabled { get; set; }

        [Column("printer_name")]
        [StringLength(255)]
        public string PrinterName { get; set; }

        [Column("week_days")]
        public int? WeekDays { get; set; }

        [Column("time")]
        public DateTime? Time { get; set; }

        [Column("email")]
        [StringLength(255)]
        public string Email { get; set; }
    }
}
