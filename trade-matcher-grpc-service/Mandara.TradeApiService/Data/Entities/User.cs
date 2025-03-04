using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.TradeApiService.Data
{
    [Table("users")]
    public partial class User
    {
        public User()
        {
        }

        [Column("user_name")]
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Column("first_name")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Column("last_name")]
        [StringLength(100)]
        public string LastName { get; set; }

        [Column("user_id")]
        [Key]
        public int UserId { get; set; }

        [Column("default_portfolio_id")]
        public int? DefaultPortfolioId { get; set; }

        [Column("master_password_hash")]
        [StringLength(2000)]
        public string MasterPasswordHash { get; set; }

        [Column("locked")]
        public bool? Locked { get; set; }

        [Column("force_password_change")]
        public short? ForcePasswordChangeDb { get; set; }

        [Column("last_password_change")]
        public DateTime? LastPasswordChange { get; set; }

        [Column("days_to_change_pass")]
        public int? DaysBetweenPasswordChange { get; set; }

        [Column("failed_auth_attempts")]
        public int? FailedAuthAttempts { get; set; }

        [Column("mifid_short_code")]
        public int MifidShortCode { get; set; }

        [ForeignKey("DefaultPortfolioId")]
        public virtual Portfolio Portfolio
        {
            get { return _portfolio; }
            set
            {
                _portfolio = value;
                DefaultPortfolioId = _portfolio != null ? _portfolio.PortfolioId : (int?)null;
            }
        }

        private static User _currentUser;
        private Portfolio _portfolio;

        [NotMapped]
        public static User CurrentUser
        {
            get
            {
                return _currentUser;
            }
            set { _currentUser = value; }
        }

        [NotMapped]
        public User Instance
        {
            get { return this; }
        }

        public override string ToString()
        {
            return string.Format("Username: {0}", UserName);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var p = obj as User;
            if (p == null)
            {
                return false;
            }

            return UserId == p.UserId;
        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode();
        }

        [NotMapped]
        public bool LockedValue { get { return !Locked.HasValue ? false : Locked.Value; } set { Locked = value; } }
    }
}
