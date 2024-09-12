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


        #region Category
        // Category ve CategoryDto arasında mapping
        // CreateCategoryDto -> Category mapping
        CreateMap<CreateCategoryDto, Category>()
            .ForMember(dest => dest.CategoryProducts, opt => opt.MapFrom(src => src.ProductIds.Select(id => new CategoryProduct { ProductId = id })))
            .ReverseMap(); // ReverseMap() tersine dönüşüm için

        // CategoryDto -> Category mapping
        CreateMap<CategoryDto, Category>()
            .ForMember(dest => dest.CategoryProducts, opt => opt.MapFrom(src => src.Products.Select(p => new CategoryProduct { ProductId = p.Id })))
            .ReverseMap();

        // UpdateCategoryDto -> Category mapping
        CreateMap<UpdateCategoryDto, Category>()
            .ForMember(dest => dest.CategoryProducts, opt => opt.MapFrom(src => src.ProductIds.Select(id => new CategoryProduct { ProductId = id })))
            .ReverseMap();
        #endregion

        #region Product
        // Product <-> CreateProductDto
        CreateMap<Product, CreateProductDto>()
            .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => src.CategoryProducts.Select(cp => cp.CategoryId).ToList()))
            .ForMember(dest => dest.ImageIds, opt => opt.MapFrom(src => src.Images.Select(img => img.Id).ToList()))
            .ReverseMap()
            .ForMember(dest => dest.CategoryProducts, opt => opt.Ignore()) // CategoryProducts manuel eşlenecek
            .ForMember(dest => dest.Images, opt => opt.Ignore()); // Images manuel eşlenecek

        // Product <-> UpdateProductDto
        CreateMap<Product, UpdateProductDto>()
            .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => src.CategoryProducts.Select(cp => cp.CategoryId).ToList()))
            .ForMember(dest => dest.ImageIds, opt => opt.MapFrom(src => src.Images.Select(img => img.Id).ToList()))
            .ReverseMap()
            .ForMember(dest => dest.CategoryProducts, opt => opt.Ignore()) // CategoryProducts manuel eşlenecek
            .ForMember(dest => dest.Images, opt => opt.Ignore()); // Images manuel eşlenecek

        // Product <-> ProductDto
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.CategoryProducts.Select(cp => cp.Category).ToList()))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
            .ReverseMap();
        #endregion

        #region Image
        // CreateImageDto -> Image mapping
        CreateMap<CreateImageDto, Image>()
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())  // IFormFile -> string manuel atanacak
            .ReverseMap();                                         // ReverseMap() tersine dönüşüm için

        // ImageDto -> Image mapping
        CreateMap<ImageDto, Image>().ReverseMap();// ReverseMap() tersine dönüşüm için

        // UpdateImageDto -> Image mapping
        CreateMap<UpdateImageDto, Image>()
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())  // IFormFile -> string manuel atanacak
            .ReverseMap();
        #endregion

    }
}
