using CarpetProject.Entities.Products;
using CarpetProject.EntityDto.ProductImages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace CarpetProject.Entities.Categories
{
    public class CategoryDto: FullAuditedEntityDto<int>
    {
        public string Name { get; set; } // Kategori adı
        public string Description { get; set; } // Kategori açıklaması
        public int? ParentCategoryId { get; set; } // Üst kategori ID'si
        public bool IsApproved { get; set; } // Kategori onay durumu
        public string ColorCode { get; set; } // Renk kodu

        public List<int>? Products { get; set; } // Bu kategoriye ait ürünler
        public int ImageId { get; set; } // Kategoriye ait resim

        public CategoryDto()
        {

            Products = new List<int>();
        }

    }
}
