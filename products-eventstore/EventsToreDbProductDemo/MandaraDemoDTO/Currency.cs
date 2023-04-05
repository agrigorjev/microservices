using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MandaraDemoDTO
{
    public partial class Currency
    {
 
        public Guid Id { get; set; }

        private string _isoName;

        [StringLength(3)]
        public string IsoName
        {
            get => _isoName;
            set
            {
                if (value.Trim().Length != 3)
                {
                    throw new ArgumentOutOfRangeException(
                        "ISO name",
                        "A currency ISO name is exactly 3 uppercase characters.");
                }

                _isoName = value.ToUpper();
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Currency entity && Id == entity.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return IsoName;
        }
    }
}
