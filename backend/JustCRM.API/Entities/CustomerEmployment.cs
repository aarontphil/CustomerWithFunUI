using System.ComponentModel.DataAnnotations;

namespace JustCRM.API.Entities
{
    public class CustomerEmployment
    {
        public long EmploymentId { get; set; }

        [Required]
        public long CustomerId { get; set; }

        // Navigation property
        public Customer? Customer { get; set; }

        [Required]
        public long IndustryId { get; set; }

        // Navigation property
        public Industry? Industry { get; set; }

        [MaxLength(200)]
        public string? CompanyName { get; set; }

        [MaxLength(150)]
        public string? JobTitle { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
