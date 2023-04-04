using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataImport.DataEntries
{
    [Table("regions")]
    public partial class Region
    {
        public Region()
        {
            
        }

        [Column("region_id")]
        [Key]
        public int RegionId { get; set; }

        [Column("region_name")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }


        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Region return false.
            var p = obj as Region;
            if ((System.Object)p == null)
            {
                return false;
            }

            return RegionId == p.RegionId;
        }

        public override int GetHashCode()
        {
            return RegionId.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
