using CarpetProject.Entities.Products;
using CarpetProject.EntityDto.ProductImages;
using CarpetProject.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace CarpetProject.ProductImages
{
    public class ProductImageAppService : CrudAppService<ProductImage, ProductImageDto, int, PagedAndSortedResultRequestDto, CreateProductImageDto, UpdateProductImageDto>, IProductImageAppService
    {
        private readonly ProductImageManager _productImageManager;

        public ProductImageAppService(IRepository<ProductImage, int> repository, ProductImageManager productImageManager) : base(repository)
        {
            _productImageManager = productImageManager;
        }

        public override async Task<ProductImageDto> CreateAsync(CreateProductImageDto input)
        {
            return await _productImageManager.CreateAsync(input);
        }

        public override async Task<ProductImageDto> UpdateAsync(int id, UpdateProductImageDto input)
        {
            return await _productImageManager.UpdateAsync(id, input);
        }

        public override async Task DeleteAsync(int id)
        {
            await _productImageManager.DeleteAsync(id);
        }

        public override async Task<ProductImageDto> GetAsync(int id)
        {
            return await _productImageManager.GetAsync(id);
        }

        public override async Task<PagedResultDto<ProductImageDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return await _productImageManager.GetListAsync(input);
        }

        public async Task HartDeleteAsync(int id)
        {
            await Repository.HardDeleteAsync(c => c.Id == id);
        }
    }
}
