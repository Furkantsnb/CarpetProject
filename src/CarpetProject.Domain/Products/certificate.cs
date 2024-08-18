using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace CarpetProject.Products
{
    public class certificate : FullAuditedEntity<int>
    {
        public string Name { get; set; } // Etiket adı

        public virtual ICollection<Product> Products { get; set; } // Etiket ile ilişkilendirilmiş ürünler

        public certificate()
        {
            Products = new HashSet<Product>();
        }


    }
}
