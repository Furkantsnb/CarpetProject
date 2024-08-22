using CarpetProject.Entities.Products;
using CarpetProject.EntityDto.ProductImages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetProject.Entities.Categories
{
    public class CreateCategoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentCategoryId { get; set; } // Optional for creating a subcategory
        public bool IsApproved { get; set; } = true; // Default to approved
        public string ColorCode { get; set; } // Renk kodu

     
    }
}
