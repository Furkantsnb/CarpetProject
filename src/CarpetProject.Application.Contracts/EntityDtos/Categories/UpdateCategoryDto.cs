using CarpetProject.Entities.Products;
using CarpetProject.EntityDto.ProductImages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetProject.Entities.Categories
{
    public class UpdateCategoryDto
    {
        public string Name { get; set; } // Kategori adı
        public string Description { get; set; } // Kategori açıklaması
        public int? ParentCategoryId { get; set; } // Üst kategori ID'si
        public bool IsApproved { get; set; } // Kategori onay durumu
        public string ColorCode { get; set; } // Renk kodu

        public List<int> SubCategories { get; set; } // Alt kategoriler
        public List<int> Products { get; set; } // Bu kategoriye ait ürünler
        public int ProductImage { get; set; } // Kategoriye ait resim

        public UpdateCategoryDto()
        {
            SubCategories = new List<int>();
            Products = new List<int>();
        }


    }
}
