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
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<Product, CreateProductDto>().ReverseMap();
        CreateMap<Product, UpdateProductDto>().ReverseMap();

        CreateMap<Image, ImageDto>().ReverseMap();
        CreateMap<Image, CreateImageDto>().ReverseMap();
        CreateMap<Image, UpdateImageDto>().ReverseMap();

        // Category -> CategoryDto mapping
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Category, CreateCategoryDto>().ReverseMap();
        CreateMap<Category, UpdateCategoryDto>().ReverseMap();

        CreateMap<CreateCategoryDto, Category>()
      .ForMember(dest => dest.Products, opt => opt.Ignore()); // Ürünleri dikkate almayın

        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products.Select(p => p.Id).ToList()));


    }
}
