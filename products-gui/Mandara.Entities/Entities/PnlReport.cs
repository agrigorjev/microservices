using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mandara.Date;

namespace Mandara.Entities
{
    [Table("pnl_reports")]
    public class PnlReport : IComparer<PnlReport>
    {
        public PnlReport()
        {
            PnlReportsPortfolios = new HashSet<PnlReportPortfolio>();
        }

        [Column("report_id")]
        [Key]
        public int ReportId { get; set; }

        [Column("report_date")]
        public DateTime ReportDate { get; set; }

        public virtual ICollection<PnlReportPortfolio> PnlReportsPortfolios { get; set; }


        public int Compare(PnlReport x, PnlReport y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                return -1;
            }
            if (y == null)
            {
                return 1;
            }
            int retval = y.ReportDate.CompareTo(x.ReportDate);
            if (retval != 0)
            {
                return retval;
            }
            return y.ReportDate.CompareTo(x.ReportDate);
        }

        public override string ToString()
        {
            return
                $"PnL Date: {ReportDate.ToDayFirstDateAndTime('.', ' ', ':')}";
        }
    }
}
