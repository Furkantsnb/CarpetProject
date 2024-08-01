using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarpetProject.Entities.Products;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace CarpetProject.Products
{
    public class ProductAppService :CrudAppService<Product,ProductDto,int,PagedAndSortedResultRequestDto,CreateProductDto,UpdateProductDto>, IProductAppService
    {
        private readonly ProductManager _productManager;

        public ProductAppService(IRepository<Product,int> repository, ProductManager productManager):base(repository)
        {
            _productManager = productManager;
        }

        public override async Task<ProductDto> CreateAsync(CreateProductDto input)
        {
            return await _productManager.CreateAsync(input);
        }

        public override async Task<ProductDto> UpdateAsync(int id, UpdateProductDto input)
        {
            return await _productManager.UpdateAsync(id, input);
        }

        public override  async Task DeleteAsync(int id)
        {
            await _productManager.DeleteAsync(id);
        }

        public override async Task<ProductDto> GetAsync(int id)
        {
            return await _productManager.GetAsync(id);
        }

        public override Task<PagedResultDto<ProductDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return base.GetListAsync(input);
        }

        public async Task HartDeleteAsync(int id)
        {
            await Repository.HardDeleteAsync(c=>c.Id==id);
        }
        
    }
}
