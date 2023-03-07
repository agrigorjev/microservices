using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities.Entities
{
    [Table("portfolio_clearing_accounts")]
    public partial class PortfolioClearingAccount
    {
        public PortfolioClearingAccount()
        {
            PortfolioId = NoPortfolio;
            Name = NoName;
            Code = NoCode;
        }

        private int _portfolioId = NoPortfolio;

        [Column("portfolio_id")]
        [Key]
        [ForeignKey("AccountPortfolio")]
        public int PortfolioId
        {
            get => AccountPortfolio?.PortfolioId ?? _portfolioId;
            set => _portfolioId = value;
        }

        [Column("name")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Column("code")]
        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        public virtual Portfolio AccountPortfolio { get; set; }

        private const int NoPortfolio = -1;
        private const string NoName = "NoAccount";
        private const string NoCode = "NoCode";
        private const Portfolio NoAccountPortfolio = null;

        public static readonly PortfolioClearingAccount Default = new PortfolioClearingAccount()
        {
            PortfolioId = NoPortfolio,
            Name = NoName,
            Code = NoCode,
            AccountPortfolio = NoAccountPortfolio,
        };

        public bool IsDefault()
        {
            return NoPortfolio == PortfolioId
                   && NoName == Name
                   && NoCode == Code
                   && NoAccountPortfolio == AccountPortfolio;
        }

        public override bool Equals(object obj)
        {
            if (obj is PortfolioClearingAccount otherPortfolioAcc)
            {
                return PortfolioId.Equals(otherPortfolioAcc.PortfolioId);
            }

            return false;
        }

        public bool Equals(PortfolioClearingAccount otherPortfolioAcc)
        {
            return PortfolioId == otherPortfolioAcc.PortfolioId
                   && Name.Equals(otherPortfolioAcc.Name)
                   && Code.Equals(otherPortfolioAcc.Code);
        }

        public override int GetHashCode()
        {
            return PortfolioId.GetHashCode() ^ (2083 * Name.GetHashCode()) ^ (419 * Code.GetHashCode());
        }

        public override string ToString()
        {
            return $"{PortfolioId} - {Name} - {Code}";
        }

    }
}
