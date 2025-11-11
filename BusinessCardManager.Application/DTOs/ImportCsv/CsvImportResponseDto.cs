namespace BusinessCardManager.Application.DTOs.ImportCsv
{
    public class CsvImportResponseDto
    {
        public int ImportedCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
