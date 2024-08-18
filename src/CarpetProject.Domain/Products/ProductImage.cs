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
        public int ProductId { get; set; }
        public string ImageUrl { get; set; }
    
        public virtual Product Product { get; set; }
    }
}
