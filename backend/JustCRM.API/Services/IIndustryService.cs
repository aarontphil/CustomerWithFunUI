using JustCRM.API.DTOs;

namespace JustCRM.API.Services
{
    public interface IIndustryService
    {
        Task<IEnumerable<IndustryDto>> GetAllIndustriesAsync();
        Task<IndustryDto?> GetIndustryByIdAsync(long id);
    }
}
