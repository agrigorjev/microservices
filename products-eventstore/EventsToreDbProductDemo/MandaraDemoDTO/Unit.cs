
using System.ComponentModel.DataAnnotations;

namespace MandaraDemoDTO
{
    public partial class Unit: IReference
    {

        public Guid Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
        public decimal? DefaultPositionFactor { get; set; }
        public bool AllowOnlyMonthlyContractSize { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Unit entity && Id == entity.Id;
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
