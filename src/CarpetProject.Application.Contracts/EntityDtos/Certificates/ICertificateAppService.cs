using CarpetProject.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;


namespace CarpetProject.EntityDtos.Tags
{
    public interface ICertificateAppService : ICrudAppService<CertificateDto, int, PagedAndSortedResultRequestDto, CreateCertificateDto, UpdateCertificateDto>
    {
        Task HartDeleteAsync(int id);
        
    }
}
