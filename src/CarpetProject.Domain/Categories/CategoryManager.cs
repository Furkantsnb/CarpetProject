using AutoMapper;
using CarpetProject.Entities.Categories;
using CarpetProject.Entities.Products;
using CarpetProject.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace CarpetProject.Categories
{
    public class CategoryManager : DomainService
    {
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryManager(IRepository<Category, int> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto input)
        {
            var category = _mapper.Map<CreateCategoryDto, Category>(input);
            await _categoryRepository.InsertAsync(category);
            return _mapper.Map<Category, CategoryDto>(category);
        }

        public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto input)
        {
            var category = await _categoryRepository.GetAsync(id);
            _mapper.Map(input, category);
            await _categoryRepository.UpdateAsync(category);
            return _mapper.Map<Category, CategoryDto>(category);
        }

        public async Task DeleteAsync(int id)
        {
            await _categoryRepository.DeleteAsync(id);
        }

        public async Task<CategoryDto> GetAsync(int id)
        {
            var category = await _categoryRepository.GetAsync(id);
            return _mapper.Map<Category, CategoryDto>(category);
        }

        public async Task<PagedResultDto<CategoryDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var queryable = await _categoryRepository.GetQueryableAsync();
            var totalCount = await AsyncExecuter.CountAsync(queryable);

            var lines = await AsyncExecuter.ToListAsync(
                queryable.OrderBy(a => a.Name)
                         .Skip(input.SkipCount)
                         .Take(input.MaxResultCount)
            );

            var categoryDtos = _mapper.Map<List<Category>, List<CategoryDto>>(lines);

            return new PagedResultDto<CategoryDto>(
                totalCount,
                categoryDtos
            );
        }
    }
}
