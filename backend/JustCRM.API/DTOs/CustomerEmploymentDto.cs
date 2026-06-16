using System.ComponentModel.DataAnnotations;

namespace JustCRM.API.DTOs
{
    public class CustomerEmploymentDto
    {
        public long EmploymentId { get; set; }
        public long CustomerId { get; set; }
        public long IndustryId { get; set; }
        public string? IndustryName { get; set; }
        public string? CompanyName { get; set; }
        public string? JobTitle { get; set; }
        public bool IsActive { get; set; }
    }

    public class CustomerEmploymentCreateUpdateDto
    {
        public long? EmploymentId { get; set; }

        [Required]
        public long IndustryId { get; set; }

        [MaxLength(200)]
        public string? CompanyName { get; set; }

        [MaxLength(150)]
        public string? JobTitle { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
