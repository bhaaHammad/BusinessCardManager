using BusinessCardManager.Application.Common;
using BusinessCardManager.Application.DTOs.BusinessCards;
using BusinessCardManager.Application.DTOs.Import;

namespace BusinessCardManager.Application.Interfaces
{
    public interface IBusinessCardService
    {
        Task<Response<BusinessCardResponseDto>> CreateBusinessCardAsync(BusinessCardRequestDto businessCardRequestDto);

        Task<Response<BulkBusinessCardResponseDto>> BulkCreateBusinessCardAsync(BulkBusinessCardRequestDto request);

        Task<Response<List<BusinessCardResponseDto>>> GetAllBusinessCardsAsync(BusinessCardFilterDto? filter = null);
        Task<Response<List<BusinessCardResponseDto>>> GetAllBusinessCardsForExportAsync(BusinessCardFilterDto? filter = null);
        Task<Response<bool>> DeleteBusinessCardAsync(int id);
    }
}
