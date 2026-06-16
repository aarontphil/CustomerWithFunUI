using System.ComponentModel.DataAnnotations;

namespace JustCRM.API.Entities
{
    public class CustomerDocument
    {
        public long DocumentId { get; set; }

        [Required]
        public long CustomerId { get; set; }

        // Navigation property
        public Customer? Customer { get; set; }

        [Required]
        [MaxLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
