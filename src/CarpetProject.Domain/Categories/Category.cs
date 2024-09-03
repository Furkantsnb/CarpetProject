using CarpetProject.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.CmsKit.Comments;
using Volo.CmsKit.Tags;

namespace CarpetProject.Categories
{
    public class Category : FullAuditedEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ImageId { get; set; }
        public int? ParentCategoryId { get; set; }
        public bool IsApproved { get; set; }//Aktif mi değilmi Kontrolü
        public string ColorCode { get; set; } // Renk kodu
        public  Category ParentCategory { get; set; }//üst kategory
        public  ICollection<Product> Products { get; set; }//kategorideki ürünler
        public  Image Image { get; set; } // Kategoriye ait resim (Birebir ilişki)


        public Category()
        {
            Products = new HashSet<Product>();
        }
    }
}
