using CarpetProject.Entities.Categories;
using CarpetProject.Entities.Products;
using CarpetProject.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace CarpetProject.Categories
{
    public class CategoryAppService : CrudAppService<Category, CategoryDto, int, PagedAndSortedResultRequestDto, CreateCategoryDto, UpdateCategoryDto>, ICategoryAppService
    {
        private readonly CategoryManager _categoryManager;
        public CategoryAppService(IRepository<Category, int> repository , CategoryManager categoryManager) : base(repository)
        {
            _categoryManager = categoryManager; 
        }
        public override async Task<CategoryDto> CreateAsync(CreateCategoryDto input)
        {
            return await _categoryManager.CreateAsync(input);
        }

        public override async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto input)
        {
            return await _categoryManager.UpdateAsync(id, input);
        }

        public override async Task DeleteAsync(int id)
        {
            await _categoryManager.DeleteAsync(id);
        }

        public override async Task<CategoryDto> GetAsync(int id)
        {
            return await _categoryManager.GetAsync(id);
        }

        public override Task<PagedResultDto<CategoryDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return base.GetListAsync(input);
        }

        public async Task HartDeleteAsync(int id)
        {
            await Repository.HardDeleteAsync(c => c.Id == id);
        }
    }
}
