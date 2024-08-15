using CarpetProject.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace CarpetProject.Products
{
    public class Product : FullAuditedEntity<int>
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string? Ingredients { get; set; }//ürün içeriği
        public string? Usage { get; set; }//ürün nasıl kullanılmalı
        public string? AdditionalInfo { get; set; }//ek bilgi
        public int Discount { get; set; }//indirim yüzdesi
        public bool DiscountAvailable { get; set; }//indirim var mı?
        public bool IsApproved { get; set; }//Aktif mi değilmi Kontrolü
        public DateTime ReleaseDate { get; set; } // Ürünün piyasaya çıkış tarihi
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<Tag> Tags { get; set; } // Ürüne ait etiketler
     



        public Product()
        {
            Categories = new HashSet<Category>();
            ProductImages = new HashSet<ProductImage>();
            Tags = new HashSet<Tag>();
        

        }
    }
}
