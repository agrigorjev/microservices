using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Mandara.TradeApiService.Data
{
    [Table("portfolios")]
    public partial class Portfolio
    {
        public Portfolio()
        {
            Portfolios = new HashSet<Portfolio>();
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
        public string? UpdatedBy { get; set; }

        [Column("is_archived")]
        public bool? IsArchivedDb { get; set; }

        [ForeignKey("ParentId")]
        public virtual Portfolio? ParentPortfolio
        {
            get { return _parentPortfolio; }
            set
            {
                _parentPortfolio = value;
                ParentId = _parentPortfolio != null ? _parentPortfolio.PortfolioId : (int?)null;
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
        public virtual ICollection<User> Users { get; set; }


        public virtual ICollection<Portfolio> Portfolios { get; set; }

        [NotMapped]
        public bool IsArchived
        {
            get { return IsArchivedDb ?? false; }
            set { IsArchivedDb = value; }
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
