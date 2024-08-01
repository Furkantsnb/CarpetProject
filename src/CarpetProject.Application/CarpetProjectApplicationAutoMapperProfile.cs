using AutoMapper;
using CarpetProject.Categories;
using CarpetProject.Entities.Categories;
using CarpetProject.Entities.Products;
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

        // Product -> ProductWithCategoryDto mapping
        CreateMap<Product, ProductWithCategoryDto>()
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Categories.FirstOrDefault()))
            .ForMember(dest => dest.ProductCountInCategory, opt => opt.Ignore());


        // Custom mappings for ignoring certain properties
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore());

        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore());



        // Category -> CategoryDto mapping
        CreateMap<Category, CategoryDto>().ReverseMap();
    }
}
