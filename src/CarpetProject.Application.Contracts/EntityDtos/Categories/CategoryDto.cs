﻿using CarpetProject.Entities.Products;
using CarpetProject.EntityDto.ProductImages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace CarpetProject.Entities.Categories
{
    public class CategoryDto: FullAuditedEntityDto<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ImageId { get; set; }
        public int? ParentCategoryId { get; set; }
        public bool IsApproved { get; set; }//Aktif mi değilmi Kontrolü
        public string ColorCode { get; set; } // Renk kodu

        public List<int>? Products { get; set; }

        public CategoryDto()
        {
          Products=new List<int>();
        }

    }
}
