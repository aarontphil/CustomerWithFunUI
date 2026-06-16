using System.ComponentModel.DataAnnotations;

namespace JustCRM.API.DTOs
{
    public class CustomerDto
    {
        public long CustomerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public CustomerContactDto? Contact { get; set; }
        public CustomerEmploymentDto? Employment { get; set; }
        public List<CustomerDocumentDto> Documents { get; set; } = new List<CustomerDocumentDto>();
    }

    public class CustomerListDto
    {
        public long CustomerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? SecondaryPhone { get; set; }
        public string? Address { get; set; }
        public string? CompanyName { get; set; }
        public string? IndustryName { get; set; }
        public string? ProfileImageUrl { get; set; }
    }

    public class CustomerCreateUpdateDto
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        public CustomerContactCreateUpdateDto? Contact { get; set; }
        public CustomerEmploymentCreateUpdateDto? Employment { get; set; }
    }
}
