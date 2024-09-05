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
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ImageId { get; set; }
        public int? ParentCategoryId { get; set; }
        public bool IsApproved { get; set; }
        public string ColorCode { get; set; }

        // Kategoriye ait resim
        public ImageDto Image { get; set; }

        // Alt kategori (üstsüz kategori)
        public CategoryDto ParentCategory { get; set; }

        // Kategori ürünleri DTO'ları
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();

    }
}
