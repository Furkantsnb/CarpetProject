using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetProject.EntityDto.ProductImages
{
    public class CreateProductImageDto
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public int? ProductId { get; set; }
    }
}
