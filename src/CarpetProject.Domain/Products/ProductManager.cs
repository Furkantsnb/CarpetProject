﻿using AutoMapper;
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

            // 3. Ürünü veritabanına ekleyin ve Id'sini alın (Ürün önce eklenmeli ki ID'ye sahip olsun)
            await _productRepository.InsertAsync(product);

            // 4. Kategori ID'lerinin geçerliliğini kontrol et ve CategoryProduct ilişkisini kur
            if (input.CategoryIds != null && input.CategoryIds.Any())
            {
                foreach (var categoryId in input.CategoryIds)
                {
                    var category = await _categoryRepository.FirstOrDefaultAsync(c => c.Id == categoryId);
                    if (category == null)
                    {
                        throw new UserFriendlyException($"Kategori ID'si ({categoryId}) geçerli değil.");
                    }

                    // CategoryProduct ilişkisini oluştur
                    var categoryProduct = new CategoryProduct
                    {
                        CategoryId = category.Id,
                        ProductId = product.Id
                    };

                    await _CategoryProductRepository.InsertAsync(categoryProduct); // Ara tabloya ekle
                }
            }

            // 5. Resim ID'lerinin geçerliliğini kontrol et ve ProductId güncellemesini yap
            if (input.ImageIds != null && input.ImageIds.Any())
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

            // 3. Eski kategori ilişkilerini temizle
            var existingCategoryProducts = await _CategoryProductRepository.GetListAsync(cp => cp.ProductId == id);
            foreach (var categoryProduct in existingCategoryProducts)
            {
                await _CategoryProductRepository.DeleteAsync(categoryProduct);
            }

            // 4. Yeni kategori ID'lerinin geçerliliğini kontrol et ve ara tabloya ekle
            if (input.CategoryIds != null && input.CategoryIds.Any())
            {
                foreach (var categoryId in input.CategoryIds)
                {
                    var category = await _categoryRepository.FirstOrDefaultAsync(c => c.Id == categoryId);
                    if (category == null)
                    {
                        throw new UserFriendlyException($"Kategori ID'si ({categoryId}) geçerli değil.");
                    }

                    var categoryProduct = new CategoryProduct
                    {
                        CategoryId = category.Id,
                        ProductId = product.Id
                    };

                    await _CategoryProductRepository.InsertAsync(categoryProduct); // Yeni kategori ilişkisini ara tabloya ekle
                }
            }

            // 5. Resim ID'lerinin geçerliliğini kontrol et ve ProductId güncellemesini yap
            if (input.ImageIds != null && input.ImageIds.Any())
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

            // 6. Ürün bilgilerini güncelle
            _mapper.Map(input, product);
            await _productRepository.UpdateAsync(product);

            // 7. Güncellenmiş ürünü DTO olarak geri döndür
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
            // Ürünü ve ilişkili resimleri ile kategorileri çekmek için gerekli detayları dahil et
            var product = await _productRepository
                .WithDetailsAsync(p => p.Images, p => p.CategoryProducts) // İlişkili verileri yükle
                .ConfigureAwait(false); // Yapılandırılmamış bir devam bağlamı kullan

            // ID'ye göre ürünü bul ve silinmiş/onaylanmamış olup olmadığını kontrol et
            var productEntity = product
                .FirstOrDefault(p => p.Id == id && !p.IsDeleted && p.IsApproved);

            if (productEntity == null)
            {
                throw new UserFriendlyException($"ID = {id} olan ürün bulunamadı veya silinmiş/onaylanmamış.");
            }

            // Kategori ürünlerinin ID'lerini al
            var categoryIds = productEntity.CategoryProducts.Select(cp => cp.CategoryId).ToList();
            var categories = await _categoryRepository
                .GetListAsync(c => categoryIds.Contains(c.Id) && !c.IsDeleted && c.IsApproved); // Kategorileri al

            // Resimleri DTO'ya dönüştür
            var imageDtos = productEntity.Images
                .Where(i => !i.IsDeleted) // Silinmiş resimleri hariç tut
                .Select(i => _mapper.Map<Image, ImageDto>(i))
                .ToList();

            // Kategorileri DTO'ya dönüştür
            var categoryDtos = categories
                .Where(c => !c.IsDeleted) // Silinmiş kategorileri hariç tut
                .Select(c => _mapper.Map<Category, CategoryDto>(c))
                .ToList();

            // Ürünü DTO'ya dönüştür
            var productDto = _mapper.Map<Product, ProductDto>(productEntity);

            // Resim ve kategorileri DTO'ya ekle
            productDto.Images = imageDtos;
            productDto.Categories = categoryDtos;

            return productDto;
        }

   

        public async Task<PagedResultDto<ProductDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            // 1. Sorgu oluştur ve ilişkili resimler ile kategorileri dahil et
            var queryable = await _productRepository.WithDetailsAsync(p => p.Images, p => p.CategoryProducts); // CategoryProducts ara tablosu

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
            var products = await AsyncExecuter.ToListAsync(
                queryable
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
            );

            // 7. DTO'ya dönüştür ve kategorileri yükle
            var productDtos = new List<ProductDto>();

            foreach (var product in products)
            {
                var productDto = _mapper.Map<Product, ProductDto>(product);

                // Kategorileri al
                var categoryIds = product.CategoryProducts.Select(cp => cp.CategoryId).ToList();
                var categories = await _categoryRepository.GetListAsync(c => categoryIds.Contains(c.Id) && !c.IsDeleted && c.IsApproved);

                // Kategorileri DTO'ya dönüştür
                var categoryDtos = categories.Select(c => _mapper.Map<Category, CategoryDto>(c)).ToList();

                // DTO'ya kategorileri ekle
                productDto.Categories = categoryDtos;

                productDtos.Add(productDto);
            }

            return new PagedResultDto<ProductDto>(
                totalCount,
                productDtos
            );
        }

    }
}
