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
    public class UpdateProductDto 
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool HasDiscount { get; set; }
        public decimal? OldPrice { get; set; }
        public bool Certification { get; set; }
        public string Description { get; set; }
        public string? Ingredients { get; set; }
        public string? Usage { get; set; }
        public string? AdditionalInfo { get; set; }
        public bool IsApproved { get; set; }

        // Kategori ID'leri
        public List<int> CategoryIds { get; set; } = new List<int>();

        // Resim ID'leri
        public List<int> ImageIds { get; set; } = new List<int>();
    }
}
