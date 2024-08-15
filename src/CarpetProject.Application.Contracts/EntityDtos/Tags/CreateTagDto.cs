using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetProject.EntityDtos.Tags
{
    public class CreateTagDto
    {
        public string Name { get; set; } // Etiket adı
        public string? Color { get; set; } // Opsiyonel: Etiket rengi (örneğin, #FF5733)
        public string? IconUrl { get; set; } // Opsiyonel: Etiket simgesi (görsel URL'si)
    }
}
