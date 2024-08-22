using AutoMapper;
using CarpetProject.Categories;
using CarpetProject.Entities.Products;
using CarpetProject.EntityDto.ProductImages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace CarpetProject.Products
{
    public class ProductImageManager : DomainService
    {
        private readonly IRepository<Product, int> _productRepository;
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly IRepository<ProductImage, int> _productImage;
        private readonly IMapper _mapper;

        private readonly IBlobContainer _blobContainer;

      
        
        public ProductImageManager(IRepository<Product, int> productRepository, IRepository<Category, int> categoryRepository, IRepository<ProductImage, int> productImage, IMapper mapper, IBlobContainerFactory blobContainerFactory)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productImage = productImage;
            _mapper = mapper;
            _blobContainer = blobContainerFactory.Create("furkan");
        }

        public async Task<ProductImageDto> CreateAsync(CreateProductImageDto input)
        {
            // Kategori ID veya Ürün ID'nin geçerliliğini kontrol et
            if (input.CategoryId.HasValue)
            {
                var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == input.CategoryId.Value);
                if (!categoryExists)
                {
                    throw new UserFriendlyException($"Kategori ID'si ({input.CategoryId}) geçerli değil.");
                }
            }

            if (input.ProductId.HasValue)
            {
                var productExists = await _productRepository.AnyAsync(p => p.Id == input.ProductId.Value);
                if (!productExists)
                {
                    throw new UserFriendlyException($"Ürün ID'si ({input.ProductId}) geçerli değil.");
                }
            }

            // Aynı isimde başka bir resim olup olmadığını kontrol et
            var isImageNameExists = await _productImage.AnyAsync(img => img.Name == input.Name &&
                (img.CategoryId == input.CategoryId || img.ProductId == input.ProductId));
            if (isImageNameExists)
            {
                throw new UserFriendlyException($"Bu isim ({input.Name}) zaten başka bir resim tarafından kullanılmıştır.");
            }

            // Resmi MinIO'ya yükleyin
            var blobName = Guid.NewGuid().ToString("N") + Path.GetExtension(input.ImageUrl);

            // Resim dosyasını okuyup byte array'e çevirin
            byte[] imageBytes;
            try
            {
                imageBytes = await File.ReadAllBytesAsync(input.ImageUrl); // input.ImageUrl burada bir dosya yolu olmalı
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Resim dosyasını okurken bir hata oluştu.", ex.Message);
            }

            await _blobContainer.SaveAsync(blobName, imageBytes);

            // DTO'yu ürün resim entity'sine dönüştürün ve MinIO URL'ini ayarlayın
            var productImage = _mapper.Map<CreateProductImageDto, ProductImage>(input);
            productImage.ImageUrl = blobName; // MinIO'daki resim URL'sini ayarlayın

            // Resmi veritabanına ekleyin
            await _productImage.InsertAsync(productImage);

            // Eklenen resmi DTO olarak geri döndürün
            return _mapper.Map<ProductImage, ProductImageDto>(productImage);
        }

        public async Task<ProductImageDto> UpdateAsync(int id, UpdateProductImageDto input)
        {
            // Güncellenmekte olan resim kaydını al
            var productImage = await _productImage.GetAsync(id);

            // Kategori ID veya Ürün ID'nin geçerliliğini kontrol et
            if (input.CategoryId.HasValue)
            {
                var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == input.CategoryId.Value);
                if (!categoryExists)
                {
                    throw new UserFriendlyException($"Kategori ID'si ({input.CategoryId}) geçerli değil.");
                }
            }

            if (input.ProductId.HasValue)
            {
                var productExists = await _productRepository.AnyAsync(p => p.Id == input.ProductId.Value);
                if (!productExists)
                {
                    throw new UserFriendlyException($"Ürün ID'si ({input.ProductId}) geçerli değil.");
                }
            }

            // Aynı isimde başka bir resim olup olmadığını kontrol et, ancak mevcut resim hariç tutulmalı
            var isImageNameExists = await _productImage.AnyAsync(img => img.Name == input.Name &&
                img.Id != id &&
                (img.CategoryId == input.CategoryId || img.ProductId == input.ProductId));
            if (isImageNameExists)
            {
                throw new UserFriendlyException($"Bu isim ({input.Name}) zaten başka bir resim tarafından kullanılmıştır.");
            }

            // Eğer yeni bir resim URL'i verilmişse, MinIO'ya yükle
            if (!string.IsNullOrEmpty(input.ImageUrl))
            {
                var blobName = Guid.NewGuid().ToString("N") + Path.GetExtension(input.ImageUrl);

                // Resim dosyasını okuyup byte array'e çevirin
                byte[] imageBytes;
                try
                {
                    imageBytes = await File.ReadAllBytesAsync(input.ImageUrl); // input.ImageUrl burada bir dosya yolu olmalı
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException("Resim dosyasını okurken bir hata oluştu.", ex.Message);
                }

                await _blobContainer.SaveAsync(blobName, imageBytes);
                productImage.ImageUrl = blobName; // MinIO'daki yeni resim URL'sini ayarlayın
            }

            // Diğer alanları güncelle
            _mapper.Map(input, productImage);

            // Resmi veritabanında güncelle
            await _productImage.UpdateAsync(productImage);

            // Güncellenen resmi DTO olarak geri döndür
            return _mapper.Map<ProductImage, ProductImageDto>(productImage);
        }

        public async Task DeleteAsync(int id)
        {
            // İlgili resim kaydını al
            var productImage = await _productImage.GetAsync(id);

         
            // Soft delete işlemi: Resmi "silinmiş" olarak işaretle
            productImage.IsDeleted = true;

            // Veritabanında güncelle
            await _productImage.UpdateAsync(productImage);
        }

        public async Task<ProductImageDto> GetAsync(int id)
        {
            // İlgili resim kaydını al
            var productImage = await _productImage.FirstOrDefaultAsync(img => img.Id == id && !img.IsDeleted);

            if (productImage == null)
            {
                throw new UserFriendlyException("Resim bulunamadı veya silinmiş.");
            }
            return _mapper.Map<ProductImage, ProductImageDto>(productImage);
        }

        public async Task<PagedResultDto<ProductImageDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var queryable = await _productImage.GetQueryableAsync();
            var totalCount = await AsyncExecuter.CountAsync(queryable);

            var lines = await AsyncExecuter.ToListAsync(
                queryable.OrderBy(a => a.Id)
                         .Skip(input.SkipCount)
                         .Take(input.MaxResultCount)
            );

            var productImageDtos = _mapper.Map<List<ProductImage>, List<ProductImageDto>>(lines);

            return new PagedResultDto<ProductImageDto>(
                totalCount,
                productImageDtos
            );
        }

       
    }
}
