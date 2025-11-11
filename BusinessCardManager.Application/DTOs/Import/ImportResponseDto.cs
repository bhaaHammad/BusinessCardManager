namespace BusinessCardManager.Application.DTOs.Import
{
    public class ImportResponseDto
    {
        public int ImportedCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
