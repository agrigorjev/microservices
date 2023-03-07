using System.ComponentModel.DataAnnotations.Schema;
using Mandara.Entities.Enums;

namespace Mandara.Entities
{
    [Table("company_brokerages")]
    public class CompanyBrokerage
    {
        private Company _company;
        private Unit _unit;

        [Column("id")]
        public int CompanyBrokerageId { get; set; }

        [Column("company_id")]
        public int CompanyId { get; set; }

//        [Column("unit")]
//        public int UnitDb { get; set; }

        [Column("default_brokerage")]
        public decimal? DefaultBrokerage { get; set; }

        [Column("volume_brokerage")]
        public decimal? VolumeBrokerage { get; set; }

        [Column("volume_condition")]
        public int? VolumeConditionDb { get; set; }

        [Column("volume_volume")]
        public decimal? VolumeVolume { get; set; }

        [Column("unit_id")]
        public int? UnitId { get; set; }

        public virtual Company Company
        {
            get { return _company; }
            set
            {
                _company = value;
                CompanyId = _company != null ? _company.CompanyId : default(int);
            }
        }

        [ForeignKey("UnitId")]
        public virtual Unit Unit
        {
            get { return _unit; }
            set
            {
                _unit = value;
                UnitId = _unit != null ? _unit.UnitId : 0;
            }
        }

        [NotMapped]
        public VolumeCondition? VolumeCondition
        {
            get
            {
                if (!VolumeConditionDb.HasValue)
                    return null;

                return (VolumeCondition)VolumeConditionDb;
            }
            set { VolumeConditionDb = (int?)value; }
        }

    }
}
