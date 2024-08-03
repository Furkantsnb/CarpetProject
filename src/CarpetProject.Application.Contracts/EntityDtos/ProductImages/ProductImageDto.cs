using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace CarpetProject.EntityDto.ProductImages
{
    public class ProductImageDto : EntityDto<int>
    {
        public int ProductId { get; set; }
        public string ImageUrl { get; set; }
    }
}
