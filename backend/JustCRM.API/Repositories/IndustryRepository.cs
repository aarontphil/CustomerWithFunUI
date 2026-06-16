using JustCRM.API.Data;
using JustCRM.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace JustCRM.API.Repositories
{
    public class IndustryRepository : Repository<Industry>, IIndustryRepository
    {
        public IndustryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> IndustryExistsAsync(long id)
        {
            return await _context.Industries.AnyAsync(i => i.IndustryId == id);
        }
    }
}
