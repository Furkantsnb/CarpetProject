using AutoMapper;
using CarpetProject.Categories;
using CarpetProject.Entities.Categories;
using CarpetProject.Entities.Products;
using CarpetProject.EntityDto.ProductImages;

using CarpetProject.Products;
using System.Linq;

namespace CarpetProject;

public class CarpetProjectApplicationAutoMapperProfile : Profile
{
    public CarpetProjectApplicationAutoMapperProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<Product,CreateProductDto>().ReverseMap();
        CreateMap<Product,UpdateProductDto>().ReverseMap();
        CreateMap<Product, ProductDto>()
    .ForMember(dest => dest.ImageIds, opt => opt.MapFrom(src => src.Images.Select(pi => pi.Id).ToList()));
        CreateMap<Image, ImageDto>().ReverseMap();
        CreateMap<Image, CreateImageDto>().ReverseMap();
        CreateMap<Image, UpdateImageDto>().ReverseMap();

        // Category -> CategoryDto mapping
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Category, CreateCategoryDto>().ReverseMap();
        CreateMap<Category, UpdateCategoryDto>().ReverseMap();

     





        //// Custom mappings for ignoring certain properties
        //CreateMap<CreateProductDto, Product>()
        //    .ForMember(dest => dest.Categories, opt => opt.Ignore());

        //CreateMap<UpdateProductDto, Product>()
        //    .ForMember(dest => dest.Categories, opt => opt.Ignore());

        //// Product -> ProductWithCategoryDto mapping
        //CreateMap<Product, ProductWithCategoryDto>()
        //    .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src))
        //    .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Categories.FirstOrDefault()))
        //    .ForMember(dest => dest.ProductCountInCategory, opt => opt.Ignore());
    }
}
