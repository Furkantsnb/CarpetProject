using CarpetProject.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace CarpetProject.Entities.Categories
{
    public interface ICategoryAppService:ICrudAppService<CategoryDto, int, PagedAndSortedResultRequestDto, CreateCategoryDto, UpdateCategoryDto>
    {
        Task HartDeleteAsync(int id);
    }
}
