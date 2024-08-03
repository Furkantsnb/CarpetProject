using CarpetProject.Entities.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace CarpetProject.Entities.Products
{
    public class ProductWithCategoryDto 
    {
        public ProductDto Product { get; set; }
        public CategoryDto Category { get; set; }
        public int ProductCountInCategory { get; set; }
    }
}
