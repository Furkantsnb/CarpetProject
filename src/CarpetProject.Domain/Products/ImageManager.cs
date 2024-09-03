using AutoMapper;
using CarpetProject.Categories;
using CarpetProject.Entities.Products;
using CarpetProject.EntityDto.ProductImages;
using Microsoft.IdentityModel.Tokens;
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
    public class ImageManager : DomainService
    {
        private readonly IRepository<Product, int> _productRepository;
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly IRepository<Image, int> _productImage;
        private readonly IMapper _mapper;

        private readonly IBlobContainer _blobContainer;

      
        
        public ImageManager(IRepository<Product, int> productRepository, IRepository<Category, int> categoryRepository, IRepository<Image, int> productImage, IMapper mapper, IBlobContainerFactory blobContainerFactory)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productImage = productImage;
            _mapper = mapper;
            _blobContainer = blobContainerFactory.Create("furkan");
        }

        public async Task<ImageDto> CreateAsync(CreateImageDto input)
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
            var blobName = Guid.NewGuid().ToString("N") + Path.GetExtension(input.ImageFile.FileName);

            // Resim dosyasını okuyup byte array'e çevirin
            byte[] imageBytes;
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await input.ImageFile.CopyToAsync(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Resim dosyasını okurken bir hata oluştu.", ex.Message);
            }

            await _blobContainer.SaveAsync(blobName, imageBytes);

            // DTO'yu ürün resim entity'sine dönüştürün ve MinIO URL'ini ayarlayın
            var productImage = _mapper.Map<CreateImageDto, Image>(input);
            productImage.ImageUrl = blobName; // MinIO'daki resim URL'sini ayarlayın

            // Resmi veritabanına ekleyin
            await _productImage.InsertAsync(productImage);

            // Eklenen resmi DTO olarak geri döndürün
            return _mapper.Map<Image, ImageDto>(productImage);
        }

        public async Task<ImageDto> UpdateAsync(int id, UpdateImageDto input)
        {
            // Güncellenmekte olan resim kaydını al
            var productImage = await _productImage.GetAsync(id);
            if (productImage == null)
            {
                throw new UserFriendlyException($"Resim ID'si ({id}) geçerli değil.");
            }

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

            // Eğer yeni bir resim dosyası sağlanmışsa, resmi MinIO'ya yükleyin
            if (input.ImageFile != null && input.ImageFile.Length > 0)
            {
                var blobName = Guid.NewGuid().ToString("N") + Path.GetExtension(input.ImageFile.FileName);

                // Resim dosyasını okuyup byte array'e çevirin
                byte[] imageBytes;
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await input.ImageFile.CopyToAsync(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException("Resim dosyasını okurken bir hata oluştu.", ex.Message);
                }

                // Eski resmi silmek isteyebilirsiniz, ancak bu isteğe bağlıdır
                // await _blobContainer.DeleteAsync(productImage.ImageUrl);

                await _blobContainer.SaveAsync(blobName, imageBytes);

                // MinIO URL'ini güncelle
                productImage.ImageUrl = blobName;
            }

            // Diğer alanları güncelle
            _mapper.Map(input, productImage);

            // Resmi veritabanında güncelle
            await _productImage.UpdateAsync(productImage);

            // Güncellenen resmi DTO olarak geri döndür
            return _mapper.Map<Image, ImageDto>(productImage);
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

        public async Task<ImageDto> GetAsync(int id)
        {
            // İlgili resim kaydını al
            var productImage = await _productImage.FirstOrDefaultAsync(img => img.Id == id && !img.IsDeleted);

            if (productImage == null)
            {
                throw new UserFriendlyException("Resim bulunamadı veya silinmiş.");
            }
            return _mapper.Map<Image, ImageDto>(productImage);
        }

        public async Task<PagedResultDto<ImageDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var queryable = await _productImage.GetQueryableAsync();

            // 2. Yumuşak silinmiş ürünleri hariç tut
            queryable = queryable.Where(p => p.IsDeleted == false);
            var totalCount = await AsyncExecuter.CountAsync(queryable);

            var lines = await AsyncExecuter.ToListAsync(
                queryable.OrderBy(a => a.Id)
                         .Skip(input.SkipCount)
                         .Take(input.MaxResultCount)
            );

            var productImageDtos = _mapper.Map<List<Image>, List<ImageDto>>(lines);

            return new PagedResultDto<ImageDto>(
                totalCount,
                productImageDtos
            );
        }

       
    }
}
