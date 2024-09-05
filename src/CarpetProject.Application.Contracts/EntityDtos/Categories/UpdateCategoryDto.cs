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
     public string Name { get; set; }
        public string Description { get; set; }
        public int? ImageId { get; set; }
        public int? ParentCategoryId { get; set; }
        public bool IsApproved { get; set; }
        public string ColorCode { get; set; }

        // Kategori ürünleri ID'leri
        public List<int> ProductIds { get; set; } = new List<int>();


    }
}
