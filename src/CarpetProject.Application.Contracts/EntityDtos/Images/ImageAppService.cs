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
    public interface ImageAppService : ICrudAppService<ImageDto, int, PagedAndSortedResultRequestDto, CreateImageDto, UpdateImageDto>
    {
        Task HartDeleteAsync(int id);
    }
}
