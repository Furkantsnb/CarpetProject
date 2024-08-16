using CarpetProject.EntityDtos.Tags;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetProject.FluentValidations.Tags
{
    public class UpdateTagDtoValidator : AbstractValidator<TagDto>
    {
        public UpdateTagDtoValidator()
        {
            // Name alanı zorunludur ve en az 3, en fazla 50 karakter olmalıdır
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Etiket adı boş olamaz.")
                .Length(3, 50)
                .WithMessage("Etiket adı 3 ile 50 karakter arasında olmalıdır.");

            // Color alanı opsiyoneldir, ancak sağlanmışsa geçerli bir HEX rengi olmalıdır
            RuleFor(x => x.Color)
                .Matches("^#(?:[0-9a-fA-F]{3}){1,2}$")
                .WithMessage("Renk geçerli bir HEX kodu olmalıdır.")
                .When(x => !string.IsNullOrEmpty(x.Color));

            // IconUrl opsiyoneldir, ancak sağlanmışsa geçerli bir URL formatında olmalıdır
            RuleFor(x => x.IconUrl)
                .Must(IsValidUrl)
                .WithMessage("IconUrl geçerli bir URL olmalıdır.")
                .When(x => !string.IsNullOrEmpty(x.IconUrl));
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
