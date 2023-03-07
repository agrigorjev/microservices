using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("groups")]
    public class Group
    {
        public Group()
        {
            Permissions = new HashSet<Permission>();
            Users = new HashSet<User>();
        }

        [Column("group_id")]
        [Key]
        public int GroupId { get; set; }

        [Column("group_name")]
        [Required]
        [StringLength(100)]
        public string GroupName { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }

        public virtual ICollection<User> Users { get; set; }

        private readonly List<string> _untrackedProperties = new List<string> { "ChangeTracker", "AuditMessages", "Users" };

        [NotMapped]
        public Group Instance
        {
            get
            {
                return this;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var p = obj as Group;
            if (p == null)
            {
                return false;
            }

            return GroupId == p.GroupId;
        }

        public override int GetHashCode()
        {
            return GroupId.GetHashCode();
        }

        public override string ToString()
        {
            return GroupName;
        }

    }
}
