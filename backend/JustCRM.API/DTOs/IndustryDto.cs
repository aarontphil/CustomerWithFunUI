namespace JustCRM.API.DTOs
{
    public class IndustryDto
    {
        public long IndustryId { get; set; }
        public string IndustryName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
