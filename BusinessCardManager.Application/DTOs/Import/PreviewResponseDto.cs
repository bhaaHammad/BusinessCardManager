using BusinessCardManager.Application.DTOs.BusinessCards;

namespace BusinessCardManager.Application.DTOs.Import
{
    public class PreviewResponseDto
    {
        public List<BusinessCardRequestDto> Cards { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public int TotalRows { get; set; }
        public int ValidRows { get; set; }
        public int InvalidRows { get; set; }
    }
}