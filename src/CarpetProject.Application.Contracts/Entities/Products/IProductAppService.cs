using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace CarpetProject.Entities.Products
{
    public interface IProductAppService :ICrudAppService<ProductDto,int,PagedAndSortedResultRequestDto,CreateProductDto,UpdateProductDto>
    {

        Task HartDeleteAsync(int id);
    }
}
