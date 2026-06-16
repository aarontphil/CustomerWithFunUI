using JustCRM.API.Entities;

namespace JustCRM.API.Repositories
{
    public interface IIndustryRepository : IRepository<Industry>
    {
        Task<bool> IndustryExistsAsync(long id);
    }
}
