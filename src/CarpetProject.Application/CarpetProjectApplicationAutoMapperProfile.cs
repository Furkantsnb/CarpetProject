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
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<Category, CreateProductDto>().ReverseMap();
        CreateMap<Category, UpdateProductDto>().ReverseMap();
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
