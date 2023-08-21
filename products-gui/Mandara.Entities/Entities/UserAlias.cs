using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("user_aliases")]
    public partial class UserAlias
    {
        private User _user;

        [Key]
        [Column("user_id", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        [Key]
        [Column("alias", Order = 1)]
        [StringLength(100)]
        public string Alias { get; set; }

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
    }
}
