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
using Volo.Abp.Uow;
using Volo.CmsKit.Pages;

namespace CarpetProject.Categories
{
    public class CategoryManager : DomainService
    {
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly IRepository<Product, int> _productRepository;
        private readonly IRepository<Image, int> _ımageRepository;
        private readonly IRepository<CategoryProduct,int> _CategoryProductRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public CategoryManager(IRepository<Category, int> categoryRepository, IRepository<Product, int> productRepository, IRepository<Image, int> ımageRepository, IRepository<CategoryProduct, int> categoryProductRepository, IMapper mapper, IUnitOfWorkManager unitOfWorkManager)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _ımageRepository = ımageRepository;
            _CategoryProductRepository = categoryProductRepository;
            _mapper = mapper;
            _unitOfWorkManager = unitOfWorkManager;
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
            await _categoryRepository.InsertAsync(category);
            await _unitOfWorkManager.Current.SaveChangesAsync(); // ID'nin oluşturulması için
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

            // 5. Ürünleri Kontrol Et ve Benzersiz Olmalarını Sağla
            if (input.ProductIds != null && input.ProductIds.Any())
            {
                var productIds = input.ProductIds.Distinct().ToList(); // Ürün ID'lerini benzersiz yap
                var existingProducts = await _productRepository.GetListAsync(p => productIds.Contains(p.Id));
                var existingProductIds = existingProducts.Select(p => p.Id).ToList();

                // Girilen ürün ID'lerinden veritabanında mevcut olmayanları kontrol et
                var invalidProductIds = productIds.Except(existingProductIds).ToList();
                if (invalidProductIds.Any())
                {
                    throw new UserFriendlyException($"Aşağıdaki ürün ID'leri geçerli değil: {string.Join(", ", invalidProductIds)}");
                }

                // Tekrar eden ürün ID'lerini kontrol et
                var duplicateProductIds = productIds.GroupBy(id => id).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                if (duplicateProductIds.Any())
                {
                    throw new UserFriendlyException($"Aşağıdaki ürün ID'leri tekrar edilmiş: {string.Join(", ", duplicateProductIds)}");
                }

                // Geçerli ürünlerle ilişkileri güncelle
                foreach (var product in existingProducts)
                {
                    await _CategoryProductRepository.InsertAsync(new CategoryProduct
                    {
                        CategoryId = category.Id,
                        ProductId = product.Id
                    });
                }
            }

          

            // 7. DTO'ya Dönüştür ve Geri Döndür
            return _mapper.Map<Category, CategoryDto>(category);
        }

        public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto input)
        {
            // 1. Kategori Mevcut Olmalıdır
            var category = await _categoryRepository.GetAsync(id);
            if (category == null)
            {
                throw new UserFriendlyException($"Kategori ID'si ({id}) mevcut değil.");
            }

            // 2. Kategori Adı Benzersiz Olmalıdır
            var existingCategory = await _categoryRepository.FirstOrDefaultAsync(c => c.Name == input.Name && c.Id != id);
            if (existingCategory != null)
            {
                throw new UserFriendlyException($"Kategori adı ({input.Name}) zaten mevcut.");
            }

            // 3. Üst Kategori ID Geçerli Olmalıdır
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

                // Kategorinin kendisinin üst kategorisi olamayacağını kontrol et
                if (parentCategory.Id == id)
                {
                    throw new UserFriendlyException("Bir kategori kendi üst kategorisi olamaz.");
                }
            }

            // 4. Kategori Güncellemesi Yap
            _mapper.Map(input, category);

            // 5. Resim ID'si Geçerli Olmalıdır ve Kategori ID'si Güncellenmelidir
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

            // 6. Ürünleri Kontrol Et ve Benzersiz Olmalarını Sağla
            if (input.ProductIds != null && input.ProductIds.Any())
            {
                var productIds = input.ProductIds.Distinct().ToList(); // Ürün ID'lerini benzersiz yap
                var existingProducts = await _productRepository.GetListAsync(p => productIds.Contains(p.Id));
                var existingProductIds = existingProducts.Select(p => p.Id).ToList();

                // Girilen ürün ID'lerinden veritabanında mevcut olmayanları kontrol et
                var invalidProductIds = productIds.Except(existingProductIds).ToList();
                if (invalidProductIds.Any())
                {
                    throw new UserFriendlyException($"Aşağıdaki ürün ID'leri geçerli değil: {string.Join(", ", invalidProductIds)}");
                }

                // Tekrar eden ürün ID'lerini kontrol et
                var duplicateProductIds = productIds.GroupBy(id => id).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                if (duplicateProductIds.Any())
                {
                    throw new UserFriendlyException($"Aşağıdaki ürün ID'leri tekrar edilmiş: {string.Join(", ", duplicateProductIds)}");
                }

                // Kategori ile ilgili eski ürünleri sil
                await _CategoryProductRepository.DeleteAsync(cp => cp.CategoryId == id);

                // Geçerli ürünlerle ilişkileri güncelle
                foreach (var product in existingProducts)
                {
                    await _CategoryProductRepository.InsertAsync(new CategoryProduct
                    {
                        CategoryId = id,
                        ProductId = product.Id
                    });
                }
            }

            // 7. Kategoriyi Güncelle
            await _categoryRepository.UpdateAsync(category);

            // 8. DTO'ya Dönüştür ve Geri Döndür
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
            var hasActiveProducts = await _productRepository.AnyAsync(p => p.CategoryProducts.Any(cp => cp.CategoryId == category.Id) && p.IsApproved);
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
            // 1. Kategori ve ilişkili ürünlerin CategoryProduct tablosu üzerinden getirilmesi
            var category = await _categoryRepository.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted && c.IsApproved);
            if (category == null)
            {
                throw new UserFriendlyException($"ID = {id} olan kategori bulunamadı veya silinmiş/onaylanmamış.");
            }

            // 2. Ara tablo CategoryProduct üzerinden ilişkili ürünlerin getirilmesi
            var categoryProducts = await _CategoryProductRepository.GetListAsync(cp => cp.CategoryId == id);

            // 3. Onaylı ve silinmemiş ürünleri filtreleyin
            var productIds = categoryProducts.Select(cp => cp.ProductId).Distinct().ToList();
            var approvedProducts = await _productRepository.GetListAsync(p =>
                productIds.Contains(p.Id) && !p.IsDeleted && p.IsApproved
            );

            // 4. DTO'ya dönüştür
            var categoryDto = _mapper.Map<Category, CategoryDto>(category);
            categoryDto.Products = _mapper.Map<List<Product>, List<ProductDto>>(approvedProducts);

            return categoryDto;
        }

        public async Task<PagedResultDto<CategoryDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            // 1. Onaylı ve silinmemiş kategorileri getir
            var queryable = await _categoryRepository.GetQueryableAsync();
            queryable = queryable.Where(p => !p.IsDeleted && p.IsApproved);

            // 2. Toplam sayıyı al
            var totalCount = await AsyncExecuter.CountAsync(queryable);

            // 3. Kategorileri sıralama ve sayfalama işlemleri
            var categories = await AsyncExecuter.ToListAsync(
                queryable.OrderBy(a => a.Name)
                         .Skip(input.SkipCount)
                         .Take(input.MaxResultCount)
            );

            // 4. Kategorilere ait ürünleri ara tablo (CategoryProduct) üzerinden getirme
            var categoryIds = categories.Select(c => c.Id).ToList();
            var categoryProducts = await _CategoryProductRepository.GetListAsync(cp => categoryIds.Contains(cp.CategoryId));

            // 5. Kategorilere ait ürünleri çekip DTO'ya dönüştür
            var productIds = categoryProducts.Select(cp => cp.ProductId).Distinct().ToList();
            var products = await _productRepository.GetListAsync(p => productIds.Contains(p.Id) && !p.IsDeleted && p.IsApproved);

            // 6. DTO'ya dönüştürme
            var categoryDtos = _mapper.Map<List<Category>, List<CategoryDto>>(categories);

            // 7. Her kategoriye ait ürünleri ilgili DTO'ya ekleme
            foreach (var categoryDto in categoryDtos)
            {
                var relatedProductIds = categoryProducts
                    .Where(cp => cp.CategoryId == categoryDto.Id)
                    .Select(cp => cp.ProductId)
                    .ToList();

                categoryDto.Products = _mapper.Map<List<Product>, List<ProductDto>>(
                    products.Where(p => relatedProductIds.Contains(p.Id)).ToList()
                );
            }

            // 8. Sayfalama sonucunu döndür
            return new PagedResultDto<CategoryDto>(
                totalCount,
                categoryDtos
            );
        }
    }
}
