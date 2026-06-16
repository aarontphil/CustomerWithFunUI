using JustCRM.API.Entities;

namespace JustCRM.API.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetCustomerDetailsAsync(long id);
        Task<(IEnumerable<Customer> Customers, int TotalCount)> GetPagedCustomersAsync(
            string? search, 
            long? industryId, 
            string? sortBy, 
            bool isDescending, 
            int pageNumber, 
            int pageSize);
    }
}
