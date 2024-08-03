using AutoMapper;
using CarpetProject.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public ProductImageManager(IRepository<Product, int> productRepository, IRepository<Category, int> categoryRepository, IRepository<ProductImage, int> productImage, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productImage = productImage;
            _mapper = mapper;
        }
    }
}
