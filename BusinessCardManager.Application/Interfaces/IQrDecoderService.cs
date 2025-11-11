using BusinessCardManager.Application.Common;
using BusinessCardManager.Application.DTOs.BusinessCards;
using Microsoft.AspNetCore.Http;

namespace BusinessCardManager.Application.Interfaces
{
    public interface IQrDecoderService
    {
        Task<Response<BusinessCardRequestDto>> ImportQrAsync(IFormFile file);
    }
}
