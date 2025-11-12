namespace BusinessCardManager.Application.DTOs.Import
{
    public class BulkBusinessCardResponseDto
    {
        public int InsertedCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
