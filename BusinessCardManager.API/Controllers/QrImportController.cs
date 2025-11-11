using BusinessCardManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCardManager.API.Controllers
{
    [Route("api/qr")]
    public class QrImportController : ApiController
    {
        private readonly IQrDecoderService _qrDecoder;

        public QrImportController(IQrDecoderService qrDecoder)
        {
            _qrDecoder = qrDecoder;
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import([FromForm] IFormFile file)
        {
            var response = await _qrDecoder.ImportQrAsync(file);
            return HandleResponse(response);
        }
    }
}
