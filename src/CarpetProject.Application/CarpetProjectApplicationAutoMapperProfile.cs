using AutoMapper;
using CarpetProject.Categories;
using CarpetProject.Entities.Categories;
using CarpetProject.Entities.Products;
using CarpetProject.EntityDto.ProductImages;

using CarpetProject.Products;
using Microsoft.AspNetCore.Routing.Constraints;
using System.Linq;

namespace CarpetProject;

public class CarpetProjectApplicationAutoMapperProfile : Profile
{

    public CarpetProjectApplicationAutoMapperProfile()
    {
        // CreateProductDto ile Product arasında mapping
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.CategoryProducts, opt => opt.MapFrom(src => src.CategoryIds.Select(id => new CategoryProduct { CategoryId = id })))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ImageIds));

        // UpdateProductDto ile Product arasında mapping
        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.CategoryProducts, opt => opt.MapFrom(src => src.CategoryIds.Select(id => new CategoryProduct { CategoryId = id })))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ImageIds));

        // Product ile ProductDto arasında mapping
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.CategoryProducts.Select(cp => new CategoryDto { Id = cp.CategoryId })))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(i => new ImageDto { Id = i.Id })));

        // ProductDto ile Product arasında mapping
        CreateMap<ProductDto, Product>()
            .ForMember(dest => dest.CategoryProducts, opt => opt.MapFrom(src => src.Categories.Select(c => new CategoryProduct { CategoryId = c.Id })))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images));

        // Image ile CreateImageDto arasında mapping
        CreateMap<CreateImageDto, Image>()
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()).ReverseMap(); // IFormFile doğrudan entity'ye aktarılmıyor

        // Image ile ImageDto arasında mapping
        CreateMap<Image, ImageDto>()
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()).ReverseMap(); // IFormFile doğrudan DTO'da gösterilmiyor

        // Image ile UpdateImageDto arasında mapping
        CreateMap<UpdateImageDto, Image>()
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()).ReverseMap(); // IFormFile doğrudan entity'ye aktarılmıyor


        // Category ile CreateCategoryDto arasında mapping
        CreateMap<CreateCategoryDto, Category>()
            .ForMember(dest => dest.CategoryProducts, opt => opt.MapFrom(src => src.ProductIds.Select(id => new CategoryProduct { ProductId = id })))
            .ForMember(dest => dest.Image, opt => opt.Ignore()); // Image ve ParentCategory için ilgili mapping işlemlerini belirleyebilirsiniz

        // Category ile UpdateCategoryDto arasında mapping
        CreateMap<UpdateCategoryDto, Category>()
            .ForMember(dest => dest.CategoryProducts, opt => opt.MapFrom(src => src.ProductIds.Select(id => new CategoryProduct { ProductId = id })))
            .ForMember(dest => dest.Image, opt => opt.Ignore()); // Image ve ParentCategory için ilgili mapping işlemlerini belirleyebilirsiniz

        // Category ile CategoryDto arasında mapping
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image != null ? new ImageDto { Id = src.Image.Id } : null))
            .ForMember(dest => dest.ParentCategory, opt => opt.MapFrom(src => src.ParentCategory != null ? new CategoryDto { Id = src.ParentCategory.Id } : null))
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.CategoryProducts.Select(cp => new ProductDto { Id = cp.ProductId })));

        // CategoryDto ile Category arasında mapping
        CreateMap<CategoryDto, Category>()
            .ForMember(dest => dest.CategoryProducts, opt => opt.MapFrom(src => src.Products.Select(p => new CategoryProduct { ProductId = p.Id })))
            .ForMember(dest => dest.Image, opt => opt.Ignore()) // Image ve ParentCategory için ilgili mapping işlemlerini belirleyebilirsiniz
            .ForMember(dest => dest.ParentCategory, opt => opt.Ignore()); // ParentCategory'yi daha detaylı bir şekilde yönetebilirsiniz



    }
}
