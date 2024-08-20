using CarpetProject.Entities.Products;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetProject.FluentValidations.Products
{
    public class ProductDtoValidator : AbstractValidator<ProductDto>
    {
        public ProductDtoValidator()
        {

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ürün adı gereklidir.")
                .MaximumLength(100).WithMessage("Ürün adı 100 karakterden uzun olmamalıdır.");

            RuleFor(x => x.Price)
                .GreaterThan(0.01m).WithMessage("Ürün fiyatı 0.01'den büyük olmalıdır.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Açıklama gereklidir.");

            RuleFor(x => x.ReleaseDate)
                .NotEmpty().WithMessage("Piyasaya çıkış tarihi gereklidir.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Piyasaya çıkış tarihi gelecekte olamaz.");

            RuleFor(x => x.ProductImages)
                .NotEmpty().WithMessage("En az bir ürün görseli gereklidir.")
                .ForEach(image => image
                    .Must(img => IsValidUrl(img.ImageUrl)).WithMessage("Her ürün görseli geçerli bir URL formatında olmalıdır."));


            RuleFor(x => x.IsApproved)
                .NotNull().WithMessage("Onay durumu gereklidir.")
                .Must(val => val == true || val == false).WithMessage("Onay durumu true ya da false olmalıdır.");

       

            RuleFor(x => x.Categories)
                .NotEmpty().WithMessage("En az bir kategori atanmalıdır.")
                .Must(cIds => cIds.Count > 0).WithMessage("En az bir kategori atanmalıdır.");

       
        }

        // Geçerli bir URL olup olmadığını kontrol eden yardımcı metot
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
