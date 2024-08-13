using CarpetProject.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace CarpetProject.Categories
{
    public class Category : FullAuditedEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int? ParentCategoryId { get; set; }
        public bool IsApproved { get; set; }//Aktif mi değilmi Kontrolü
        public string ColorCode { get; set; } // Renk kodu

        public virtual Category ParentCategory { get; set; }//üst kategory
        public virtual ICollection<Category> SubCategories { get; set; }//alt kategoriy
        public virtual ICollection<Product> Products { get; set; }//kategorideki ürünler

        public Category()
        {
            SubCategories = new HashSet<Category>();
            Products = new HashSet<Product>();
        }
    }
}
