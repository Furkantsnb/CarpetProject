using AutoMapper;
using CarpetProject.Categories;
using CarpetProject.Entities.Categories;
using CarpetProject.Entities.Products;
using CarpetProject.EntityDto.ProductImages;
using CarpetProject.EntityDtos.Tags;
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

        CreateMap<ProductImage, ProductImageDto>().ReverseMap();
        CreateMap<ProductImage, CreateProductImageDto>().ReverseMap();
        CreateMap<ProductImage, UpdateProductImageDto>().ReverseMap();

        // Category -> CategoryDto mapping
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Category, CreateCategoryDto>().ReverseMap();
        CreateMap<Category, UpdateCategoryDto>().ReverseMap();

        CreateMap<certificate, CertificateDto>().ReverseMap();
        CreateMap<certificate, CreateCertificateDto>().ReverseMap();
        CreateMap<certificate, UpdateCertificateDto>().ReverseMap();





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
