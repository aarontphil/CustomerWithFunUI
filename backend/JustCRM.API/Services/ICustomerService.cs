using JustCRM.API.DTOs;
using Microsoft.AspNetCore.Http;

namespace JustCRM.API.Services
{
    public interface ICustomerService
    {
        Task<PagedResult<CustomerListDto>> GetPagedCustomersAsync(
            string? search, long? industryId, string? sortBy, bool isDescending, int pageNumber, int pageSize);
        Task<CustomerDto?> GetCustomerByIdAsync(long id);
        Task<CustomerDto> CreateCustomerAsync(CustomerCreateUpdateDto dto);
        Task<CustomerDto?> UpdateCustomerAsync(long id, CustomerCreateUpdateDto dto);
        Task<bool> SoftDeleteCustomerAsync(long id);
        Task<CustomerDocumentDto?> UploadCustomerDocumentAsync(long customerId, string documentType, IFormFile file);
        Task<bool> SoftDeleteCustomerDocumentAsync(long customerId, long documentId);
    }
}
