using CarpetProject.Entities.Categories;
using CarpetProject.EntityDto.ProductImages;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace CarpetProject.Entities.Products
{
    public class CreateProductDto 
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

        // Bir ürünün atanacağı kategori ID'lerini tutan liste
        public List<int> CategoryIds { get; set; }

        // Ürün için yüklenen görsellerin ID'lerini tutan liste
        public List<int> ImageIds { get; set; }


    }
}
