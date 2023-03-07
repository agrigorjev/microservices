using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("adm_alert_group_emails")]
    public partial class AlertEmailAddress
    {
        private AdministrativeAlertGroup _group;

        [Key]
        [Column("adm_alert_group_email_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AlertEmailAddressID { get; set; }

        [Column("alert_group_id")]
        public int AlertGroupId { get; set; }

        [Column("email")]
        [StringLength(250)]
        public string Email { get; set; }

        [ForeignKey("AlertGroupId")]
        public virtual AdministrativeAlertGroup Group
        {
            get { return _group; }
            set
            {
                _group = value;
                AlertGroupId = _group != null ? _group.GroupId : 0;
            }
        }
    }
}
