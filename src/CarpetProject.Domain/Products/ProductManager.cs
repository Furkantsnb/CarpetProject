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
using CarpetProject.Entities.Products;

using Volo.Abp;
using CarpetProject.Entities.Categories;
using CarpetProject.EntityDto.ProductImages;

namespace CarpetProject.Products
{
    public class ProductManager : DomainService
    {
        private readonly IRepository<Product, int> _productRepository;
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly IRepository<Image, int> _productImageRepository;
        private readonly IRepository<CategoryProduct, int> _CategoryProductRepository;
        private readonly IMapper _mapper;

        public ProductManager(IRepository<Product, int> productRepository, IRepository<Category, int> categoryRepository, IRepository<Image, int> productImageRepository, IRepository<CategoryProduct, int> categoryProductRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productImageRepository = productImageRepository;
            _CategoryProductRepository = categoryProductRepository;
            _mapper = mapper;
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto input)
        {

            // 1. Aynı isimde başka bir ürün olup olmadığını kontrol et
            var isProductNameExists = await _productRepository.AnyAsync(p => p.Name == input.Name);
            if (isProductNameExists)
            {
                throw new UserFriendlyException($"Bu ürün ismi ({input.Name}) zaten başka bir ürün tarafından kullanılmıştır.");
            }

            // 2. DTO'yu ürün entity'sine dönüştürün
            var product = _mapper.Map<CreateProductDto, Product>(input);

            // 3. Ürünü veritabanına ekleyin
            await _productRepository.InsertAsync(product);

            // 4. Kategori ID'lerinin geçerliliğini kontrol et ve CategoryProduct ilişkisini kur
            if (input.CategoryIds != null && input.CategoryIds.Any())
            {
                var categoryProducts = new List<CategoryProduct>();

                foreach (var categoryId in input.CategoryIds)
                {
                    var category = await _categoryRepository.FirstOrDefaultAsync(c => c.Id == categoryId);
                    if (category == null)
                    {
                        throw new UserFriendlyException($"Kategori ID'si ({categoryId}) geçerli değil.");
                    }

                    // CategoryProduct ilişkisini oluştur ve listeye ekle
                    categoryProducts.Add(new CategoryProduct
                    {
                        CategoryId = category.Id,
                        ProductId = product.Id
                    });
                }

                // Toplu olarak CategoryProduct ilişkilerini ekle
                await _CategoryProductRepository.InsertManyAsync(categoryProducts);
            }

            // 5. Resim ID'lerinin geçerliliğini kontrol et ve ProductId güncellemesini yap
            if (input.ImageIds != null && input.ImageIds.Any())
            {
                var images = await _productImageRepository.GetListAsync(i => input.ImageIds.Contains(i.Id));

                if (images.Count != input.ImageIds.Count)
                {
                    throw new UserFriendlyException("Geçersiz resim ID'leri var.");
                }

                foreach (var image in images)
                {
                    if (image.ProductId.HasValue)
                    {
                        throw new UserFriendlyException($"Resim ID'si ({image.Id}) zaten bir ürüne atanmış.");
                    }

                    // ProductId'yi güncelle
                    image.ProductId = product.Id;
                    await _productImageRepository.UpdateAsync(image);
                }
            }

            // 6. Eklenen ürünü DTO olarak geri döndürün
            return _mapper.Map<Product, ProductDto>(product);
        }

        public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto input)
        {
            // 1. İlgili ID'ye sahip ürünü getir
            var product = await _productRepository.GetAsync(id);

            // 2. Güncellenmek istenen ürün adının benzersiz olup olmadığını kontrol et
            var isProductNameUnique = await _productRepository.AnyAsync(p => p.Name == input.Name && p.Id != id);
            if (isProductNameUnique)
            {
                throw new UserFriendlyException($"Güncellenmek istenen ürün ismi ({input.Name}) adında başka bir ürün bulunmaktadır.");
            }

            // 3. Eski kategori ilişkilerini toplu halde temizle
            var existingCategoryProducts = await _CategoryProductRepository.GetListAsync(cp => cp.ProductId == id);
            if (existingCategoryProducts.Any())
            {
                await _CategoryProductRepository.DeleteManyAsync(existingCategoryProducts);
            }

            // 4. Yeni kategori ID'lerinin geçerliliğini kontrol et ve ara tabloya ekle
            if (input.CategoryIds != null && input.CategoryIds.Any())
            {
                var categoryProducts = new List<CategoryProduct>();

                foreach (var categoryId in input.CategoryIds)
                {
                    var category = await _categoryRepository.FirstOrDefaultAsync(c => c.Id == categoryId);
                    if (category == null)
                    {
                        throw new UserFriendlyException($"Kategori ID'si ({categoryId}) geçerli değil.");
                    }

                    // CategoryProduct ilişkisini oluştur ve listeye ekle
                    categoryProducts.Add(new CategoryProduct
                    {
                        CategoryId = category.Id,
                        ProductId = product.Id
                    });
                }

                // Yeni kategori ilişkilerini toplu halde ekle
                await _CategoryProductRepository.InsertManyAsync(categoryProducts);
            }

            // 5. Resim ID'lerinin geçerliliğini kontrol et ve ProductId güncellemesini yap
            if (input.ImageIds != null && input.ImageIds.Any())
            {
                var images = await _productImageRepository.GetListAsync(i => input.ImageIds.Contains(i.Id));

                if (images.Count != input.ImageIds.Count)
                {
                    throw new UserFriendlyException("Geçersiz resim ID'leri var.");
                }

                foreach (var image in images)
                {
                    if (image.ProductId.HasValue && image.ProductId != id)
                    {
                        throw new UserFriendlyException($"Resim ID'si ({image.Id}) zaten başka bir ürüne atanmış.");
                    }

                    // Eğer ProductId null ise, resmin ProductId değerini güncelle
                    if (!image.ProductId.HasValue)
                    {
                        image.ProductId = product.Id;
                        await _productImageRepository.UpdateAsync(image);
                    }
                }
            }

            // 6. Ürün bilgilerini güncelle
            _mapper.Map(input, product);
            await _productRepository.UpdateAsync(product);

            // 7. Güncellenmiş ürünü DTO olarak geri döndür
            return _mapper.Map<Product, ProductDto>(product);
        }


        public async Task DeleteAsync(int id)
        {
            // 1. İlgili ID'ye sahip ürünü getir
            var product = await _productRepository.GetAsync(id);

            if (product == null)
            {
                throw new UserFriendlyException($"Ürün ID'si ({id}) geçerli değil.");
            }

            // 2. Soft delete işlemi
            await _productRepository.DeleteAsync(product);
        }

        public async Task<ProductDto> GetAsync(int id)
        {
            // 1. Ürünü ilişkili resimler ve kategorilerle birlikte getir
            var product = await _productRepository
                .WithDetailsAsync(p => p.Images, p => p.CategoryProducts)
                .ConfigureAwait(false);

            // 2. ID'ye göre ürünü bul ve silinmiş/onaylanmamış olup olmadığını kontrol et
            var productEntity = product
                .FirstOrDefault(p => p.Id == id && !p.IsDeleted && p.IsApproved);

            if (productEntity == null)
            {
                throw new UserFriendlyException($"ID = {id} olan ürün bulunamadı veya silinmiş/onaylanmamış.");
            }

            // 3. Kategori ürünlerinin ID'lerini al
            var categoryIds = productEntity.CategoryProducts.Select(cp => cp.CategoryId).ToList();
            var categories = await _categoryRepository
                .GetListAsync(c => categoryIds.Contains(c.Id) && !c.IsDeleted && c.IsApproved);

            // 4. Resimleri DTO'ya dönüştür
            var imageDtos = productEntity.Images
                .Where(i => !i.IsDeleted)
                .Select(i => _mapper.Map<Image, ImageDto>(i)) // Mapping işlemi
                .ToList();

            // 5. Kategorileri DTO'ya dönüştür
            var categoryDtos = categories
                .Where(c => !c.IsDeleted)
                .Select(c => _mapper.Map<Category, CategoryDto>(c)) // Mapping işlemi
                .ToList();

            // 6. Ürünü DTO'ya dönüştür
            var productDto = _mapper.Map<Product, ProductDto>(productEntity);

            // 7. Resim ve kategorileri DTO'ya ekle
            productDto.Images = imageDtos;
            productDto.Categories = categoryDtos;

            return productDto;
        }

   

        public async Task<PagedResultDto<ProductDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            // 1. Sorgu oluştur ve ilişkili resimler ile kategorileri dahil et
            var queryable = await _productRepository.WithDetailsAsync(p => p.Images, p => p.CategoryProducts);

            // 2. Yumuşak silinmiş ürünleri hariç tut
            queryable = queryable.Where(p => !p.IsDeleted);

            // 3. Onaylı ürünleri listele
            queryable = queryable.Where(p => p.IsApproved);

            // 4. Sıralama işlemi
            if (!string.IsNullOrEmpty(input.Sorting))
            {
                queryable = input.Sorting switch
                {
                    "NameAsc" => queryable.OrderBy(p => p.Name),
                    "NameDesc" => queryable.OrderByDescending(p => p.Name),
                    "PriceAsc" => queryable.OrderBy(p => p.Price),
                    "PriceDesc" => queryable.OrderByDescending(p => p.Price),
                    "DiscountAsc" => queryable.OrderBy(p => p.HasDiscount),
                    "DiscountDesc" => queryable.OrderByDescending(p => p.HasDiscount),
                    "CertificationAsc" => queryable.OrderBy(p => p.Certification),
                    "CertificationDesc" => queryable.OrderByDescending(p => p.Certification),
                    _ => queryable.OrderBy(p => p.Name)
                };
            }

            // 5. Toplam sayıyı al
            var totalCount = await AsyncExecuter.CountAsync(queryable);

            // 6. Veriyi çek
            var products = await AsyncExecuter.ToListAsync(
                queryable
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
            );

            // 7. DTO'ya dönüştür ve ilişkili kategorileri ve resimleri yükle
            var productDtos = new List<ProductDto>();

            foreach (var product in products)
            {
                var productDto = _mapper.Map<Product, ProductDto>(product);

                // Kategori ID'lerini al
                var categoryIds = product.CategoryProducts.Select(cp => cp.CategoryId).ToList();
                var categories = await _categoryRepository.GetListAsync(c => categoryIds.Contains(c.Id) && !c.IsDeleted && c.IsApproved);

                // Kategorileri DTO'ya dönüştür
                var categoryDtos = categories.Select(c => _mapper.Map<Category, CategoryDto>(c)).ToList();

                // DTO'ya kategorileri ekle
                productDto.Categories = categoryDtos;

                // Resimlerin DTO'ya eklenmesi
                var imageDtos = product.Images
                    .Where(i => !i.IsDeleted) // Silinmiş resimleri dahil etme
                    .Select(i => _mapper.Map<Image, ImageDto>(i))
                    .ToList();

                productDto.Images = imageDtos; // DTO'ya resimleri ekle

                productDtos.Add(productDto);
            }

            // 8. Sonuçları PagedResultDto olarak geri döndür
            return new PagedResultDto<ProductDto>(
                totalCount,
                productDtos
            );
        }

    }
}
