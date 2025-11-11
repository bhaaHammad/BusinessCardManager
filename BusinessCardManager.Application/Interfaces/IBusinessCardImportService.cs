using BusinessCardManager.Application.Common;
using BusinessCardManager.Application.DTOs.Import;
using Microsoft.AspNetCore.Http;


namespace BusinessCardManager.Application.Interfaces
{
    public interface IBusinessCardImportService
    {
        Task<Response<PreviewResponseDto>> PreviewCsvAsync(IFormFile file);
        Task<Response<PreviewResponseDto>> PreviewXmlAsync(IFormFile file);
        Task<Response<ImportResponseDto>> CommitImportAsync(ImportRequestDto request);
    }
}
