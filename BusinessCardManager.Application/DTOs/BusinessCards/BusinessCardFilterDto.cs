using BusinessCardManager.Application.Common.Filters;

namespace BusinessCardManager.Application.DTOs.BusinessCards
{
    public class BusinessCardFilterDto : PaginationFilter
    {
        public string? Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
    }
}
