using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Mandara.Entities.Enums;

namespace Mandara.Entities
{
    [Description("Company")]
    [Table("companies")]
    public class Company
    {
        private Region _region;

        public Company()
        {
            Brokers = new HashSet<Broker>();
            CompanyAliases = new HashSet<CompanyAlias>();
            ProductsBrokerages = new HashSet<ProductBrokerage>();
            Brokerages = new HashSet<CompanyBrokerage>();
        }

        [Column("company_id")]
        [Key]
        public int CompanyId { get; set; }

        [Column("company_name")]
        [Required]
        [StringLength(255)]
        public string CompanyName { get; set; }

        [Column("region_id")]
        public int? RegionId { get; set; }

        [Column("abbreviation")]
        [StringLength(4)]
        public string AbbreviationName { get; set; }

        [Column("default_brokerage")]
        public decimal? DefaultBrokerage { get; set; }

        [Column("default_brokerage_kt")]
        public decimal? DefaultBrokerageKt { get; set; }

        [Column("HalVhalColor")]
        public int? HalVhalColor { get; set; }

        [ForeignKey("RegionId")]
        public virtual Region Region
        {
            get { return _region; }
            set
            {
                _region = value;
                RegionId = _region != null ? _region.RegionId : default (int);
            }
        }

        public virtual ICollection<Broker> Brokers { get; set; }

        public virtual ICollection<CompanyAlias> CompanyAliases { get; set; }

        public virtual ICollection<ProductBrokerage> ProductsBrokerages { get; set; }

        public virtual ICollection<CompanyBrokerage> Brokerages { get; set; }

        [NotMapped]
        public Company Instance
        {
            get
            {
                return this;
            }
        }

        [NotMapped]
        public string Abbr
        {
            get
            {
                return AbbreviationName ?? CompanyName.ToUpper().Substring(0, 3);
            }
        }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Region return false.
            var p = obj as Company;
            if (p == null)
            {
                return false;
            }

            return CompanyId == p.CompanyId;
        }

        public override int GetHashCode()
        {
            return CompanyId.GetHashCode();
        }

        public override string ToString()
        {
            return CompanyName;
        }

        public decimal? GetBrokerage(Unit unit, decimal numOfLots)
        {
            if (Brokerages == null || unit == null)
                return null;

            var companyBrokerage = Brokerages.FirstOrDefault(x => unit.Equals(x.Unit));

            if (companyBrokerage == null)
                return null;

            if (companyBrokerage.VolumeVolume.HasValue)
            {
                switch (companyBrokerage.VolumeCondition)
                {
                    case VolumeCondition.Lt:
                        if (numOfLots < companyBrokerage.VolumeVolume.Value)
                            return companyBrokerage.VolumeBrokerage;
                        break;
                    case VolumeCondition.Gt:
                        if (numOfLots > companyBrokerage.VolumeVolume.Value)
                            return companyBrokerage.VolumeBrokerage;
                        break;
                }
            }

            return companyBrokerage.DefaultBrokerage;
        }
    }
}
