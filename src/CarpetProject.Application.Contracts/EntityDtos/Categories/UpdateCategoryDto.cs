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
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentCategoryId { get; set; } // Optional for changing the parent category
        public bool IsApproved { get; set; }
        public string ColorCode { get; set; }

      
    }
}
