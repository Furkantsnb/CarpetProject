using CarpetProject.EntityDto.ProductImages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace CarpetProject.Entities.Products
{
    public class UpdateProductDto 
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public bool IsApproved { get; set; }
        public bool IsHome { get; set; }
        public bool IsCertified { get; set; }
        public string CertificationDetails { get; set; }
        public string Ingredients { get; set; }
        public string Usage { get; set; }
        public string AdditionalInfo { get; set; }

        public List<int> CategoryIds { get; set; }
        public List<UpdateProductImageDto> ProductImages { get; set; }
    }
}
