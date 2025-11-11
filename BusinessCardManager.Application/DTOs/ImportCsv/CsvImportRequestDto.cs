using BusinessCardManager.Application.DTOs.BusinessCards;

namespace BusinessCardManager.Application.DTOs.ImportCsv
{
    public class CsvImportRequestDto
    {
        public List<BusinessCardRequestDto> Cards { get; set; } = new List<BusinessCardRequestDto>();
    }
}