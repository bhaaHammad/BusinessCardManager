using BusinessCardManager.Application.DTOs.BusinessCards;
using BusinessCardManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCardManager.API.Controllers
{
    [Route("api/business-cards")]
    public class BusinessCardsController : ApiController
    {
        private readonly IBusinessCardService _businessCardService;

        public BusinessCardsController(IBusinessCardService businessCardService)
        {
            _businessCardService = businessCardService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBusinessCard([FromForm] BusinessCardRequestDto BusinessCardRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _businessCardService.CreateBusinessCardAsync(BusinessCardRequestDto);
            return HandleResponse(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetBusinessCards([FromQuery] BusinessCardFilterDto? filter)
        {
            var businessCards = await _businessCardService.GetAllBusinessCardsAsync(filter);
            return HandleResponse(businessCards);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusinessCard(int id)
        {
            var deleted = await _businessCardService.DeleteBusinessCardAsync(id);
            return HandleResponse(deleted);
        }
    }
}
