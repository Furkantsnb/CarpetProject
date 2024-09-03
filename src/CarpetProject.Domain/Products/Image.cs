using CarpetProject.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace CarpetProject.Products
{
    public class Image : FullAuditedEntity<int>
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        // Product ile bire çok ilişki
        public int? ProductId { get; set; }
        public Product Product { get; set; }

        // Category ile bire bir ilişki
        public int? CategoryId { get; set; }
        public Category Category { get; set; }

    }
}
