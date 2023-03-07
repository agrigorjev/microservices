using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("category_user_portfolio")]
    public partial class UserProductCategoryPortfolio
    {
        private Portfolio _portfolio;
        private ProductCategory _productCategory;
        private User _user;

        [Key]
        [Column("user_id", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int user_id { get; set; }

        [Key]
        [Column("category_id", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int category_id { get; set; }

        [Column("portfolio_id")]
        public int portfolio_id { get; set; }

        [ForeignKey("portfolio_id")]
        public virtual Portfolio Portfolio
        {
            get { return _portfolio; }
            set
            {
                _portfolio = value;
                portfolio_id = _portfolio != null ? _portfolio.PortfolioId : 0;
            }
        }

        [ForeignKey("category_id")]
        public virtual ProductCategory ProductCategory
        {
            get { return _productCategory; }
            set
            {
                _productCategory = value;
                if (_productCategory != null)
                    category_id = _productCategory.CategoryId;
            }
        }

        [ForeignKey("user_id")]
        public virtual User User
        {
            get { return _user; }
            set
            {
                _user = value;
                if (_user != null)
                    user_id = _user.UserId;
            }
        }
    }
}
