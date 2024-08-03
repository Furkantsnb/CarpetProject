using CarpetProject.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace CarpetProject.Products
{
    public class ProductImageAppService : CrudAppService<Product, ProductDto, int, PagedAndSortedResultRequestDto, CreateProductDto, UpdateProductDto>, IProductAppService
    {
        public ProductImageAppService(IRepository<Product, int> repository) : base(repository)
        {
        }

        public Task HartDeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
