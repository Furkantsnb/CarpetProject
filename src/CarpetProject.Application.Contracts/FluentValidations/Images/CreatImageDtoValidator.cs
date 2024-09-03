using CarpetProject.EntityDto.ProductImages;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetProject.FluentValidations.ProductImages
{
    public class CreatImageDtoValidator :AbstractValidator<CreateImageDto>
    {
        public CreatImageDtoValidator()
        {
            RuleFor(x => x.CategoryId)
                 .NotNull().When(x => x.ProductId == null).WithMessage("Kategori ID gereklidir.")
                 .Must((dto, categoryId) => categoryId != null || dto.ProductId != null)
                 .WithMessage("Kategori ID veya Ürün ID'den en az biri girilmelidir.");

            RuleFor(x => x.ProductId)
                .NotNull().When(x => x.CategoryId == null).WithMessage("Ürün ID gereklidir.")
                .Must((dto, productId) => productId != null || dto.CategoryId != null)
                .WithMessage("Kategori ID veya Ürün ID'den en az biri girilmelidir.");


        
        }

   
    }
}
