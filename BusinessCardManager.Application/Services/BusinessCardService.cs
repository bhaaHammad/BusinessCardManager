using AutoMapper;
using BusinessCardManager.Application.Common;
using BusinessCardManager.Application.DTOs.BusinessCards;
using BusinessCardManager.Application.Interfaces;
using BusinessCardManager.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace BusinessCardManager.Application.Services
{
    public class BusinessCardService : IBusinessCardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BusinessCardService> _logger;
        private readonly IMapper _mapper;

        public BusinessCardService(
            IUnitOfWork unitOfWork,
            ILogger<BusinessCardService> logger,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Response<BusinessCardResponseDto>> CreateBusinessCardAsync(BusinessCardRequestDto businessCardRequestDto)
        {
            if (!string.IsNullOrEmpty(businessCardRequestDto.Photo))
                ValidatePhotoSize(businessCardRequestDto.Photo);

            var businessCard = _mapper.Map<BusinessCard>(businessCardRequestDto);

            await _unitOfWork.Repository<BusinessCard>().AddAsync(businessCard);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(Messages.BusinessCardCreatedLog, businessCard.Id);

            var dto = _mapper.Map<BusinessCardResponseDto>(businessCard);
            return Response<BusinessCardResponseDto>.SuccessResponse(dto, Messages.BusinessCardCreated);
        }

        public async Task<Response<List<BusinessCardResponseDto>>> GetAllBusinessCardsAsync(BusinessCardFilterDto? filter = null)
        {
            Expression<Func<BusinessCard, bool>> predicate = bc =>
                (filter == null || string.IsNullOrEmpty(filter.Name) || bc.Name.Contains(filter.Name)) &&
                (filter == null || !filter.DateOfBirth.HasValue || (bc.DateOfBirth.HasValue && bc.DateOfBirth.Value.Date == filter.DateOfBirth.Value.Date)) &&
                (filter == null || string.IsNullOrEmpty(filter.Phone) || bc.Phone.Contains(filter.Phone)) &&
                (filter == null || string.IsNullOrEmpty(filter.Gender) || (bc.Gender != null && bc.Gender.Contains(filter.Gender))) &&
                (filter == null || string.IsNullOrEmpty(filter.Email) || bc.Email.Contains(filter.Email));

            var businessCards = await _unitOfWork.Repository<BusinessCard>()
                .GetAllAsync(predicate, filter.PageNumber, filter.PageSize);

            var dto = _mapper.Map<List<BusinessCardResponseDto>>(businessCards);

            string message = dto.Any()
                ? string.Format(Messages.BusinessCardsRetrieved, dto.Count)
                : Messages.NoBusinessCardsFound;

            return Response<List<BusinessCardResponseDto>>.SuccessResponse(dto, message);
        }


        public async Task<Response<bool>> DeleteBusinessCardAsync(int id)
        {
            var repository = _unitOfWork.Repository<BusinessCard>();
            var businessCard = await repository.GetByIdAsync(id);

            if (businessCard == null)
                return Response<bool>.FailureResponse(string.Format(Messages.BusinessCardNotFoundById, id));

            repository.Delete(businessCard);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(Messages.BusinessCardDeletedLog, businessCard.Id);

            return Response<bool>.SuccessResponse(true, Messages.BusinessCardDeleted);
        }

        #region private methods
        private static void ValidatePhotoSize(string base64Photo)
        {
            var base64String = base64Photo.Contains(",")
                ? base64Photo.Split(',')[1]
                : base64Photo;

            var sizeInBytes = (base64String.Length * 3) / 4;
            const int maxSizeInBytes = 1024 * 1024;

            if (sizeInBytes > maxSizeInBytes)
            {
                throw new ArgumentException(Messages.PhotoSizeExceeded);
            }
        }
        #endregion
    }
}
