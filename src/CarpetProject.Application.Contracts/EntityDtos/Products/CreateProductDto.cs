using CarpetProject.EntityDto.ProductImages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace CarpetProject.Entities.Products
{
    public class CreateProductDto 
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int Discount { get; set; }//indirim
        public bool DiscountAvailable { get; set; }//indirim var mı?
        public bool IsApproved { get; set; }//Aktif mi değilmi Kontrolü
        public bool IsHome { get; set; }//Ana sayfada gözüksün mü?
        public bool IsCertified { get; set; }//sertfikası varmı
        public string? CertificationDetails { get; set; }//serfika detayı
        public string Ingredients { get; set; }//ürün içeriği
        public string Usage { get; set; }//ürün nasıl kullanılmalı
        public string AdditionalInfo { get; set; }//ek bilgi

        public List<int> CategoryIds { get; set; }
        public List<CreateProductImageDto> ProductImages { get; set; }
    }
}
