using AutoMapper;
using CarpetProject.EntityDtos.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace CarpetProject.Products
{
    public class certificateManager : DomainService
    {
        private readonly IRepository<certificate, int> _tagRepository;
        private readonly IMapper _mapper;

        public certificateManager(IRepository<certificate, int> tagRepository, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        // Etiket oluşturma
        public async Task<CertificateDto> CreateAsync(CreateCertificateDto input)
        {
            var tag = _mapper.Map<certificate>(input);
            await _tagRepository.InsertAsync(tag);
            return _mapper.Map<CertificateDto>(tag);
        }

        // Etiketi güncelleme
        public async Task<CertificateDto> UpdateAsync(int id, UpdateCertificateDto input)
        {
            var tag = await _tagRepository.GetAsync(id);
            _mapper.Map(input, tag); // input DTO'sunu mevcut tag'e uygula
            await _tagRepository.UpdateAsync(tag);
            return _mapper.Map<CertificateDto>(tag);
        }

        // Etiket silme
        public async Task DeleteAsync(int id)
        {
            var tag = await _tagRepository.GetAsync(id);
            await _tagRepository.DeleteAsync(tag);
        }

        // Etiketleri listeleme
        public async Task<List<CertificateDto>> GetListAsync()
        {
            var tags = await _tagRepository.GetListAsync();
            return _mapper.Map<List<CertificateDto>>(tags);
        }

        // Tek bir etiketi alma
        public async Task<CertificateDto> GetAsync(int id)
        {
            var tag = await _tagRepository.GetAsync(id);
            return _mapper.Map<CertificateDto>(tag);
        }

        // Hard Delete - Kalıcı silme
        public async Task HartDeleteAsync(int id)
        {
            var tag = await _tagRepository.GetAsync(id);
            await _tagRepository.DeleteAsync(tag);
        }
    }
}
