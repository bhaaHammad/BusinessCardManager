using AutoMapper;
using BusinessCardManager.Application.DTOs.BusinessCards;
using BusinessCardManager.Application.Mapping;
using BusinessCardManager.Application.Services;
using BusinessCardManager.Domain.Entities;
using BusinessCardManager.Tests.TestDoubles;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BusinessCardManager.Tests.Services
{
    public class BusinessCardServiceTests
    {
        private readonly IMapper _mapper;
        private readonly ILogger<BusinessCardService> _logger = NullLogger<BusinessCardService>.Instance;

        public BusinessCardServiceTests()
        {
            var cfg = new MapperConfiguration(cfg => cfg.AddProfile(new BusinessCardProfile()));
            _mapper = cfg.CreateMapper();
        }

        private static IFormFile MakeFile(long sizeBytes)
        {
            var ms = new MemoryStream(new byte[sizeBytes]);
            return new FormFile(ms, 0, ms.Length, "photo", "photo.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }

        [Fact]
        public async Task CreateBusinessCardAsync_RejectsPhotoOver1MB()
        {
            var uow = new InMemoryUnitOfWork();
            var svc = new BusinessCardService(uow, _logger, _mapper);
            var dto = new BusinessCardRequestDto
            {
                Name = "Alice",
                Email = "alice@example.com",
                Phone = "123",
                Photo = MakeFile(1_200_000)
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                async () => await svc.CreateBusinessCardAsync(dto)
            );

            Assert.Equal(Messages.PhotoSizeExceeded, exception.Message);
        }

        [Fact]
        public async Task CreateBusinessCardAsync_PersistsValidCard()
        {
            var uow = new InMemoryUnitOfWork();
            var svc = new BusinessCardService(uow, _logger, _mapper);

            var dto = new BusinessCardRequestDto
            {
                Name = "Bob",
                Email = "bob@example.com",
                Phone = "555",
                Photo = null
            };

            var resp = await svc.CreateBusinessCardAsync(dto);

            Assert.True(resp.Success);
            Assert.Equal("Bob", resp.Result.Name);

            var repo = uow.GetInMemoryRepo<BusinessCard>();
            Assert.Single(repo.Items);
            Assert.Equal("bob@example.com", ((BusinessCard)repo.Items[0]).Email);
        }

        [Fact]
        public async Task DeleteBusinessCardAsync_ReturnsFailure_WhenNotFound()
        {
            var uow = new InMemoryUnitOfWork();
            var svc = new BusinessCardService(uow, _logger, _mapper);

            var resp = await svc.DeleteBusinessCardAsync(123);
            Assert.False(resp.Success);
            Assert.Contains("123", resp.Message);
        }
    }
}