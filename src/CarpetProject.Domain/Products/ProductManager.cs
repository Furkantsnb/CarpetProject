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

namespace CarpetProject.Products
{
    public class ProductManager : DomainService
    {
        private readonly IRepository<Product, int> _productRepository;
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly IRepository<Image, int> _productImageRepository;
        private readonly IMapper _mapper;

        public ProductManager(IRepository<Product, int> productRepository, IRepository<Category, int> categoryRepository, IRepository<Image, int> productImage, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productImageRepository = productImage;
            _mapper = mapper;
        }


        public async Task<ProductDto> CreateAsync(CreateProductDto input)
        {
            // Aynı isimde başka bir ürün olup olmadığını kontrol et
            var isProductNameExists = await _productRepository.AnyAsync(p => p.Name == input.Name);
            if (isProductNameExists)
            {
                throw new UserFriendlyException($"Bu ürün ismi ({input.Name}) zaten başka bir ürün tarafından kullanılmıştır.");
            }

            // Kategori ID'lerinin geçerliliğini kontrol et
            if (input.CategoryIds != null)
            {
                foreach (var categoryId in input.CategoryIds)
                {
                    var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == categoryId);
                    if (!categoryExists)
                    {
                        throw new UserFriendlyException($"Kategori ID'si ({categoryId}) geçerli değil.");
                    }
                }
            }
            // DTO'yu ürün entity'sine dönüştürün
            var product = _mapper.Map<CreateProductDto, Product>(input);

            // Ürünü veritabanına ekleyin
            await _productRepository.InsertAsync(product);

            // Resim ID'lerinin geçerliliğini kontrol et ve ProductId güncellemesini yap
            if (input.ImageIds != null)
            {
                foreach (var imageId in input.ImageIds)
                {
                    var image = await _productImageRepository.FirstOrDefaultAsync(i => i.Id == imageId);
                    if (image == null)
                    {
                        throw new UserFriendlyException($"Resim ID'si ({imageId}) geçerli değil.");
                    }

                    if (image.ProductId.HasValue)
                    {
                        throw new UserFriendlyException($"Resim ID'si ({imageId}) zaten bir ürüne atanmış.");
                    }

                    // ProductId'yi güncelle
                    image.ProductId = product.Id;
                    await _productImageRepository.UpdateAsync(image);
                }
            }
            // Eklenen ürünü DTO olarak geri döndürün
            return _mapper.Map<Product, ProductDto>(product);
        }

        public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto input)
        {
            // İlgili ID'ye sahip ürünü getir
            var product = await _productRepository.GetAsync(id);

            // Güncellenmek istenen ürün adının benzersiz olup olmadığını kontrol et
            var isProductNameUnique = await _productRepository.AnyAsync(p => p.Name == input.Name && p.Id != id);
            if (isProductNameUnique)
            {
                throw new UserFriendlyException($"Güncellenmek istenen ürün ismi ({input.Name}) adında başka bir ürün bulunmaktadır.");
            }

            // Kategori ID'lerinin geçerliliğini kontrol et
            if (input.CategoryIds != null)
            {
                foreach (var categoryId in input.CategoryIds)
                {
                    var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == categoryId);
                    if (!categoryExists)
                    {
                        throw new UserFriendlyException($"Kategori ID'si ({categoryId}) geçerli değil.");
                    }
                }
            }

            // Resim ID'lerinin geçerliliğini kontrol et ve ProductId güncellemesini yap
            if (input.ImageIds != null)
            {
                foreach (var imageId in input.ImageIds)
                {
                    var image = await _productImageRepository.FirstOrDefaultAsync(i => i.Id == imageId);
                    if (image == null)
                    {
                        throw new UserFriendlyException($"Resim ID'si ({imageId}) geçerli değil.");
                    }

                    if (image.ProductId.HasValue && image.ProductId != id)
                    {
                        throw new UserFriendlyException($"Resim ID'si ({imageId}) zaten başka bir ürüne atanmış.");
                    }

                    // Eğer ProductId null ise, resmin ProductId değerini güncelle
                    if (image.ProductId == null)
                    {
                        image.ProductId = product.Id;
                        await _productImageRepository.UpdateAsync(image);
                    }
                }
            }

            // Ürün bilgilerini güncelle
            _mapper.Map(input, product);
            await _productRepository.UpdateAsync(product);

            // Güncellenmiş ürünü DTO olarak geri döndür
            return _mapper.Map<Product, ProductDto>(product);
        }

        public async Task DeleteAsync(int id)
        {
            // id sahip ürünü getirir
            var product = await _productRepository.GetAsync(id);
            product.IsDeleted = true;
            await _productRepository.UpdateAsync(product); // Silme işlemi güncelleme olarak işaretlenir
        }

        public async Task<ProductDto> GetAsync(int id)
        {
            // Ürünü ve ilişkili resimleri dahil ederek getir
            var productQuery = await _productRepository.WithDetailsAsync(p => p.Images);

            var product = productQuery.FirstOrDefault(p => p.Id == id && !p.IsDeleted && p.IsApproved);

            //Ürünün mevcut olup olmadığını ve geçerli olup olmadığını kontrol et
            var products = await _productRepository.FirstOrDefaultAsync(p => !p.IsDeleted && p.IsApproved);

            if (products == null)
            {
                throw new UserFriendlyException($"ID = {id} olan ürün silinmiş/onaylanmamış.");
            }
            return _mapper.Map<Product, ProductDto>(product);
        }

   

        public async Task<PagedResultDto<ProductDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {

            // 1. Sorgu oluştur ve ilişkili resimleri dahil et
            var queryable = await _productRepository.WithDetailsAsync(p => p.Images);

            // 2. Yumuşak silinmiş ürünleri hariç tut
            queryable = queryable.Where(p => p.IsDeleted == false);

            // 3. Onaylı ürünleri listele
            queryable = queryable.Where(p => p.IsApproved);


            // 4. Sıralama
            if (!string.IsNullOrEmpty(input.Sorting))
            {
                queryable = input.Sorting switch
                {
                    "NameAsc" => queryable.OrderBy(p => p.Name),
                    "NameDesc" => queryable.OrderByDescending(p => p.Name),
                    "PriceAsc" => queryable.OrderBy(p => p.Price),
                    "PriceDesc" => queryable.OrderByDescending(p => p.Price),
                    "DiscountAsc" => queryable.OrderBy(p => p.HasDiscount), // İndirim durumuna göre artan sıralama
                    "DiscountDesc" => queryable.OrderByDescending(p => p.HasDiscount), // İndirim durumuna göre azalan sıralama
                    "CertificationAsc" => queryable.OrderBy(p => p.Certification), // Sertifika durumuna göre artan sıralama
                    "CertificationDesc" => queryable.OrderByDescending(p => p.Certification), // Sertifika durumuna göre azalan sıralama
                    _ => queryable.OrderBy(p => p.Name) // Varsayılan sıralama
                };
            }

            // 5. Toplam sayıyı al
            var totalCount = await AsyncExecuter.CountAsync(queryable);

            // 6. Veriyi çek
            var lines = await AsyncExecuter.ToListAsync(
                queryable
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
            );

            // 7. DTO'ya dönüştür
            var productDtos = _mapper.Map<List<Product>, List<ProductDto>>(lines);

            return new PagedResultDto<ProductDto>(
                totalCount,
                productDtos
            );
        }

    }
}
