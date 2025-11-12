using BusinessCardManager.Application.DTOs.BusinessCards;

namespace BusinessCardManager.Application.DTOs.Import
{
    public class BulkBusinessCardRequestDto
    {
        public List<BusinessCardRequestDto> Cards { get; set; } = new List<BusinessCardRequestDto>();
    }
}