using System.ComponentModel.DataAnnotations;

namespace JustCRM.API.DTOs
{
    public class CustomerContactDto
    {
        public long ContactId { get; set; }
        public long CustomerId { get; set; }
        public string? Email { get; set; }
        public string? PrimaryPhone { get; set; }
        public string? SecondaryPhone { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
    }

    public class CustomerContactCreateUpdateDto
    {
        public long? ContactId { get; set; }

        [EmailAddress]
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
