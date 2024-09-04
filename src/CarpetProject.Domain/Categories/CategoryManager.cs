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
using Volo.CmsKit.Pages;

namespace CarpetProject.Categories
{
    public class CategoryManager : DomainService
    {
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly IRepository<Product, int> _productRepository;
        private readonly IRepository<Image, int> _ımageRepository;
        private readonly IMapper _mapper;

        public CategoryManager(IRepository<Category, int> categoryRepository, IRepository<Product, int> productRepository, IRepository<Image, int> ımageRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _ımageRepository = ımageRepository;
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
                var parentCategory = await _categoryRepository.GetAsync(input.ParentCategoryId.Value);
                if (parentCategory == null)
                {
                    throw new UserFriendlyException($"Üst kategori ID'si ({input.ParentCategoryId}) geçerli değil.");
                }

                // Alt kategorilerin alt kategorilere sahip olmaması gerektiğini kontrol et
                var parentCategoryIsChild = parentCategory.ParentCategoryId.HasValue;
                if (parentCategoryIsChild)
                {
                    throw new UserFriendlyException("Bir kategori, bir alt kategorinin alt kategorisi olamaz.");
                }
            }

            // 3. Yeni Kategoriyi Oluştur
            var category = _mapper.Map<CreateCategoryDto, Category>(input);

            // 4. Resim ID'si Geçerli Olmalıdır ve Kategori ID'si Güncellenmelidir
            if (input.ImageId.HasValue)
            {
                var image = await _ımageRepository.FirstOrDefaultAsync(i => i.Id == input.ImageId.Value);
                if (image == null)
                {
                    throw new UserFriendlyException($"Resim ID'si ({input.ImageId}) geçerli değil.");
                }

                if (image.CategoryId.HasValue)
                {
                    throw new UserFriendlyException($"Resim ID'si ({input.ImageId}) zaten başka bir kategoriye atanmış.");
                }

                // Resmin kategori ID'sini güncelle
                image.CategoryId = category.Id;
                await _ımageRepository.UpdateAsync(image);
            }


            // 5. Ürün Listesini Güncelle
            if (input.Products != null && input.Products.Any())
            {
                var productIds = input.Products.Distinct().ToList(); // Ürün ID'lerini benzersiz yap
                var products = await _productRepository.GetListAsync(p => productIds.Contains(p.Id));

                foreach (var product in products)
                {
                    if (product.Categories == null)
                    {
                        product.Categories = new HashSet<Category>();
                    }

                    if (!product.Categories.Any(c => c.Id == category.Id))
                    {
                        product.Categories.Add(category);
                    }
                }

                // Ürünleri topluca güncelle
                if (products.Any())
                {
                    await _productRepository.UpdateManyAsync(products);
                }
            }

            // 6. Yeni Kategoriyi Oluştur
            // Ürünleri kategoriye eklemek yerine, bu adımda sadece kategoriyi veritabanına ekliyoruz
            await _categoryRepository.InsertAsync(category);

            // 7. DTO'ya Dönüştür ve Geri Döndür
            return _mapper.Map<Category, CategoryDto>(category);
        }

        public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto input)
        {
            // 1. Kategori Var mı Kontrolü
            var category = await _categoryRepository.GetAsync(id);

            // 2. Kategori Adı Benzersiz Olmalıdır
            if (category.Name != input.Name)
            {
                var existingCategory = await _categoryRepository.FirstOrDefaultAsync(c => c.Name == input.Name);
                if (existingCategory != null)
                {
                    throw new UserFriendlyException($"Kategori adı ({input.Name}) zaten mevcut.");
                }
            }

            // 3. Üst Kategori ID Geçerli Olmalıdır
            if (input.ParentCategoryId.HasValue)
            {
                var parentCategory = await _categoryRepository.GetAsync(input.ParentCategoryId.Value);
                if (parentCategory == null)
                {
                    throw new UserFriendlyException($"Üst kategori ID'si ({input.ParentCategoryId}) geçerli değil.");
                }

                // Güncellenmiş kategori, üst kategorinin alt kategorisi olmamalıdır
                if (parentCategory.Id == id)
                {
                    throw new UserFriendlyException("Kategori, kendisinin alt kategorisi olamaz.");
                }

                // Alt kategorilerin alt kategorilere sahip olmaması gerektiğini kontrol et
                var parentCategoryIsChild = parentCategory.ParentCategoryId.HasValue;
                if (parentCategoryIsChild)
                {
                    throw new UserFriendlyException("Bir kategori, bir alt kategorinin alt kategorisi olamaz.");
                }
            }

            // 4. Resim ID'si Geçerli Olmalıdır ve Kategori ID'si Güncellenmelidir
            if (input.ImageId.HasValue)
            {
                var image = await _ımageRepository.FirstOrDefaultAsync(i => i.Id == input.ImageId.Value);
                if (image == null)
                {
                    throw new UserFriendlyException($"Resim ID'si ({input.ImageId}) geçerli değil.");
                }

                if (image.CategoryId.HasValue && image.CategoryId != id)
                {
                    throw new UserFriendlyException($"Resim ID'si ({input.ImageId}) zaten başka bir kategoriye atanmış.");
                }

                // Resmin kategori ID'sini güncelle
                image.CategoryId = id;
                await _ımageRepository.UpdateAsync(image);
            }


            // 5. Ürün Listesini Güncelle
            if (input.Products != null && input.Products.Any())
            {
                var productIds = input.Products.Distinct().ToList(); // Ürün ID'lerini benzersiz yap
                var products = await _productRepository.GetListAsync(p => productIds.Contains(p.Id));

                foreach (var product in products)
                {
                    if (product.Categories == null)
                    {
                        product.Categories = new HashSet<Category>();
                    }

                    if (!product.Categories.Any(c => c.Id == category.Id))
                    {
                        product.Categories.Add(category);
                    }
                }

                // Ürünleri topluca güncelle
                if (products.Any())
                {
                    await _productRepository.UpdateManyAsync(products);
                }
            }

            // 6. DTO'yu mevcut kategoriye uygula
            _mapper.Map(input, category);
            await _categoryRepository.UpdateAsync(category);

            // 7. DTO'ya Dönüştür ve Geri Döndür
            return _mapper.Map<Category, CategoryDto>(category);
        }

        public async Task DeleteAsync(int id)
        {
            // 1. Kategori Var mı Kontrolü
            var category = await _categoryRepository.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                throw new UserFriendlyException($"Kategori ID'si ({id}) geçerli değil.");
            }

            // 2. Kategorinin Alt Kategorileri Kontrol Ediliyor
            var subCategories = await _categoryRepository.GetListAsync(c => c.ParentCategoryId == id);
            if (subCategories.Any())
            {
                throw new UserFriendlyException("Kategori alt kategorilere sahip. Lütfen önce alt kategorileri silin veya başka bir kategoriye taşıyın.");
            }

            // 3. Kategorinin Aktif Ürünlerle İlişkili Olup Olmadığını Kontrol Et
            var hasActiveProducts = await _productRepository.AnyAsync(p => p.Categories.Any(c => c.Id == category.Id) && p.IsApproved);
            if (hasActiveProducts)
            {
                throw new UserFriendlyException("Aktif ürünlere sahip bir kategori silinemez. Lütfen önce ilgili ürünleri başka bir kategoriye taşıyın veya devre dışı bırakın.");
            }

            // 4. Kategorinin İlişkili Resimlerini Güncelle
            var images = await _ımageRepository.GetListAsync(i => i.CategoryId == id);
            foreach (var image in images)
            {
                image.CategoryId = null; // Kategori ID'sini boşalt
                await _ımageRepository.UpdateAsync(image);
            }

            // 5. Kategori Soft Delete ile Silinir
            category.IsDeleted = true;


            await _categoryRepository.UpdateAsync(category);
        }

        public async Task<CategoryDto> GetAsync(int id)
        {
            // Category ve ilişkili ürünleri dahil ederek getir
            var CategoryQuery = await _categoryRepository.WithDetailsAsync(p => p.Products);

            var category = CategoryQuery.FirstOrDefault(p => p.Id == id && !p.IsDeleted && p.IsApproved);

            if (category == null)
            {
                throw new UserFriendlyException($"ID = {id} olan kategori silinmiş/onaylanmamış.");
            }

            // 1. Onaylı ve Silinmemiş Ürünleri Listele
            var approvedProducts = category.Products
                .Where(p => !p.IsDeleted && p.IsApproved)
                .Select(p => p.Id) // Ürün ID'lerini al
                .ToList();

            // 2. DTO'ya dönüştür
            var categoryDto = _mapper.Map<Category, CategoryDto>(category);

            // 3. Ürün ID'lerini DTO'daki Products listesine ekle
            categoryDto.Products = approvedProducts;

            return categoryDto;
        }

        public async Task<PagedResultDto<CategoryDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var queryable = await _categoryRepository.WithDetailsAsync(c => c.Products);

            // 2. Yumuşak silinmiş kategorileri hariç tut
            queryable = queryable.Where(p => !p.IsDeleted);

            // 3. Onaylı kategorileri listele
            queryable = queryable.Where(p => p.IsApproved);

            var totalCount = await AsyncExecuter.CountAsync(queryable);

            var categories = await AsyncExecuter.ToListAsync(
                queryable.OrderBy(a => a.Name)
                         .Skip(input.SkipCount)
                         .Take(input.MaxResultCount)
            );

            // Kategorilerin DTO'ya dönüştürülmesi
            var categoryDtos = _mapper.Map<List<Category>, List<CategoryDto>>(categories);

            // Her bir kategori için ürün ID'lerini filtreleyip DTO'ya ekleyin
            foreach (var categoryDto in categoryDtos)
            {
                var category = categories.FirstOrDefault(c => c.Id == categoryDto.Id);
                if (category != null)
                {
                    // Onaylı ve silinmemiş ürünlerin ID'lerini alın
                    var approvedProductIds = category.Products
                                                     .Where(p => !p.IsDeleted && p.IsApproved)
                                                     .Select(p => p.Id)
                                                     .ToList();

                    // DTO'nun Products özelliğine bu ID'leri ekleyin
                    categoryDto.Products = approvedProductIds;
                }
            }

            return new PagedResultDto<CategoryDto>(
                totalCount,
                categoryDtos
            );
        }
    }
}
