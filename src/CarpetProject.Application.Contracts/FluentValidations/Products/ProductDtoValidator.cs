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

            // Description zorunlu ve en fazla 500 karakter olmalı
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Açıklama boş olamaz.")
                .MaximumLength(500)
                .WithMessage("Açıklama 500 karakteri aşmamalıdır.");


            RuleFor(x => x.IsApproved)
                .NotNull().WithMessage("Onay durumu gereklidir.")
                .Must(val => val == true || val == false).WithMessage("Onay durumu true ya da false olmalıdır.");

            RuleFor(x => x.Certification)
                    .NotNull().WithMessage("Serfika durumu gereklidir.")
                    .Must(val => val == true || val == false).WithMessage("Serfika durumu true ya da false olmalıdır.");

            RuleFor(x => x.HasDiscount)
                   .NotNull().WithMessage("İndirim durumu gereklidir.")
                   .Must(val => val == true || val == false).WithMessage("indirim durumu true ya da false olmalıdır.");

            RuleFor(x => x.HasDiscount)
                   .Must((dto, hasDiscount) =>
                   {
                       if (hasDiscount && dto.OldPrice == null)
                       {
                           return false;
                       }
                       return true;
                   })
                     .WithMessage("İndirim uygulanmışsa eski fiyat belirtilmelidir.")
                  .When(x => x.HasDiscount); // Eski fiyatı sadece indirim uygulandığında kontrol et

            RuleFor(x => x.OldPrice)
                .GreaterThan(0.01m)
                .When(x => x.HasDiscount) // Eski fiyatı sadece indirim uygulandığında kontrol et
                .WithMessage("Eski fiyat 0.01'den büyük olmalıdır.");


            RuleFor(x => x)
                .Must(x => !x.HasDiscount || (x.OldPrice.HasValue && x.Price < x.OldPrice))
                .WithMessage("İndirimli fiyat, eski fiyattan düşük olmalıdır.");


            // Ürünün en az bir kategoriye atanmış olduğunu kontrol eden kural
            RuleFor(x => x.CategoryIds)
                .NotEmpty().WithMessage("Ürün en az bir kategoriye atanmalıdır.")
                .Must(categoryIds => categoryIds != null && categoryIds.Count <= 5)
                .WithMessage("Bir ürün en fazla 5 kategoriye atanabilir.");

            // Ürünün en az bir görsele sahip olduğunu kontrol eden kural
            RuleFor(x => x.ImageIds)
                .NotEmpty().WithMessage("Ürün için en az bir görsel yüklenmelidir.")
                .Must(imageIds => imageIds != null && imageIds.Count <= 10)
                .WithMessage("Ürün için en fazla 10 görsel yüklenebilir.");

        }

    }
}
