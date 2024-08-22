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
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; } // To display the name of the parent category
        public bool IsApproved { get; set; }
        public string ColorCode { get; set; }

        public ICollection<CategoryDto> SubCategories { get; set; } // For nested categories
        public ICollection<ProductDto> Products { get; set; } // Products associated with this category
        public ProductImageDto ProductImage { get; set; } // Image associated with the category

        public CategoryDto()
        {
            SubCategories = new List<CategoryDto>();
            Products = new List<ProductDto>();
        }

    }
}
