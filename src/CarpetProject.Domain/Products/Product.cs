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
        public string ImageUrl { get; set; }
        public bool IsApproved { get; set; }//ürün satışa koyulsun mu koyulmasın mı?
        public bool IsHome { get; set; }//Ana sayfada gözüksün mü gözükmesin mi
        public bool IsCertified { get; set; }//ürün sertfikası varsa 

        public string CertificationDetails { get; set; }
        public string Ingredients { get; set; }//ürün içeriğini belirtir.
        public string Usage { get; set; }//ürünün nasıl kullanılması gerektiğini içerir
        public string AdditionalInfo { get; set; }//ürün hakkında ek bilgi verir

        public virtual ICollection<Category> Categories { get; set; }

        public Product()
        {
            Categories = new HashSet<Category>();
        }
    }
}
