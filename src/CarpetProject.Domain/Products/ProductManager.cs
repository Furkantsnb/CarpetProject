using AutoMapper;
using CarpetProject.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using CarpetProject.Products;
using CarpetProject.Entities.Products;
using CarpetProject.Entities.Categories;

namespace CarpetProject.Products
{
    public class ProductManager : DomainService
    {
        private readonly IRepository<Product, int> _productRepository;
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly IMapper _mapper;

        public ProductManager(IRepository<Product, int> productRepository, IRepository<Category, int> categoryRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto input)
        {
            var product = _mapper.Map<CreateProductDto, Product>(input);
            await _productRepository.InsertAsync(product);
            return _mapper.Map<Product, ProductDto>(product);
        }

        public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto input)
        {
            var product = await _productRepository.GetAsync(id);
            _mapper.Map(input, product);
            await _productRepository.UpdateAsync(product);
            return _mapper.Map<Product, ProductDto>(product);
        }

        public async Task DeleteAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }

        public async Task<ProductDto> GetAsync(int id)
        {
            var product = await _productRepository.GetAsync(id);
            return _mapper.Map<Product, ProductDto>(product);
        }

        public async Task<PagedResultDto<ProductDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var queryable = await _productRepository.GetQueryableAsync();
            
            var totalCount = await AsyncExecuter.CountAsync(queryable);
            

            var lines = await AsyncExecuter.ToListAsync(
                queryable.OrderBy(a => a.Name)
                         .Skip(input.SkipCount)
                         .Take(input.MaxResultCount)
            );

            var productDtos = _mapper.Map<List<Product>, List<ProductDto>>(lines);

            return new PagedResultDto<ProductDto>(
                totalCount,
                productDtos
            );
        }

    }
}
