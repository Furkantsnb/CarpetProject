using CarpetProject.EntityDto.ProductImages;
using CarpetProject.EntityDtos.Tags;
using CarpetProject.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace CarpetProject.Tags
{
    public class CertificateAppService : CrudAppService<certificate, CertificateDto, int, PagedAndSortedResultRequestDto, CreateCertificateDto, UpdateCertificateDto>, ICertificateAppService
    {
        private readonly certificateManager _tagManager;

        public CertificateAppService(IRepository<certificate, int> repository, certificateManager tagManager) : base(repository)
        {
            _tagManager = tagManager;   
        }
        public override async Task<CertificateDto> CreateAsync(CreateCertificateDto input)
        {
            return await _tagManager.CreateAsync(input);
        }

        public override async Task<CertificateDto> UpdateAsync(int id, UpdateCertificateDto input)
        {
            return await _tagManager.UpdateAsync(id, input);
        }

        public override async Task DeleteAsync(int id)
        {
            await _tagManager.DeleteAsync(id);
        }

        public override async Task<CertificateDto> GetAsync(int id)
        {
            return await _tagManager.GetAsync(id);
        }

        public override Task<PagedResultDto<CertificateDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return base.GetListAsync(input);
        }

        public async Task HartDeleteAsync(int id)
        {
            await Repository.HardDeleteAsync(c => c.Id == id);
        }
    }
}
