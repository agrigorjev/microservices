using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("adm_alert_group_phones")]
    public partial class AlertPhone
    {
        private AdministrativeAlertGroup _group;

        [Key]
        [Column("adm_alert_group_phone_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AlertPhoneID { get; set; }

        [Column("alert_group_id")]
        public int AlertGroupId { get; set; }

        [Column("phone")]
        [StringLength(15)]
        public string Phone { get; set; }

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
