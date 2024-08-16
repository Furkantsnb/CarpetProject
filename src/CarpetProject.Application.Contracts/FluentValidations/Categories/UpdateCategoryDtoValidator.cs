using CarpetProject.Entities.Categories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetProject.FluentValidations.Categories
{
    public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
    {
        public UpdateCategoryDtoValidator()
        {
            // Name zorunlu ve 3 ile 100 karakter arasında olmalı
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Kategori adı boş olamaz.")
                .Length(3, 100)
                .WithMessage("Kategori adı 3 ile 100 karakter arasında olmalıdır.");

            // Description zorunlu ve en fazla 500 karakter olmalı
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Açıklama boş olamaz.")
                .MaximumLength(500)
                .WithMessage("Açıklama 500 karakteri aşmamalıdır.");

            // Image zorunlu ve geçerli bir URL formatında olmalı
            RuleFor(x => x.Image)
                .NotEmpty()
                .WithMessage("Resim URL'si boş olamaz.")
                .Must(IsValidUrl)
                .WithMessage("Resim URL'si geçerli bir URL olmalıdır.");

            // ColorCode zorunlu ve geçerli bir HEX renk kodu formatında olmalı
            RuleFor(x => x.ColorCode)
                .NotEmpty()
                .WithMessage("Renk kodu boş olamaz.")
                .Matches("^#(?:[0-9a-fA-F]{3}){1,2}$")
                .WithMessage("Renk kodu geçerli bir HEX kodu olmalıdır.");

            // IsApproved zorunlu, true ya da false olmalı
            RuleFor(x => x.IsApproved)
                .NotNull()
                .WithMessage("Onay durumu boş olamaz.")
                .Must(x => x == true || x == false)
                .WithMessage("Onay durumu true ya da false olmalıdır.");

            // SubCategories listesi null olamaz, fakat boş olabilir
            RuleFor(x => x.SubCategories)
                .NotNull()
                .WithMessage("Alt kategoriler listesi null olamaz.");
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
