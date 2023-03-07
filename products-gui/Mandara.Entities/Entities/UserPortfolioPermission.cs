using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Mandara.Entities
{
    [Table("user_portfolio_permissions")]
    public partial class UserPortfolioPermission
    {
        private Portfolio _portfolio;
        private User _user;

        [Key]
        [Column("user_id", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        [Key]
        [Column("portfolio_id", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PortfolioId { get; set; }

        [Column("can_view_risk")]
        public bool? CanViewRisk { get; set; }

        [Column("can_view_pnl")]
        public bool? CanViewPnl { get; set; }

        [Column("can_add_edit_trades")]
        public bool? CanAddEditTrades { get; set; }

        [Column("can_use_master_tool")]
        public bool? CanUseMasterTool { get; set; }

        [Column("can_add_edit_books")]
        public bool? CanAddEditBooks { get; set; }

        [Column("can_edit_brokerage")]
        public bool? CanEditBrokerage { get; set; }

        [ForeignKey("PortfolioId")]
        public virtual Portfolio Portfolio
        {
            get { return _portfolio; }
            set
            {
                _portfolio = value;
                if (_portfolio != null)
                    PortfolioId = _portfolio.PortfolioId;
            }
        }

        [ForeignKey("UserId")]
        public virtual User User
        {
            get { return _user; }
            set
            {
                _user = value;
                if (_user != null)
                    UserId = _user.UserId;
            }
        }

        [NotMapped]
        public bool CanViewRiskValue
        {
            get { return CanViewRisk.HasValue && CanViewRisk.Value; }
        }

        [NotMapped]
        public bool CanViewPnlValue
        {
            get { return CanViewPnl.HasValue && CanViewPnl.Value; }
        }

        [NotMapped]
        public bool CanAddEditTradesValue
        {
            get { return CanAddEditTrades.HasValue && CanAddEditTrades.Value; }
        }

        [NotMapped]
        public bool CanUseMasterToolValue
        {
            get { return CanUseMasterTool.HasValue && CanUseMasterTool.Value; }
        }

        [NotMapped]
        public bool CanAddEditBooksValue
        {
            get { return CanAddEditBooks.HasValue && CanAddEditBooks.Value; }
        }

        [NotMapped]
        public bool CanAmendBrokerageValue
        {
            get { return CanEditBrokerage.HasValue && CanEditBrokerage.Value; }
        }

        public bool CheckWithParents(User user, Func<UserPortfolioPermission, bool> selector)
        {
            return selector(this) || CheckParent(user, selector);
        }

        private bool CheckParent(User user, Func<UserPortfolioPermission, bool> selector)
        {
            if (Portfolio.ParentPortfolio == null)
                return false;

            var parentPermission =
                user.PortfolioPermissions.SingleOrDefault(p => p.PortfolioId == Portfolio.ParentPortfolioId);

            return parentPermission != null && parentPermission.CheckWithParents(user, selector);
        }

        public override string ToString()
        {
            string portfolioName = string.Empty;

            if (Portfolio != null)
                portfolioName = Portfolio.Name;

            if (string.IsNullOrEmpty(portfolioName))
            {
                return string.Format("Portfolio Id: {0}", PortfolioId);
            }
            else
            {
                return string.Format("Portfolio: {0}", portfolioName);
            }
        }

    }
}
