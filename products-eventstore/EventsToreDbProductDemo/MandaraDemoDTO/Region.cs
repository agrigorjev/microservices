using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace MandaraDemoDTO
{
    public partial class Region: IReference
    {
        public Guid Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            var p = obj as Region;
            if (p == null)
            {
                return false;
            }

            return Id == p.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
