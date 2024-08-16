using CarpetProject.EntityDto.ProductImages;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetProject.FluentValidations.ProductImages
{
    public class ProductImageDtoValidator :AbstractValidator<ProductImageDto>
    {
        public ProductImageDtoValidator() 
        {

            // ProductId'nin pozitif bir sayı olması gerektiğini belirtir
            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId 0'dan Büyük Olmalı");

            // ImageUrl'in boş olmaması gerektiğini belirtir
            RuleFor(x => x.ImageUrl)
                .NotEmpty()
                .WithMessage("Resim Boş Olamaz");


            // ImageUrl'in geçerli bir URL formatında olup olmadığını kontrol eder
            RuleFor(x => x.ImageUrl)
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
