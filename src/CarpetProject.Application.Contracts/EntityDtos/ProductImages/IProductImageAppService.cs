using CarpetProject.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace CarpetProject.EntityDto.ProductImages
{
    public interface IProductImageAppService : ICrudAppService<ProductImageDto, int, PagedAndSortedResultRequestDto, CreateProductImageDto, UpdateProductImageDto>
    {
        Task HartDeleteAsync(int id);
    }
}
