using System.ComponentModel.DataAnnotations;

namespace JustCRM.API.Entities
{
    public class Industry
    {
        public long IndustryId { get; set; }

        [Required]
        [MaxLength(150)]
        public string IndustryName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation property
        public ICollection<CustomerEmployment> CustomerEmployments { get; set; } = new List<CustomerEmployment>();
    }
}
