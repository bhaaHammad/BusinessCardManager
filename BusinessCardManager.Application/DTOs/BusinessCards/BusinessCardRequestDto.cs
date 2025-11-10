namespace BusinessCardManager.Application.DTOs.BusinessCards
{
    public class BusinessCardRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Photo { get; set; }
        public string? Address { get; set; }
    }
}
