using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace CarpetProject.EntityDto.ProductImages
{
    public class ImageDto : FullAuditedEntityDto<int>
    {
        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public int? ProductId { get; set; }
        public string ImageUrl { get; set; } // Dosya yükleme için IFormFile kullanıyoruz
    }
}
