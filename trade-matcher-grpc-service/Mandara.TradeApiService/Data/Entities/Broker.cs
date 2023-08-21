using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.TradeApiService.Data
{
    [Table("brokers")]
    public class Broker
    {
        private Company _company;
        private ParserDefaultProduct _parserDefaultProduct;

        public Broker()
        {
        }

        [Column("broker_id")]
        [Key]
        public int BrokerId { get; set; }

        [Column("yahoo_id")]
        [Required]
        [StringLength(255)]
        public string YahooId { get; set; }

        [Column("company_id")]
        public int? CompanyId { get; set; }

        [Column("channel_name")]
        [StringLength(50)]
        public string channel_name { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company
        {
            get { return _company; }
            set
            {
                _company = value;
                CompanyId = _company != null ? _company.CompanyId : (int?) null;
            }
        }

        [ForeignKey("BrokerId")]
        public virtual ParserDefaultProduct ParserDefaultProduct
        {
            get { return _parserDefaultProduct; }
            set { _parserDefaultProduct = value; }
        }

        [NotMapped]
        public Broker Instance
        {
            get { return this; }
        }

        [NotMapped]
        public OfficialProduct DefaultProduct
        {
            get { return ParserDefaultProduct != null ? ParserDefaultProduct.OfficialProduct : null; }
            set
            {
                if (ParserDefaultProduct == null)
                    ParserDefaultProduct = new ParserDefaultProduct
                    {
                        BrokerId = this.BrokerId
                    };

                ParserDefaultProduct.OfficialProduct = value;
            }
        }

        [NotMapped]
        public string DisplayName
        {
            get
            {
                if (channel_name != YahooId || Company == null || String.IsNullOrEmpty(Company.CompanyName))
                    return YahooId;
                else
                    return Company.CompanyName;
            }
        }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Unit return false.
            var p = obj as Broker;
            if ((System.Object)p == null)
            {
                return false;
            }

            return BrokerId == p.BrokerId;
        }

        public override int GetHashCode()
        {
            return BrokerId.GetHashCode();
        }

        public override string ToString()
        {
            return YahooId;
        }
    }
}
