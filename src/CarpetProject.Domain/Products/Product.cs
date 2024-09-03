using CarpetProject.Categories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.CmsKit.Blogs;
using Volo.CmsKit.Comments;
using Volo.CmsKit.Tags;


namespace CarpetProject.Products
{
    public class Product : FullAuditedEntity<int>
    {
      
        public string Name { get; set; }
        public decimal Price { get; set; } // Yeni fiyat 
        public bool HasDiscount { get; set; }//indirim var mı
        public decimal? OldPrice { get; set; } // Eski fiyat (indirim uygulanmışsa)
        public bool Certification { get; set; }//serfikası var mı?
        public string Description { get; set; }//açıklama
        public string? Ingredients { get; set; }//ürün içeriği
        public string? Usage { get; set; }//ürün nasıl kullanılmalı
        public string? AdditionalInfo { get; set; }//ek bilgi     
        public bool IsApproved { get; set; }//Aktif mi değilmi Kontrolü
      
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Image>? Images { get; set; }
        



        public Product()
        {
            Categories = new HashSet<Category>();
            Images = new HashSet<Image>();
            
        }
    }
}
