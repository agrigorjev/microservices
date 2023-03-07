using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities.Entities
{
    [Serializable]
    [Table("product_class")]
    public class ProductClass
    {
        public ProductClass()
        {
            Categories = new HashSet<ProductCategory>();
        }

        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        private string _name;

        [Column("name")]
        [Required]
        public string Name
        {
            get => _name;
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    _name = value;
                }
            }
        }

        public virtual ICollection<ProductCategory> Categories { get; set; }
    }
}
