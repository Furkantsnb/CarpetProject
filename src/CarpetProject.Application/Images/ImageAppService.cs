using CarpetProject.Entities.Products;
using CarpetProject.EntityDto.ProductImages;
using CarpetProject.Products;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace CarpetProject.ProductImages
{
    public class ImageAppService : CrudAppService<Image, ImageDto, int, PagedAndSortedResultRequestDto, CreateImageDto, UpdateImageDto>, EntityDto.ProductImages.ImageAppService
    {
        private readonly ImageManager _productImageManager;

        public ImageAppService(IRepository<Image, int> repository, ImageManager productImageManager) : base(repository)
        {
            _productImageManager = productImageManager;
        }

        [Consumes("multipart/form-data")]
        public override async Task<ImageDto> CreateAsync([FromForm] CreateImageDto input)
        {
            return await _productImageManager.CreateAsync(input);
        }
        [Consumes("multipart/form-data")]
        public override async Task<ImageDto> UpdateAsync(int id, [FromForm] UpdateImageDto input)
        {
            return await _productImageManager.UpdateAsync(id, input);
        }

        public override async Task DeleteAsync(int id)
        {
            await _productImageManager.DeleteAsync(id);
        }

        public override async Task<ImageDto> GetAsync(int id)
        {
            return await _productImageManager.GetAsync(id);
        }

        public override async Task<PagedResultDto<ImageDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return await _productImageManager.GetListAsync(input);
        }

        public async Task HartDeleteAsync(int id)
        {
            await Repository.HardDeleteAsync(c => c.Id == id);
        }
    }
}
