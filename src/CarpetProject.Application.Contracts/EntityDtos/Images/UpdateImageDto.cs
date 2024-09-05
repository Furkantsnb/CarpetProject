using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetProject.EntityDto.ProductImages
{
    public class UpdateImageDto
    {
        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public int? ProductId { get; set; }
        public IFormFile ImageUrl { get; set; } // Dosya yükleme için IFormFile kullanıyoruz
    }
}
