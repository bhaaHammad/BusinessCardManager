using BusinessCardManager.Application.DTOs.BusinessCards;
using BusinessCardManager.Application.Services;

namespace BusinessCardManager.Tests.Services
{
    public class BusinessCardExportServiceTests
    {
        private readonly BusinessCardExportService _svc = new();

        public record Sample(int Id, string Name, string Email);

        [Fact]
        public async Task ExportToCsvAsync_Empty_ReturnsEmptyString()
        {
            var csv = await _svc.ExportToCsvAsync<Sample>(new List<Sample>());
            Assert.Equal(string.Empty, csv);
        }

        [Fact]
        public async Task ExportToCsvAsync_WritesHeadersAndRows()
        {
            var items = new List<Sample>
            {
                new Sample(1, "Alice", "alice@example.com"),
                new Sample(2, "Bob", "bob@example.com")
            };

            var csv = await _svc.ExportToCsvAsync(items);

            Assert.Contains("Id,Name,Email", csv);
            Assert.Contains("1,Alice,alice@example.com", csv);
            Assert.Contains("2,Bob,bob@example.com", csv);
        }

        [Fact]
        public async Task ExportToXmlAsync_SerializesList()
        {
            var items = new List<BusinessCardResponseDto>
            {
                new BusinessCardResponseDto
                {
                    Id = 1,
                    Name = "Alice",
                    Email = "alice@example.com"
                }
            };

            var xml = await _svc.ExportToXmlAsync(items);

            Assert.Contains("<?xml version", xml);

            Assert.Contains("<Id>1</Id>", xml);
            Assert.Contains("<Name>Alice</Name>", xml);
            Assert.Contains("<Email>alice@example.com</Email>", xml);

            var doc = System.Xml.Linq.XDocument.Parse(xml);
            Assert.NotNull(doc.Root);
        }
    }
}