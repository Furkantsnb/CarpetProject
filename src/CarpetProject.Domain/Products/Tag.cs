using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace CarpetProject.Products
{
    public class Tag : FullAuditedEntity<int>
    {
        public string Name { get; set; } // Etiket adı
        public string? Color { get; set; } // Opsiyonel: Etiket rengi (örneğin, #FF5733)
        public string? IconUrl { get; set; } // Opsiyonel: Etiket simgesi (görsel URL'si)

        // Etiket ile ilişkilendirilmiş ürünler
        public virtual ICollection<Product> Products { get; set; } // Etiket ile ilişkilendirilmiş ürünler

        public Tag()
        {
            Products = new HashSet<Product>();
        }


    }
}
