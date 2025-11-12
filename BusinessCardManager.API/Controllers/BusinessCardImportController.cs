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

        [HttpPost("preview")]
        public async Task<IActionResult> Preview([FromForm] IFormFile file)
        {
            var preview = await _importService.PreviewAsync(file);
            return HandleResponse(preview);
        }
    }
}
