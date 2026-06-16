using JustCRM.API.DTOs;
using JustCRM.API.Repositories;

namespace JustCRM.API.Services
{
    public class IndustryService : IIndustryService
    {
        private readonly IIndustryRepository _industryRepository;

        public IndustryService(IIndustryRepository industryRepository)
        {
            _industryRepository = industryRepository;
        }

        public async Task<IEnumerable<IndustryDto>> GetAllIndustriesAsync()
        {
            var industries = await _industryRepository.GetAllAsync();
            return industries.Select(i => new IndustryDto
            {
                IndustryId = i.IndustryId,
                IndustryName = i.IndustryName,
                IsActive = i.IsActive
            });
        }

        public async Task<IndustryDto?> GetIndustryByIdAsync(long id)
        {
            var industry = await _industryRepository.GetByIdAsync(id);
            if (industry == null)
                return null;

            return new IndustryDto
            {
                IndustryId = industry.IndustryId,
                IndustryName = industry.IndustryName,
                IsActive = industry.IsActive
            };
        }
    }
}
