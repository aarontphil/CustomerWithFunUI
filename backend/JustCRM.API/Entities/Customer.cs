using System.ComponentModel.DataAnnotations;

namespace JustCRM.API.Entities
{
    public class Customer
    {
        public long CustomerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public CustomerContact? CustomerContact { get; set; }
        public CustomerEmployment? CustomerEmployment { get; set; }
        public ICollection<CustomerDocument> CustomerDocuments { get; set; } = new List<CustomerDocument>();
    }
}
