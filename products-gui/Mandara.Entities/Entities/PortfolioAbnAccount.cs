using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities.Entities
{
    [Table("portfolio_abn_accounts")]
    public partial class PortfolioAbnAccount
    {
        public PortfolioAbnAccount()
        {
            PortfolioId = NoPortfolio;
            Code = NoCode;
        }

        private int _portfolioId = NoPortfolio;

        [Column("portfolio_id")]
        [Key]
        [ForeignKey("AbnPortfolio")]
        public int PortfolioId
        {
            get => AbnPortfolio?.PortfolioId ?? _portfolioId;
            set => _portfolioId = value;
        }

        [Column("code")]
        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        public virtual Portfolio AbnPortfolio { get; set; }

        private const int NoPortfolio = -1;
        private const string NoCode = "NoCode";
        private const Portfolio NoAccountPortfolio = null;

        public static readonly PortfolioAbnAccount Default = new PortfolioAbnAccount()
        {
            PortfolioId = NoPortfolio,
            Code = NoCode,
            AbnPortfolio = NoAccountPortfolio,
        };

        public bool IsDefault()
        {
            return NoPortfolio == PortfolioId
                   && NoCode == Code
                   && NoAccountPortfolio == AbnPortfolio;
        }

        public override bool Equals(object obj)
        {
            if (obj is PortfolioAbnAccount otherPortfolioAcc)
            {
                return PortfolioId.Equals(otherPortfolioAcc.PortfolioId);
            }

            return false;
        }

        public bool Equals(PortfolioAbnAccount otherPortfolioAcc)
        {
            return PortfolioId == otherPortfolioAcc.PortfolioId && Code.Equals(otherPortfolioAcc.Code);
        }

        public override int GetHashCode()
        {
            return PortfolioId.GetHashCode() ^ (3833 * Code.GetHashCode());
        }

        public override string ToString()
        {
            return $"{PortfolioId} - {Code}";
        }

    }
}
