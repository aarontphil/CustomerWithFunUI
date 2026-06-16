using JustCRM.API.Data;
using JustCRM.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace JustCRM.API.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetCustomerDetailsAsync(long id)
        {
            return await _context.Customers
                .Include(c => c.CustomerContact)
                .Include(c => c.CustomerEmployment)
                    .ThenInclude(ce => ce!.Industry)
                .Include(c => c.CustomerDocuments)
                .FirstOrDefaultAsync(c => c.CustomerId == id && c.IsActive);
        }



        public async Task<(IEnumerable<Customer> Customers, int TotalCount)> GetPagedCustomersAsync(
            string? search, 
            long? industryId, 
            string? sortBy, 
            bool isDescending, 
            int pageNumber, 
            int pageSize)
        {
            var query = _context.Customers
                .AsNoTracking()
                .Where(c => c.IsActive)
                .Include(c => c.CustomerContact)
                .Include(c => c.CustomerEmployment)
                    .ThenInclude(ce => ce!.Industry)
                .Include(c => c.CustomerDocuments)
                .AsQueryable();

            // 1. Searching (by Names, Contact Email/Phone, or Employment Company/JobTitle)
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(c => c.FirstName.ToLower().Contains(search) ||
                                         c.LastName.ToLower().Contains(search) ||
                                         c.FullName.ToLower().Contains(search) ||
                                         (c.CustomerContact != null &&
                                            ((c.CustomerContact.Email ?? string.Empty).ToLower().Contains(search) ||
                                             (c.CustomerContact.PrimaryPhone ?? string.Empty).Contains(search) ||
                                             (c.CustomerContact.SecondaryPhone ?? string.Empty).Contains(search))) ||
                                         (c.CustomerEmployment != null &&
                                            ((c.CustomerEmployment.CompanyName ?? string.Empty).ToLower().Contains(search) ||
                                             (c.CustomerEmployment.JobTitle ?? string.Empty).ToLower().Contains(search))));
            }

            // 2. Filtering (by Industry in Employment)
            if (industryId.HasValue)
            {
                query = query.Where(c => c.CustomerEmployment != null && c.CustomerEmployment.IndustryId == industryId.Value);
            }

            // 3. Total Count before pagination
            int totalCount = await query.CountAsync();

            // 4. Sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy.Trim().ToLower())
                {
                    case "firstname":
                        query = isDescending ? query.OrderByDescending(c => c.FirstName) : query.OrderBy(c => c.FirstName);
                        break;
                    case "lastname":
                        query = isDescending ? query.OrderByDescending(c => c.LastName) : query.OrderBy(c => c.LastName);
                        break;
                    case "fullname":
                        query = isDescending ? query.OrderByDescending(c => c.FullName) : query.OrderBy(c => c.FullName);
                        break;
                    case "createddate":
                        query = isDescending ? query.OrderByDescending(c => c.CreatedDate) : query.OrderBy(c => c.CreatedDate);
                        break;
                    default:
                        query = query.OrderBy(c => c.CustomerId);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(c => c.CustomerId); // Default sort
            }

            // 5. Pagination
            var customers = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (customers, totalCount);
        }
    }
}
