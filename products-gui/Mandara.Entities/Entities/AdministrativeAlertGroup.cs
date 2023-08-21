using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("adm_alert_groups")]
    public partial class AdministrativeAlertGroup
    {
        public AdministrativeAlertGroup()
        {
            Emails = new HashSet<AlertEmailAddress>();
            Phones = new HashSet<AlertPhone>();
            adm_alerts = new HashSet<AdministrativeAlert>();
            adm_alerts1 = new HashSet<AdministrativeAlert>();
            Users = new HashSet<User>();
            adm_alerts2 = new HashSet<AdministrativeAlert>();
            adm_alerts3 = new HashSet<AdministrativeAlert>();
            VoicePhones = new HashSet<AlertVoicePhone>();
        }

        [Column("alert_group_id")]
        [Key]
        public int GroupId { get; set; }

        [Column("group_title")]
        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        public virtual ICollection<AlertEmailAddress> Emails { get; set; }
        public virtual ICollection<AlertPhone> Phones { get; set; }
        public virtual ICollection<AdministrativeAlert> adm_alerts { get; set; }
        public virtual ICollection<AdministrativeAlert> adm_alerts1 { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<AdministrativeAlert> adm_alerts2 { get; set; }
        public virtual ICollection<AdministrativeAlert> adm_alerts3 { get; set; }
        public virtual ICollection<AlertVoicePhone> VoicePhones { get; set; }
    }
}
