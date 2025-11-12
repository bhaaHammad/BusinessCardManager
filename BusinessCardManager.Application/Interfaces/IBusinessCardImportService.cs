using BusinessCardManager.Application.Common;
using BusinessCardManager.Application.DTOs.Import;
using Microsoft.AspNetCore.Http;


namespace BusinessCardManager.Application.Interfaces
{
    public interface IBusinessCardImportService
    {
        Task<Response<PreviewResponseDto>> PreviewAsync(IFormFile file);
    }
}
