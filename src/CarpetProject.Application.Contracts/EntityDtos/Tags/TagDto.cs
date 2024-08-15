﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace CarpetProject.EntityDtos.Tags
{
    public class TagDto : EntityDto<int>
    {
        public string Name { get; set; } // Etiket adı
        public string? Color { get; set; } // Opsiyonel: Etiket rengi (örneğin, #FF5733)
        public string? IconUrl { get; set; } // Opsiyonel: Etiket simgesi (görsel URL'si)
    }
}