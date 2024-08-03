using CarpetProject.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace CarpetProject.Products
{
    public class Product : FullAuditedEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
      
        public bool IsApproved { get; set; }
        public bool IsHome { get; set; }
        public bool IsCertified { get; set; }
        public string CertificationDetails { get; set; }
        public string Ingredients { get; set; }
        public string Usage { get; set; }
        public string AdditionalInfo { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }

        public Product()
        {
            Categories = new HashSet<Category>();
            ProductImages = new HashSet<ProductImage>();
        }
    }
}
