using BusinessCardManager.API.Controllers;
using BusinessCardManager.Application.DTOs.BusinessCards;
using BusinessCardManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;

[Route("api/business-cards/export")]
public class BusinessCardExportController : ApiController
{
    private readonly IBusinessCardExportService _exportService;
    private readonly IBusinessCardService _businessCardService;

    public BusinessCardExportController(
        IBusinessCardExportService exportService,
        IBusinessCardService businessCardService)
    {
        _exportService = exportService;
        _businessCardService = businessCardService;
    }

    [HttpGet("csv")]
    public async Task<IActionResult> ExportCsv([FromQuery] BusinessCardFilterDto filter)
    {
        var response = await _businessCardService.GetAllBusinessCardsForExportAsync(filter);

        var csv = await _exportService.ExportToCsvAsync(response.Result);

        var bom = Encoding.UTF8.GetPreamble();
        var bytes = bom.Concat(Encoding.UTF8.GetBytes(csv)).ToArray();

        return File(bytes, "text/csv", "BusinessCards.csv");
    }

    [HttpGet("xml")]
    public async Task<IActionResult> ExportXml([FromQuery] BusinessCardFilterDto filter)
    {
        var response = await _businessCardService.GetAllBusinessCardsForExportAsync(filter);

        var xml = await _exportService.ExportToXmlAsync<BusinessCardResponseDto>(response.Result);

        return File(Encoding.UTF8.GetBytes(xml), "application/xml", "BusinessCards.xml");
    }
}
