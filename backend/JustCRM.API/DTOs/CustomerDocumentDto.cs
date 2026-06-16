namespace JustCRM.API.DTOs
{
    public class CustomerDocumentDto
    {
        public long DocumentId { get; set; }
        public long CustomerId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
