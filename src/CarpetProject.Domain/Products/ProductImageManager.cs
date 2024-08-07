using AutoMapper;
using CarpetProject.Categories;
using CarpetProject.Entities.Products;
using CarpetProject.EntityDto.ProductImages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace CarpetProject.Products
{
    public class ProductImageManager : DomainService
    {
        private readonly IRepository<Product, int> _productRepository;
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly IRepository<ProductImage, int> _productImage;
        private readonly IMapper _mapper;

        public ProductImageManager(IRepository<Product, int> productRepository, IRepository<Category, int> categoryRepository, IRepository<ProductImage, int> productImage, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productImage = productImage;
            _mapper = mapper;
        }

        public async Task<ProductImageDto> CreateAsync(CreateProductImageDto input)
        {
            var productImage = _mapper.Map<CreateProductImageDto, ProductImage>(input);
            await _productImage.InsertAsync(productImage);
            return _mapper.Map<ProductImage, ProductImageDto>(productImage);
        }

        public async Task<ProductImageDto> UpdateAsync(int id, UpdateProductImageDto input)
        {
            var productImage = await _productImage.GetAsync(id);
            _mapper.Map(input, productImage);
            await _productImage.UpdateAsync(productImage);
            return _mapper.Map<ProductImage, ProductImageDto>(productImage);
        }

        public async Task DeleteAsync(int id)
        {
            await _productImage.DeleteAsync(id);
        }

        public async Task<ProductImageDto> GetAsync(int id)
        {
            var productImage = await _productImage.GetAsync(id);
            return _mapper.Map<ProductImage, ProductImageDto>(productImage);
        }

        public async Task<PagedResultDto<ProductImageDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var queryable = await _productImage.GetQueryableAsync();
            var totalCount = await AsyncExecuter.CountAsync(queryable);

            var lines = await AsyncExecuter.ToListAsync(
                queryable.OrderBy(a => a.Id)
                         .Skip(input.SkipCount)
                         .Take(input.MaxResultCount)
            );

            var productImageDtos = _mapper.Map<List<ProductImage>, List<ProductImageDto>>(lines);

            return new PagedResultDto<ProductImageDto>(
                totalCount,
                productImageDtos
            );
        }
    }
}
