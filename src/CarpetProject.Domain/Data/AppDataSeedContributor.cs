using CarpetProject.Categories;
using CarpetProject.Entities.Categories;
using CarpetProject.Entities.Products;
using CarpetProject.Products;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.CmsKit.Pages;

namespace CarpetProject
{
    public class AppDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly IRepository<Product, int> _productRepository;
        private readonly IRepository<Image, int> _imageRepository;
        private readonly IRepository<CategoryProduct, int> _categoryProductRepository; // Add repository for CategoryProduct

        public AppDataSeedContributor(IRepository<Category, int> categoryRepository, IRepository<Product, int> productRepository, IRepository<Image, int> imageRepository, IRepository<CategoryProduct, int> categoryProductRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _imageRepository = imageRepository;
            _categoryProductRepository = categoryProductRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            // Kategori seed
            var parentCategory = new Category
            {
                Name = "Parent Category",
                Description = "Parent Category Description",
                ColorCode = "#FFFFFF",
                IsApproved = true
            };
            await _categoryRepository.InsertAsync(parentCategory, autoSave: true);

            var category1 = new Category
            {
                Name = "Category 1",
                Description = "Category Description 1",
                ParentCategoryId = parentCategory.Id,
                IsApproved = true,
                ColorCode = "#FF0000",
            };
            var category2 = new Category
            {
                Name = "Category 2",
                Description = "Category Description 2",
                ParentCategoryId = parentCategory.Id,
                IsApproved = false,
                ColorCode = "#00FF00",
            };
            var category3 = new Category
            {
                Name = "Category 3",
                Description = "Category Description 3",
                ParentCategoryId = parentCategory.Id,
                IsApproved = true,
                ColorCode = "#0000FF",
            };

            await _categoryRepository.InsertAsync(category1, autoSave: true);
            await _categoryRepository.InsertAsync(category2, autoSave: true);
            await _categoryRepository.InsertAsync(category3, autoSave: true);

            // Ürün seed
            var product1 = new Product
            {
                Name = "Product 1",
                Price = 100,
                HasDiscount = true,
                OldPrice = 120,
                Certification = true,
                Description = "Product Description 1",
                Ingredients = "Ingredients 1",
                Usage = "Usage Instructions 1",
                AdditionalInfo = "Additional Info 1",
                IsApproved = true
            };
            var product2 = new Product
            {
                Name = "Product 2",
                Price = 200,
                HasDiscount = false,
                Certification = false,
                Description = "Product Description 2",
                Ingredients = "Ingredients 2",
                Usage = "Usage Instructions 2",
                AdditionalInfo = "Additional Info 2",
                IsApproved = false
            };
            var product3 = new Product
            {
                Name = "Product 3",
                Price = 300,
                HasDiscount = true,
                OldPrice = 350,
                Certification = true,
                Description = "Product Description 3",
                Ingredients = "Ingredients 3",
                Usage = "Usage Instructions 3",
                AdditionalInfo = "Additional Info 3",
                IsApproved = true
            };

            await _productRepository.InsertAsync(product1, autoSave: true);
            await _productRepository.InsertAsync(product2, autoSave: true);
            await _productRepository.InsertAsync(product3, autoSave: true);

            // Create CategoryProduct entries
            await _categoryProductRepository.InsertAsync(new CategoryProduct { ProductId = product1.Id, CategoryId = category1.Id }, autoSave: true);
            await _categoryProductRepository.InsertAsync(new CategoryProduct { ProductId = product2.Id, CategoryId = category2.Id }, autoSave: true);
            await _categoryProductRepository.InsertAsync(new CategoryProduct { ProductId = product3.Id, CategoryId = category3.Id }, autoSave: true);

            // Görsel seed
            // Resimleri seed et
         
            var images = await SeedImagesAsync();

            // Kategori ve ürünleri güncelle
            await UpdateCategoryAndProductImagesAsync(images);

        }

        public async Task<List<Image>> SeedImagesAsync()
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/DataSeedImage/");

            var images = new List<Image>
    {
        new Image { Name = "Image 1", ImageUrl = Path.Combine(basePath, "12-Photoroom.png") },
        new Image { Name = "Image 2", ImageUrl = Path.Combine(basePath, "13-Photoroom.png") },
        new Image { Name = "Image 3", ImageUrl = Path.Combine(basePath, "b587adde-548d-46c1-b26d-9a6754c055d3.jpg") }
    };

            foreach (var image in images)
            {
                await _imageRepository.InsertAsync(image, autoSave: true);
            }

            return images;
        }
        public async Task UpdateCategoryAndProductImagesAsync(List<Image> images)
        {
            var categories = await _categoryRepository.GetListAsync();
            var products = await _productRepository.GetListAsync();

            foreach (var image in images)
            {
                var category = categories.FirstOrDefault(c => c.Id == image.CategoryId);
                var product = products.FirstOrDefault(p => p.Id == image.ProductId);

                if (category != null)
                {
                    // Güncellenmiş: Kategori için tek bir resim ID'si ayarlanır
                    category.ImageId = image.Id;
                    await _categoryRepository.UpdateAsync(category, autoSave: true);
                }

                if (product != null)
                {
                    // Ürün için resimler listesine eklenir
                    product.Images.Add(image);
                    await _productRepository.UpdateAsync(product, autoSave: true);
                }
            }
        }
    }

}
