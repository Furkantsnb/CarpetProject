using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace CarpetProject.Entities.Categories
{
    public class CategoryDto:EntityDto<int>
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int? ParentCategoryId { get; set; }
        public bool IsApproved { get; set; }//Aktif mi değilmi Kontrolü
        public List<CategoryDto> SubCategories { get; set; }
    }
}
