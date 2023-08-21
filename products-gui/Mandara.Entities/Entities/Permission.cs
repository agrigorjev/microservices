using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("permissions")]
    public partial class Permission
    {
        public Permission()
        {
            Groups = new HashSet<Group>();
        }

        [Column("permission_id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PermissionId { get; set; }

        [Column("permission_name")]
        [Required]
        [StringLength(100)]
        public string PermissionName { get; set; }

        public virtual ICollection<Group> Groups { get; set; }


        [NotMapped]
        public Permission Instance
        {
            get { return this; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var p = obj as Permission;
            if (p == null)
            {
                return false;
            }

            return PermissionId == p.PermissionId;
        }

        public override int GetHashCode()
        {
            return PermissionId.GetHashCode();
        }
    }
}
