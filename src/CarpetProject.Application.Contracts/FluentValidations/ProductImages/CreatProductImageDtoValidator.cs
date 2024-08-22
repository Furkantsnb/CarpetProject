using CarpetProject.EntityDto.ProductImages;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetProject.FluentValidations.ProductImages
{
    public class CreatProductImageDtoValidator :AbstractValidator<CreateProductImageDto>
    {
        public CreatProductImageDtoValidator()
        {
            RuleFor(x => x.CategoryId)
                 .NotNull().When(x => x.ProductId == null).WithMessage("Kategori ID gereklidir.")
                 .Must((dto, categoryId) => categoryId != null || dto.ProductId != null)
                 .WithMessage("Kategori ID veya Ürün ID'den en az biri girilmelidir.");

            RuleFor(x => x.ProductId)
                .NotNull().When(x => x.CategoryId == null).WithMessage("Ürün ID gereklidir.")
                .Must((dto, productId) => productId != null || dto.CategoryId != null)
                .WithMessage("Kategori ID veya Ürün ID'den en az biri girilmelidir.");

            // ImageUrl'in boş olmaması gerektiğini belirtir
            RuleFor(x => x.ImageUrl)
                .NotEmpty()
                .WithMessage("Resim Boş Olamaz")
                 .Must(IsValidUrl)
                .WithMessage("Geçerli bir URL Gİriniz.")
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        
        }

        // Geçerli bir URL olup olmadığını kontrol eden bir yardımcı metot
        private bool IsValidUrl(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult))
            {
                return uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps;
            }
            return false;
        }
    }
}
