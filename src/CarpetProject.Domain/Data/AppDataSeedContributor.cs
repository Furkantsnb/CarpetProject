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
using Volo.CmsKit.Pages;

namespace CarpetProject
{
    public class AppDataSeedContributor : IDataSeedContributor,ITransientDependency
    {

        private readonly IRepository<Category, int> _categoryRepository;
        private readonly IRepository<Product, int> _productRepository;
        private readonly IRepository<Image, int> _imageRepository;
        private readonly IRepository<CategoryProduct,int> _categoryProductRepository;

        public AppDataSeedContributor(IRepository<Category, int> categoryRepository, IRepository<Product, int> productRepository, IRepository<Image, int> imageRepository, IRepository<CategoryProduct, int> categoryProductRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _imageRepository = imageRepository;
            _categoryProductRepository = categoryProductRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            await SeedCategoriesAsync();
            await SeedProductsAsync();
            await SeedImagesAsync();
        }

        private async Task SeedCategoriesAsync()
        {
            // Ana kategoriler
            var electronics = await InsertCategoryIfNotExistsAsync(new CreateCategoryDto
            {
                Name = "Çocuk odası",
                Description = "Çocuk odası",
                IsApproved = true,
                ColorCode = "#FF5733",
                ImageId = 1, // Bu ID'ler daha sonra güncellenebilir
                ProductIds = new List<int> { 1, 2, 3 }
            });

            var fashion = await InsertCategoryIfNotExistsAsync(new CreateCategoryDto
            {
                Name = "Yatak odası",
                Description = "Yatak odası",
                IsApproved = true,
                ColorCode = "#33FF57",
                ImageId = 2,
                ProductIds = new List<int> { 4, 5 }
            });

            var home = await InsertCategoryIfNotExistsAsync(new CreateCategoryDto
            {
                Name = "Salon",
                Description = "Salon Halıları",
                IsApproved = true,
                ColorCode = "#5733FF",
                ImageId = 3,
                ProductIds = new List<int> { 6, 7 }
            });

            var books = await InsertCategoryIfNotExistsAsync(new CreateCategoryDto
            {
                Name = "Kaymaz",
                Description = "Kaymaz Halıları",
                IsApproved = true,
                ColorCode = "#33F5FF",
                ImageId = 4,
                ProductIds = new List<int> { 8, 9 }
            });

            var toys = await InsertCategoryIfNotExistsAsync(new CreateCategoryDto
            {
                Name = "Peluş",
                Description = "Peluş Halılar",
                IsApproved = true,
                ColorCode = "#FF33A8",
                ImageId = 5,
                ProductIds = new List<int> { 10 }
            });

            // Alt kategoriler (üst kategorilerle ilişkili)
            await InsertCategoryIfNotExistsAsync(new CreateCategoryDto
            {
                Name = "Smartphones",
                Description = "Latest smartphones.",
                IsApproved = true,
                ColorCode = "#FF5733",
                ParentCategoryId = electronics.Id,
                ImageId = 6,
                ProductIds = new List<int> { 2, 3 }
            });

            await InsertCategoryIfNotExistsAsync(new CreateCategoryDto
            {
                Name = "Laptops",
                Description = "Various types of laptops.",
                IsApproved = true,
                ColorCode = "#FF5733",
                ParentCategoryId = fashion.Id,
                ImageId = 7,
                ProductIds = new List<int> { 4, 5 }
            });

            await InsertCategoryIfNotExistsAsync(new CreateCategoryDto
            {
                Name = "Men's Clothing",
                Description = "Fashion for men.",
                IsApproved = true,
                ColorCode = "#33FF57",
                ParentCategoryId = home.Id,
                ImageId = 8,
                ProductIds = new List<int> { 6, 7 }
            });

            await InsertCategoryIfNotExistsAsync(new CreateCategoryDto
            {
                Name = "Women's Clothing",
                Description = "Fashion for women.",
                IsApproved = true,
                ColorCode = "#33FF57",
                ParentCategoryId = books.Id,
                ImageId = 9,
                ProductIds = new List<int> { 8, 9 }
            });

            await InsertCategoryIfNotExistsAsync(new CreateCategoryDto
            {
                Name = "Kitchen Appliances",
                Description = "Appliances for kitchen.",
                IsApproved = true,
                ColorCode = "#5733FF",
                ParentCategoryId = toys.Id,
                ImageId = 10,
                ProductIds = new List<int> { 10 }
            });
        }

        private async Task<Category> InsertCategoryIfNotExistsAsync(CreateCategoryDto input)
        {
            var existingCategory = await _categoryRepository.FirstOrDefaultAsync(c => c.Name == input.Name);
            if (existingCategory == null)
            {
                var category = new Category
                {
                    Name = input.Name,
                    Description = input.Description,
                    IsApproved = input.IsApproved,
                    ColorCode = input.ColorCode,
                    ImageId = input.ImageId, // Resim ID'si ekleniyor
                    ParentCategoryId = input.ParentCategoryId
                };
                await _categoryRepository.InsertAsync(category);


                // Eklendikten sonra ID'yi almak için tekrar sorgulama yapalım
                existingCategory = await _categoryRepository.FirstOrDefaultAsync(c => c.Name == input.Name);
            }

            // Kategori ürünlerini ekleyelim
            if (input.ProductIds != null && input.ProductIds.Any())
            {
                foreach (var productId in input.ProductIds)
                {
                    await _categoryProductRepository.InsertAsync(new CategoryProduct
                    {
                        CategoryId = existingCategory.Id,
                        ProductId = productId
                    });
                }
            }

            return existingCategory;
        }

        private async Task SeedProductsAsync()
        {
            var products = new List<CreateProductDto>
        {
            new CreateProductDto
            {
                Name = "Çocuk odası-1",
                Price = 299.99M,
                HasDiscount = true,
                OldPrice = 300,
                Certification = true,
                Description = "Çocuk odası-1",
                Ingredients = "Çocuk odası-1",
                Usage = "Çocuk odası-1",
                AdditionalInfo = "Çocuk odası-1",
                IsApproved = true,
                CategoryIds = new List<int> { 1 },
                ImageIds = new List<int>{1}
            },
            new CreateProductDto
            {
                Name = "Çocuk odası-2",
                Price = 299.99M,
                HasDiscount = true,
                OldPrice = 300,
                Certification = true,
                Description = "Çocuk odası-2",
                Ingredients = "Çocuk odası-2",
                Usage = "Çocuk odası-2",
                AdditionalInfo = "Çocuk odası-2",
                IsApproved = true,
                CategoryIds = new List<int> { 1, 6 },
                ImageIds = new List<int>{2}
            },
            new CreateProductDto
            {
                Name = "Çocuk odası-3",
                Price = 299.99M,
                HasDiscount = true,
                OldPrice = 300,
                Certification = true,
                Description = "Çocuk odası-3",
                Ingredients = "Çocuk odası-3",
                Usage = "Çocuk odası-3",
                AdditionalInfo = "Çocuk odası-3",
                IsApproved = true,
                CategoryIds = new List<int> { 1, 6 },
                ImageIds = new List<int>{3}
            },
            new CreateProductDto
            {
                Name = "Yatak odası-1",
                Price = 299.99M,
                HasDiscount = true,
                OldPrice = 300,
                Certification = true,
                Description = "Yatak odası-1",
                Ingredients = "Yatak odası-1",
                Usage = "Yatak odası-1",
                AdditionalInfo = "Yatak odası-1",
                IsApproved = true,
                CategoryIds = new List<int> { 2, 7 },
                ImageIds = new List<int>{4}
            },
            new CreateProductDto
            {
                Name = "Yatak odası-2",
                Price = 299.99M,
                HasDiscount = true,
                OldPrice = 300,
                Certification = true,
                Description = "Yatak odası-2",
                Ingredients = "Yatak odası-2",
                Usage = "Yatak odası-2",
                AdditionalInfo = "Yatak odası-2",
                IsApproved = true,
                CategoryIds = new List<int> { 2, 7 },
                ImageIds = new List<int>{5}
            },
            new CreateProductDto
            {
                Name = "Salon",
                Price = 299.99M,
                HasDiscount = true,
                OldPrice = 300,
                Certification = true,
                Description = "Salon",
                Ingredients = "Salon",
                Usage = "Salon",
                AdditionalInfo = "Salon",
                IsApproved = true,
                CategoryIds = new List<int> { 3 },
                ImageIds = new List<int>{6}
            },
            new CreateProductDto
            {
                Name = "Kaymaz",
                Price = 299.99M,
                HasDiscount = true,
                OldPrice = 300,
                Certification = true,
                Description = "Kaymaz",
                Ingredients = "Kaymaz",
                Usage = "Kaymaz",
                AdditionalInfo = "Kaymaz",
                IsApproved = true,
                CategoryIds = new List<int> { 4 },
                ImageIds = new List<int>{7}
            },
            new CreateProductDto
            {
                Name = "Peluş",
                Price = 299.99M,
                HasDiscount = true,
                OldPrice = 300,
                Certification = true,
                Description = "Peluş",
                Ingredients = "Peluş",
                Usage = "Peluş",
                AdditionalInfo = "Peluş",
                IsApproved = true,
                CategoryIds = new List<int> { 5 },
                ImageIds = new List<int>{8}
            }
        };

            foreach (var product in products)
            {
                await InsertProductIfNotExistsAsync(product);
            }
        }

        private async Task<Product> InsertProductIfNotExistsAsync(CreateProductDto input)
        {
            var existingProduct = await _productRepository.FirstOrDefaultAsync(p => p.Name == input.Name);
            if (existingProduct == null)
            {
                var product = new Product
                {
                    Name = input.Name,
                    Price = input.Price,
                    HasDiscount = input.HasDiscount,
                    OldPrice = input.OldPrice,
                    Certification = input.Certification,
                    Description = input.Description,
                    Ingredients = input.Ingredients,
                    Usage = input.Usage,
                    AdditionalInfo = input.AdditionalInfo,
                    IsApproved = input.IsApproved
                };
                await _productRepository.InsertAsync(product);


                // Eklendikten sonra ID'yi almak için tekrar sorgulama yapalım
                existingProduct = await _productRepository.FirstOrDefaultAsync(p => p.Name == input.Name);
            }

            // Ürün resimlerini ekleyelim
            if (input.ImageIds != null && input.ImageIds.Any())
            {
                foreach (var imageId in input.ImageIds)
                {
                    await _imageRepository.InsertAsync(new Image
                    {
                        ProductId = existingProduct.Id,
                     
                    });
                }
            }

            // Ürün kategorilerini ekleyelim
            if (input.CategoryIds != null && input.CategoryIds.Any())
            {
                foreach (var categoryId in input.CategoryIds)
                {
                    await _categoryProductRepository.InsertAsync(new CategoryProduct
                    {
                        ProductId = existingProduct.Id,
                        CategoryId = categoryId
                    });
                }
            }

            return existingProduct;
        }
        private async Task<Image> InsertImageIfNotExistsAsync(string filePath, int? categoryId, int? productId)
        {
            var fileName = Path.GetFileName(filePath);
            var imageUrl = $"/images/DataSeedImage/{fileName}";

            var existingImage = await _imageRepository.FirstOrDefaultAsync(i => i.ImageUrl == imageUrl);
            if (existingImage == null)
            {
                var image = new Image
                {
                    Name = fileName,
                    ImageUrl = imageUrl,
                    CategoryId = categoryId,
                    ProductId = productId
                };

                await _imageRepository.InsertAsync(image);
                return image;
            }

            return existingImage;
        }

        private async Task SeedImagesAsync()
        {
            var imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "DataSeedImage");

            if (!Directory.Exists(imageDirectory))
            {
                throw new DirectoryNotFoundException($"Directory '{imageDirectory}' not found.");
            }

            var imageFiles = Directory.GetFiles(imageDirectory);

            var categories = await _categoryRepository.GetListAsync();
            var products = await _productRepository.GetListAsync();

            for (int i = 0; i < Math.Min(10, imageFiles.Length); i++)
            {
                var filePath = imageFiles[i];
                var image = await InsertImageIfNotExistsAsync(filePath, i < categories.Count ? categories[i].Id : (int?)null, i < products.Count ? products[i].Id : (int?)null);
            }
        }
    }
}

