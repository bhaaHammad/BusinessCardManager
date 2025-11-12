using AutoMapper;
using BusinessCardManager.Application.Common;
using BusinessCardManager.Application.DTOs.BusinessCards;
using BusinessCardManager.Application.DTOs.Import;
using BusinessCardManager.Application.Interfaces;
using BusinessCardManager.Domain.Entities;
using Microsoft.AspNetCore.Http;
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
            var businessCard = await BuildBusinessCardAsync(businessCardRequestDto);

            await _unitOfWork.Repository<BusinessCard>().AddAsync(businessCard);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(Messages.BusinessCardCreatedLog, businessCard.Id);

            var dto = _mapper.Map<BusinessCardResponseDto>(businessCard);
            return Response<BusinessCardResponseDto>.SuccessResponse(dto, Messages.BusinessCardCreated);
        }

        public async Task<Response<BulkBusinessCardResponseDto>> BulkCreateBusinessCardAsync(BulkBusinessCardRequestDto request)
        {
            if (!IsValidRequest(request))
                return Response<BulkBusinessCardResponseDto>.FailureResponse(Messages.CsvNoCardsToImport);

            var (InsertedCount, errors) = await ProcessCardsAsync(request.Cards);

            var response = new BulkBusinessCardResponseDto
            {
                InsertedCount = InsertedCount,
                Errors = errors
            };

            return Response<BulkBusinessCardResponseDto>.SuccessResponse(response);
        }

        public async Task<Response<List<BusinessCardResponseDto>>> GetAllBusinessCardsAsync(BusinessCardFilterDto? filter = null)
        {
            var predicate = BuildFilterPredicate(filter);

            var businessCards = await _unitOfWork.Repository<BusinessCard>()
                .GetAllAsync(predicate, filter.PageNumber, filter.PageSize);

            var dto = _mapper.Map<List<BusinessCardResponseDto>>(businessCards);

            string message = dto.Any()
                ? string.Format(Messages.BusinessCardsRetrieved, dto.Count)
                : Messages.NoBusinessCardsFound;

            return Response<List<BusinessCardResponseDto>>.SuccessResponse(dto, message);
        }

        public async Task<Response<List<BusinessCardResponseDto>>> GetAllBusinessCardsForExportAsync(BusinessCardFilterDto? filter = null)
        {
            var predicate = BuildFilterPredicate(filter);

            var businessCards = await _unitOfWork.Repository<BusinessCard>()
                .GetAllAsync(predicate);

            var dto = _mapper.Map<List<BusinessCardResponseDto>>(businessCards);
            return Response<List<BusinessCardResponseDto>>.SuccessResponse(dto);
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

        private async Task<string?> ConvertPhotoToBase64Async(IFormFile? photo)
        {
            if (photo == null) return null;

            using var ms = new MemoryStream();
            await photo.CopyToAsync(ms);

            var base64 = Convert.ToBase64String(ms.ToArray());
            ValidatePhotoSize(base64);

            return base64;
        }

        private async Task<BusinessCard> BuildBusinessCardAsync(BusinessCardRequestDto dto)
        {
            var base64Photo = await ConvertPhotoToBase64Async(dto.Photo);

            return new BusinessCard
            {
                Name = dto.Name,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                Photo = base64Photo
            };
        }

        private Expression<Func<BusinessCard, bool>> BuildFilterPredicate(BusinessCardFilterDto? filter)
        {
            return bc =>
                (filter == null || string.IsNullOrEmpty(filter.Name) || bc.Name.Contains(filter.Name)) &&
                (filter == null || !filter.DateOfBirth.HasValue || (bc.DateOfBirth.HasValue && bc.DateOfBirth.Value.Date == filter.DateOfBirth.Value.Date)) &&
                (filter == null || string.IsNullOrEmpty(filter.Phone) || bc.Phone.Contains(filter.Phone)) &&
                (filter == null || string.IsNullOrEmpty(filter.Gender) || (bc.Gender != null && bc.Gender.Contains(filter.Gender))) &&
                (filter == null || string.IsNullOrEmpty(filter.Email) || bc.Email.Contains(filter.Email));
        }

        private bool IsValidRequest(BulkBusinessCardRequestDto request)
        {
            return request != null && request.Cards != null && request.Cards.Count > 0;
        }

        private async Task<(int InsertedCount, List<string> errors)> ProcessCardsAsync(List<BusinessCardRequestDto> cards)
        {

            int insertedCount = 0;
            var errors = new List<string>();

            foreach (var cardDto in cards)
            {
                if (!IsValidCard(cardDto, out var validationError))
                {
                    errors.Add(validationError);
                    continue;
                }

                try
                {
                    await AddCardAsync(cardDto);
                    insertedCount++;
                }
                catch (Exception ex)
                {
                    errors.Add(string.Format(Messages.CardError, cardDto.Name ?? "Unknown", ex.Message));
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return (insertedCount, errors);
        }

        private bool IsValidCard(BusinessCardRequestDto cardDto, out string error)
        {
            if (string.IsNullOrEmpty(cardDto.Name) || string.IsNullOrEmpty(cardDto.Email))
            {
                error = string.Format(Messages.MissingRequiredFieldsCard, cardDto.Name ?? "Unknown");
                return false;
            }

            error = null;
            return true;
        }

        private async Task AddCardAsync(BusinessCardRequestDto cardDto)
        {
            var card = _mapper.Map<BusinessCard>(cardDto);
            await _unitOfWork.Repository<BusinessCard>().AddAsync(card);
        }
        #endregion
    }
}
