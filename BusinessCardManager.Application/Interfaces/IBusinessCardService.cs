using BusinessCardManager.Application.Common;
using BusinessCardManager.Application.DTOs.BusinessCards;

namespace BusinessCardManager.Application.Interfaces
{
    public interface IBusinessCardService
    {
        Task<Response<BusinessCardResponseDto>> CreateBusinessCardAsync(BusinessCardRequestDto businessCardRequestDto);
        Task<Response<List<BusinessCardResponseDto>>> GetAllBusinessCardsAsync(BusinessCardFilterDto? filter = null);
        Task<Response<bool>> DeleteBusinessCardAsync(int id);
    }
}
