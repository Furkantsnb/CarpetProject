﻿using CarpetProject.Entities.Categories;
using CarpetProject.EntityDto.ProductImages;
using CarpetProject.EntityDtos.Tags;
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

        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string? Ingredients { get; set; }//ürün içeriği
        public string? Usage { get; set; }//ürün nasıl kullanılmalı
        public string? AdditionalInfo { get; set; }//ek bilgi
        public bool IsApproved { get; set; }//Aktif mi değilmi Kontrolü
        public DateTime ReleaseDate { get; set; } // Ürünün piyasaya çıkış tarihi


        public List<CreateCategoryDto> CategoryIds { get; set; }
        public List<CreateProductImageDto> ProductImages { get; set; }
        public List<CreateTagDto> Tags { get; set; }


    }
}
