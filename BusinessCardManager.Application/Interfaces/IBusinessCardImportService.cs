using BusinessCardManager.Application.Common;
using BusinessCardManager.Application.DTOs.ImportCsv;
using Microsoft.AspNetCore.Http;


namespace BusinessCardManager.Application.Interfaces
{
    public interface IBusinessCardImportService
    {
        Task<Response<CsvPreviewResponseDto>> PreviewCsvAsync(IFormFile file);
        Task<Response<CsvImportResponseDto>> CommitImportAsync(CsvImportRequestDto request);
    }
}
