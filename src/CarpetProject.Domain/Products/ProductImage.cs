using CarpetProject.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace CarpetProject.Products
{
    public class ProductImage : FullAuditedEntity<int>
    {
        public string Name {  get; set; }
        public string ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public int? ProductId { get; set; }
       

        // İlgili kategori (birebir ilişki)
        public virtual Category Category { get; set; }
        // İlgili ürün (birebir ilişki)
        public virtual Product Product { get; set; }
    }
}
