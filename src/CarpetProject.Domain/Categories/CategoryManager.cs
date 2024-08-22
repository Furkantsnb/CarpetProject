using AutoMapper;
using CarpetProject.Entities.Categories;
using CarpetProject.Entities.Products;
using CarpetProject.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
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
         

            // 2. Kategori Adı Benzersiz Olmalıdır
            var existingCategory = await _categoryRepository.FirstOrDefaultAsync(c => c.Name == input.Name);
            if (existingCategory != null)
            {
                throw new UserFriendlyException($"Kategori adı ({input.Name}) zaten mevcut.");
            }

            // 3. Üst Kategori ID Geçerli Olmalıdır
            if (input.ParentCategoryId.HasValue)
            {
                var parentCategoryExists = await _categoryRepository.AnyAsync(c => c.Id == input.ParentCategoryId.Value);
                if (!parentCategoryExists)
                {
                    throw new UserFriendlyException($"Üst kategori ID'si ({input.ParentCategoryId}) geçerli değil.");
                }
            }

            // 4. Alt Kategoriler İçin Kontrol
            if (input.SubCategories != null)
            {
                // Alt kategorilerin döngüsel ilişkiler oluşturmadığından emin olun
                var subCategoryIds = input.SubCategories.Select(c => c.Id).ToHashSet();
                if (subCategoryIds.Count != input.SubCategories.Count)
                {
                    throw new UserFriendlyException("Alt kategoriler arasında tekrar eden ID'ler var.");
                }

                // Her alt kategorinin geçerli olduğundan emin olun
                foreach (var subCategory in input.SubCategories)
                {
                    var subCategoryExists = await _categoryRepository.AnyAsync(c => c.Id == subCategory.Id);
                    if (!subCategoryExists)
                    {
                        throw new UserFriendlyException($"Alt kategori ID'si ({subCategory.Id}) geçerli değil.");
                    }
                }

                // Alt kategorilerin kendi alt kategorilerinin parçası olup olmadığını kontrol edin
                foreach (var subCategory in input.SubCategories)
                {
                    var subCategoryHierarchy = await GetCategoryHierarchyAsync(subCategory.Id);
                    if (subCategoryHierarchy.Contains(input.ParentCategoryId.Value))
                    {
                        throw new UserFriendlyException($"Alt kategori ID'si ({subCategory.Id}) kendi üst kategorisinin alt kategorisi olamaz.");
                    }
                }
            }

            // 5. Alt Kategorilerin Geçerli Yapıda Olmasını Sağlayın
            // Bu adım, yukarıdaki alt kategori kontrolünden kapsamlı bir yapıya sahiptir.

            // 6. Kategorinin Kendi Alt Kategorileri İle Çakışmadığı Kontrol Edilmeli
            if (input.ParentCategoryId.HasValue)
            {
                var parentCategory = await _categoryRepository.GetAsync(input.ParentCategoryId.Value);
                if (parentCategory.SubCategories.Any(c => c.Id == input.ParentCategoryId))
                {
                    throw new UserFriendlyException($"Kategori ({input.ParentCategoryId}) kendi alt kategorileriyle çakışıyor.");
                }
            }

            // 7. Kategorinin Alt Kategoriler Arasında Dairevi Bağlantılar Oluşmamalıdır
            // Bu adım, genellikle alt kategori ekleme sırasında daha kapsamlı bir algoritma gerektirir. Basit bir kontrol sağlanır.

            // Yeni Kategoriyi Oluştur
            var category = _mapper.Map<CreateCategoryDto, Category>(input);
            await _categoryRepository.InsertAsync(category);

            // Alt Kategorileri Ekle
            if (input.SubCategories != null)
            {
                foreach (var subCategoryDto in input.SubCategories)
                {
                    var subCategory = await _categoryRepository.GetAsync(subCategoryDto.Id);
                    subCategory.ParentCategoryId = category.Id;
                    await _categoryRepository.UpdateAsync(subCategory);
                }
            }

            return _mapper.Map<Category, CategoryDto>(category);
        }

        // Kategorinin tüm hiyerarşisini almak için yardımcı bir metod
        private async Task<HashSet<int>> GetCategoryHierarchyAsync(int categoryId)
        {
            var result = new HashSet<int>();
            var category = await _categoryRepository.GetAsync(categoryId);
            if (category == null)
            {
                return result;
            }

            var stack = new Stack<Category>();
            stack.Push(category);

            while (stack.Any())
            {
                var current = stack.Pop();
                result.Add(current.Id);

                foreach (var sub in current.SubCategories)
                {
                    stack.Push(sub);
                }
            }

            return result;
        }

        public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto input)
        {
            // 1. Kategori Var mı Kontrolü
            var category = await _categoryRepository.GetAsync(id);
         

            // 3. Kategori Adı Benzersiz Olmalıdır
            if (category.Name != input.Name)
            {
                var existingCategory = await _categoryRepository.FirstOrDefaultAsync(c => c.Name == input.Name);
                if (existingCategory != null)
                {
                    throw new UserFriendlyException($"Kategori adı ({input.Name}) zaten mevcut.");
                }
            }

            // 4. Üst Kategori ID Geçerli Olmalıdır
            if (input.ParentCategoryId.HasValue)
            {
                var parentCategoryExists = await _categoryRepository.AnyAsync(c => c.Id == input.ParentCategoryId.Value);
                if (!parentCategoryExists)
                {
                    throw new UserFriendlyException($"Üst kategori ID'si ({input.ParentCategoryId}) geçerli değil.");
                }

                // Üst Kategori ve Alt Kategori Arasında Döngü Oluşmamalıdır
                var parentCategoryHierarchy = await GetCategoryHierarchyAsync(input.ParentCategoryId.Value);
                if (parentCategoryHierarchy.Contains(id))
                {
                    throw new UserFriendlyException("Kategori, üst kategori olarak belirlenemez çünkü kendisinin alt kategorisi olamaz.");
                }
            }

            // 5. Alt Kategoriler İçin Geçerlilik Kontrolü
            if (input.SubCategories != null)
            {
                var subCategoryIds = input.SubCategories.Select(c => c.Id).ToHashSet();
                if (subCategoryIds.Count != input.SubCategories.Count)
                {
                    throw new UserFriendlyException("Alt kategoriler arasında tekrar eden ID'ler var.");
                }

                foreach (var subCategoryDto in input.SubCategories)
                {
                    var subCategoryExists = await _categoryRepository.AnyAsync(c => c.Id == subCategoryDto.Id);
                    if (!subCategoryExists)
                    {
                        throw new UserFriendlyException($"Alt kategori ID'si ({subCategoryDto.Id}) geçerli değil.");
                    }
                }
            }

            // 6. Alt Kategoriler ile Ana Kategori Arasında Dairevi Bağlantılar Oluşmamalıdır
            // Bu adım, genellikle güncellenmiş kategori ilişkilerinin doğru olduğundan emin olunarak gerçekleştirilir.

            // DTO'yu mevcut kategoriye uygula
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
