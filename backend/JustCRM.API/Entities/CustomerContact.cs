using System.ComponentModel.DataAnnotations;

namespace JustCRM.API.Entities
{
    public class CustomerContact
    {
        public long ContactId { get; set; }

        [Required]
        public long CustomerId { get; set; }

        // Navigation property
        public Customer? Customer { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? PrimaryPhone { get; set; }

        [MaxLength(20)]
        public string? SecondaryPhone { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
