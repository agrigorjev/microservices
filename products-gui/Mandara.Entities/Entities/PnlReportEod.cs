using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("pnl_report_eod")]
    public class PnlReportEod
    {
        [Key]
        [Column("day", TypeName = "date")]
        public DateTime Day { get; set; }

        [Column("eod")]
        public DateTime EndOfDayDb { get; set; }


        [NotMapped]
        public TimeSpan EndOfDay
        {
            get { return EndOfDayDb.TimeOfDay; }
            set { EndOfDayDb = Day.Date.Add(value); }
        }

        [NotMapped]
        public DateTime SnapshotDatetime { get { return Day.Date.Add(EndOfDayDb.TimeOfDay); } }

    }
}
