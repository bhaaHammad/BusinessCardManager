using BusinessCardManager.Application.DTOs.Import;
using BusinessCardManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCardManager.API.Controllers
{
    [Route("api/business-cards/import")]
    public class BusinessCardImportController : ApiController
    {
        private readonly IBusinessCardImportService _importService;

        public BusinessCardImportController(IBusinessCardImportService importService)
        {
            _importService = importService;
        }

        [HttpPost("csv/preview")]
        public async Task<IActionResult> PreviewCsv([FromForm] IFormFile file)
        {
            var preview = await _importService.PreviewCsvAsync(file);
            return HandleResponse(preview);
        }

        [HttpPost("commit")]
        public async Task<IActionResult> CommitImport([FromBody] ImportRequestDto request)
        {
            var response = await _importService.CommitImportAsync(request);
            return HandleResponse(response);
        }

        [HttpPost("xml/preview")]
        public async Task<IActionResult> PreviewXml([FromForm] IFormFile file)
        {
            var preview = await _importService.PreviewXmlAsync(file);
            return HandleResponse(preview);
        }
    }
}
