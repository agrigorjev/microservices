using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Mandara.Entities
{
    [Table("portfolios")]
    public partial class Portfolio
    {
        public Portfolio()
        {
            PortfolioTrades = new HashSet<PortfolioTrade>();
            Portfolios = new HashSet<Portfolio>();
            Users = new HashSet<User>();
            UserPortfolioPermissions = new HashSet<UserPortfolioPermission>();
            trade_captures = new HashSet<TradeCapture>();
            trade_captures1 = new HashSet<TradeCapture>();
            trade_captures2 = new HashSet<TradeCapture>();
            pnl_reports_portfolios = new HashSet<PnlReportPortfolio>();
            adm_alerts = new HashSet<AdministrativeAlert>();
            UserProductGroups = new HashSet<UserProductCategoryPortfolio>();
        }

        [Column("portfolio_id")]
        [Key]
        public int PortfolioId { get; set; }

        [Column("name")]
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Column("parent_id")]
        public int? ParentId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("created_by")]
        [Required]
        [StringLength(255)]
        public string CreatedBy { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("updated_by")]
        [StringLength(255)]
        public string UpdatedBy { get; set; }

        [Column("is_archived")]
        public bool? IsArchivedDb { get; set; }

        [ForeignKey("ParentId")]
        public virtual Portfolio ParentPortfolio
        {
            get { return _parentPortfolio; }
            set
            {
                _parentPortfolio = value;
                ParentId = _parentPortfolio != null ? _parentPortfolio.PortfolioId : (int?) null;
            }
        }

        public const int NoPortfolio = -1;
        private const string NoName  = "NoPortfolio";
        private const int NoParentId  = -1;
        private static readonly DateTime NoCreatedAt  = DateTime.MinValue;
        private const string NoCreatedBy  = "NoOne";
        private static readonly DateTime NoUpdatedAt  = DateTime.MinValue;
        private const string NoUpdatedBy  = "NoOne";
        private const bool NoIsArchivedDb  = false;
        private const Portfolio NoParentPortfolio = null;

        public static readonly Portfolio Default = new Portfolio()
        {
            PortfolioId = NoPortfolio,
            Name = NoName,
            ParentPortfolio = NoParentPortfolio,
            ParentId = NoParentId,
            CreatedAt = NoCreatedAt,
            CreatedBy = NoCreatedBy,
            UpdatedAt = NoUpdatedAt,
            UpdatedBy = NoUpdatedBy,
            IsArchived = NoIsArchivedDb,
        };

        public bool IsDefault()
        {
            return NoPortfolio == PortfolioId
                   && NoName == Name
                   && NoParentId == ParentId
                   && NoCreatedAt == CreatedAt
                   && NoCreatedBy == CreatedBy
                   && NoUpdatedAt == UpdatedAt
                   && NoUpdatedBy == UpdatedBy
                   && NoIsArchivedDb == IsArchivedDb
                   && NoParentPortfolio == ParentPortfolio;
        }

        public virtual ICollection<PortfolioTrade> PortfolioTrades { get; set; }
        public virtual ICollection<Portfolio> Portfolios { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<UserPortfolioPermission> UserPortfolioPermissions { get; set; }
        public virtual ICollection<TradeCapture> trade_captures { get; set; }
        public virtual ICollection<TradeCapture> trade_captures1 { get; set; }
        public virtual ICollection<TradeCapture> trade_captures2 { get; set; }
        public virtual ICollection<PnlReportPortfolio> pnl_reports_portfolios { get; set; }
        public virtual ICollection<AdministrativeAlert> adm_alerts { get; set; }
        public virtual ICollection<UserProductCategoryPortfolio> UserProductGroups { get; set; }

        [NotMapped]
        public bool IsArchived
        {
            get { return IsArchivedDb ?? false; }
            set { IsArchivedDb = value; }
        }

        private int _parentPortfolioId;

        [NotMapped]
        public int ParentPortfolioId
        {
            get
            {
                if (ParentPortfolio != null)
                    return ParentPortfolio.PortfolioId;

                return _parentPortfolioId;
            }

            set { _parentPortfolioId = value; }
        }

        private bool _isErrorBook = false;
        private Portfolio _parentPortfolio;

        [NotMapped]
        public bool IsErrorBook
        {
            get { return _isErrorBook; }
            set { _isErrorBook = value; }
        }

        public List<int> GetHierarchyPortfolioIds()
        {
            IEnumerable<int> portfolioIds = GetHierarchyPortfolioIds(this);

            return portfolioIds.ToList();
        }

        private IEnumerable<int> GetHierarchyPortfolioIds(Portfolio portfolio)
        {
            return portfolio.Portfolios.SelectMany(p => GetHierarchyPortfolioIds(p)).Concat(new[] { portfolio.PortfolioId });
        }

        public override bool Equals(object obj)
        {
            var portfolio = obj as Portfolio;

            if (portfolio != null)
                return PortfolioId.Equals(portfolio.PortfolioId);

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return PortfolioId.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
