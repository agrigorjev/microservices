using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("pnl_reports_portfolios")]
    public class PnlReportPortfolio
    {
        private PnlReport _pnlReport;
        private Portfolio _portfolio;

        [Column("reports_portfolios_id")]
        [Key]
        public int ReportsPortfoliosId { get; set; }

        [Column("reports_id")]
        public int ReportId { get; set; }

        [Column("portfolio_id")]
        public int PortfolioId { get; set; }

        [Column("total_pnl", TypeName = "numeric")]
        public decimal? TotalPnl { get; set; }

        [Column("live_pnl", TypeName = "numeric")]
        public decimal? LivePnl { get; set; }

        [Column("overnight_pnl", TypeName = "numeric")]
        public decimal? OvernightPnl { get; set; }

        [Column("live_costs")]
        public decimal? LiveCosts { get; set; }

        [ForeignKey("ReportId")]
        public virtual PnlReport PnlReport
        {
            get { return _pnlReport; }
            set { _pnlReport = value; }
        }

        [ForeignKey("PortfolioId")]
        public virtual Portfolio Portfolio
        {
            get { return _portfolio; }
            set
            {
                _portfolio = value;
                PortfolioId = _portfolio != null ? _portfolio.PortfolioId : 0;
            }
        }

        public override string ToString()
        {
            return string.Format("PortfolioId: {0}, TotalPnl: {1:F2}, LivePnl:{2:F2}, OvernightPnl: {3:F2}, LiveCosts: {4:F2}", PortfolioId, TotalPnl, LivePnl, OvernightPnl, LiveCosts);
        }
 
    }
}
