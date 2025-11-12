using System.Text;
using AutoMapper;
using BusinessCardManager.Application.DTOs.BusinessCards;
using BusinessCardManager.Application.Mapping;
using BusinessCardManager.Application.Services;
using Microsoft.AspNetCore.Http;

namespace BusinessCardManager.Tests.Services
{
    public class BusinessCardImportServiceTests
    {
        public BusinessCardImportServiceTests()
        {
            var cfg = new MapperConfiguration(cfg => cfg.AddProfile(new BusinessCardProfile()));
        }

        private static IFormFile MakeFormFile(string content, string fileName, string contentType)
        {
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
            return new FormFile(ms, 0, ms.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }

        [Fact]
        public async Task PreviewAsync_Csv_MixesValidAndInvalidRows_ComputesCounts()
        {
            var csv = string.Join("\n", new[] {
                "Name,Gender,DateOfBirth,Email,Phone,Address",
                "Alice,F,1990-01-01,alice@example.com,123,Amman",
                ",M,1988-03-03,,555,Zarqa",
                "Bob,, ,bob@example.com,777,"
            });
            var file = MakeFormFile(csv, "cards.csv", "text/csv");
            var svc = new BusinessCardImportService();
            var result = await svc.PreviewAsync(file);

            Assert.True(result.Success);
            Assert.Equal(3, result.Result.TotalRows);
            Assert.Equal(2, result.Result.ValidRows);
            Assert.Equal(1, result.Result.InvalidRows);
            Assert.Equal(2, result.Result.Cards.Count);
            Assert.Single(result.Result.Errors);
        }

        [Fact]
        public async Task PreviewAsync_Xml_ParsesDtos()
        {
            var list = new List<BusinessCardResponseDto>
            {
                new BusinessCardResponseDto { Name = "Dana", Email = "dana@ex.com", Phone="000" }
            };

            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(List<BusinessCardResponseDto>));
            using var sw = new StringWriter();
            xmlSerializer.Serialize(sw, list);
            var xml = sw.ToString();

            var file = MakeFormFile(xml, "cards.xml", "text/xml");
            var svc = new BusinessCardImportService();

            var result = await svc.PreviewAsync(file);

            Assert.True(result.Success);
            Assert.Equal(1, result.Result.TotalRows);
            Assert.Equal(1, result.Result.ValidRows);
            Assert.Equal(0, result.Result.InvalidRows);
            Assert.Single(result.Result.Cards);
            Assert.Equal("Dana", result.Result.Cards.First().Name);
        }
    }
}