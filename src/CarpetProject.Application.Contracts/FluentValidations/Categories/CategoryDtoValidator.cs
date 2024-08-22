using CarpetProject.Entities.Categories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetProject.FluentValidations.Categories
{
    public class CategoryDtoValidator : AbstractValidator<CategoryDto>
    {
        public CategoryDtoValidator()
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


        }

     
    }
}
