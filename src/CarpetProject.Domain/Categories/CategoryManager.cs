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
        private readonly IRepository<Product, int> _productRepository;
        private readonly IMapper _mapper;

        public CategoryManager(IRepository<Category, int> categoryRepository, IRepository<Product, int> productRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto input)
        {
            // 1. Kategori Adı Benzersiz Olmalıdır
            var existingCategory = await _categoryRepository.FirstOrDefaultAsync(c => c.Name == input.Name);
            if (existingCategory != null)
            {
                throw new UserFriendlyException($"Kategori adı ({input.Name}) zaten mevcut.");
            }

            // 2. Üst Kategori ID Geçerli Olmalıdır
            if (input.ParentCategoryId.HasValue)
            {
                var parentCategoryExists = await _categoryRepository.AnyAsync(c => c.Id == input.ParentCategoryId.Value);
                if (!parentCategoryExists)
                {
                    throw new UserFriendlyException($"Üst kategori ID'si ({input.ParentCategoryId}) geçerli değil.");
                }
            }

            // 3. Alt Kategoriler İçin Kontrol
            if (input.SubCategories != null && input.SubCategories.Any())
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

                    // Alt kategorilerin döngüsel ilişkiler oluşturmadığından emin olun
                    var subCategoryHierarchy = await GetCategoryHierarchyAsync(subCategory.Id);
                    if (subCategoryHierarchy.Contains(input.ParentCategoryId.GetValueOrDefault()))
                    {
                        throw new UserFriendlyException($"Alt kategori ID'si ({subCategory.Id}) kendi üst kategorisinin alt kategorisi olamaz.");
                    }
                }
            }

            // 4. Kategorinin Kendi Alt Kategorileri İle Çakışmadığı Kontrol Edilmeli
            if (input.ParentCategoryId.HasValue)
            {
                var parentCategory = await _categoryRepository.GetAsync(input.ParentCategoryId.Value);
                if (parentCategory.SubCategories.Any(c => c.Id == input.ParentCategoryId))
                {
                    throw new UserFriendlyException($"Kategori ({input.ParentCategoryId}) kendi alt kategorileriyle çakışıyor.");
                }
            }

            // 5. Yeni Kategoriyi Oluştur
            var category = _mapper.Map<CreateCategoryDto, Category>(input);
            await _categoryRepository.InsertAsync(category);

            // 6. Alt Kategorileri Ekle
            if (input.SubCategories != null && input.SubCategories.Any())
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
            if (input.SubCategories != null && input.SubCategories.Any())
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

                    // Alt Kategoriler ile Ana Kategori Arasında Döngüsel Bağlantı Oluşmamalıdır
                    var subCategoryHierarchy = await GetCategoryHierarchyAsync(subCategoryDto.Id);
                    if (subCategoryHierarchy.Contains(id))
                    {
                        throw new UserFriendlyException($"Alt kategori ID'si ({subCategoryDto.Id}) kendi üst kategorisinin alt kategorisi olamaz.");
                    }
                }
            }

            // DTO'yu mevcut kategoriye uygula
            _mapper.Map(input, category);
            await _categoryRepository.UpdateAsync(category);

            return _mapper.Map<Category, CategoryDto>(category);
        }

        public async Task DeleteAsync(int id)
        {
            // 1. Kategori Var mı Kontrolü
            var category = await _categoryRepository.GetAsync(id);
         
            // 2. Alt Kategorilerin Olup Olmadığını Kontrol Et
            if (category.SubCategories != null && category.SubCategories.Any())
            {
                throw new UserFriendlyException("Alt kategorilere sahip bir kategori silinemez. Lütfen önce alt kategorileri silin veya başka bir kategoriye taşıyın.");
            }

            // 3. Kategorinin Aktif Ürünlerle İlişkili Olup Olmadığını Kontrol Et
            var hasActiveProducts = await _productRepository.AnyAsync(p => p.Categories.Any(c => c.Id == category.Id) && p.IsApproved);
            if (hasActiveProducts)
            {
                throw new UserFriendlyException("Aktif ürünlere sahip bir kategori silinemez. Lütfen önce ilgili ürünleri başka bir kategoriye taşıyın veya devre dışı bırakın.");
            }
            category.IsDeleted = true;
            // 4. Kategori Soft Delete ile Silinir
            await _categoryRepository.UpdateAsync(category);
        }

        public async Task<CategoryDto> GetAsync(int id)
        {
            var category = await _categoryRepository.GetAsync(id);
            // 1. Kategori Var mı, Silinmemiş mi ve Onaylı mı Kontrolü

            //Ürünün mevcut olup olmadığını ve geçerli olup olmadığını kontrol et
            var Categorys = await _categoryRepository.FirstOrDefaultAsync(p => !p.IsDeleted && p.IsApproved);

            if (Categorys == null)
            {
                throw new UserFriendlyException($"ID = {id} olan ürün silinmiş/onaylanmamış.");
            }
            return _mapper.Map<Category, CategoryDto>(category);
        }

        public async Task<PagedResultDto<CategoryDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var queryable = await _categoryRepository.GetQueryableAsync();

            // 2. Yumuşak silinmiş ürünleri hariç tut
            queryable = queryable.Where(p => p.IsDeleted == false);

            // 3. Onaylı ürünleri listele
            queryable = queryable.Where(p => p.IsApproved);

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
