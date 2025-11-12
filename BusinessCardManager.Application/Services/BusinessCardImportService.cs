using AutoMapper;
using BusinessCardManager.Application.Common;
using BusinessCardManager.Application.DTOs.BusinessCards;
using BusinessCardManager.Application.DTOs.Import;
using BusinessCardManager.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace BusinessCardManager.Application.Services
{
    public class BusinessCardImportService : IBusinessCardImportService
    {
        private const string XmlRootArrayOfCards = "ArrayOfBusinessCardResponseDto";
        private const string Xml = "text/xml";
        private const string Csv = "text/csv";

        public async Task<Response<PreviewResponseDto>> PreviewAsync(IFormFile file)
        {
            if (file.ContentType == Xml)
            {
                return  await PreviewXmlAsync(file);
            }
            else if(file.ContentType == Csv)
            {
                return await PreviewCsvAsync(file);
            }
            throw new NotSupportedException();
        }

        #region methods 
        private async Task<Response<PreviewResponseDto>> PreviewCsvAsync(IFormFile file)
        {
            ValidateFile(file);

            var lines = await ReadCsvLinesAsync(file);
            var result = ParseCsvLines(lines);

            return Response<PreviewResponseDto>.SuccessResponse(result, Messages.CsvPreviewGenerated);
        }

        private async Task<Response<PreviewResponseDto>> PreviewXmlAsync(IFormFile file)
        {
            ValidateFile(file);

            List<BusinessCardResponseDto> responseCards;
            try
            {
                responseCards = await DeserializeXmlFileAsync(file);
            }
            catch (Exception ex)
            {
                return Response<PreviewResponseDto>.FailureResponse(string.Format(Messages.XmlParseError, ex.Message));
            }

            var cards = responseCards.Select(MapResponseToRequestDto).ToList();

            var result = new PreviewResponseDto();
            int rowNumber = 0;

            foreach (var card in cards)
            {
                rowNumber++;
                if (!IsValidCard(card))
                {
                    result.Errors.Add(string.Format(Messages.MissingRequiredFieldsRow, rowNumber));
                    continue;
                }

                result.Cards.Add(card);
            }

            result.TotalRows = rowNumber;
            result.ValidRows = result.Cards.Count;
            result.InvalidRows = result.Errors.Count;

            return Response<PreviewResponseDto>.SuccessResponse(result, Messages.CsvPreviewGenerated);
        }

        private void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException(Messages.CsvNoCardsToImport);
        }

        private async Task<List<string>> ReadCsvLinesAsync(IFormFile file)
        {
            var lines = new List<string>();

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(line))
                    lines.Add(line);
            }

            return lines;
        }

        private PreviewResponseDto ParseCsvLines(List<string> lines)
        {
            var result = new PreviewResponseDto();
            int rowNumber = 0;

            foreach (var line in lines)
            {
                rowNumber++;
                var columns = line.Split(',');

                try
                {
                    var card = MapColumnsToDto(columns);

                    if (!IsValidCard(card))
                    {
                        result.Errors.Add(string.Format(Messages.MissingRequiredFieldsRow, rowNumber));
                        continue;
                    }

                    result.Cards.Add(card);
                }
                catch (Exception ex)
                {
                    result.Errors.Add(string.Format(Messages.RowError, rowNumber, ex.Message));
                }
            }

            result.TotalRows = rowNumber;
            result.ValidRows = result.Cards.Count;
            result.InvalidRows = result.Errors.Count;

            return result;
        }

        private BusinessCardRequestDto MapColumnsToDto(string[] columns)
        {
            return new BusinessCardRequestDto
            {
                Name = columns.Length > 0 ? columns[0].Trim() : string.Empty,
                Gender = columns.Length > 1 ? columns[1].Trim() : null,
                DateOfBirth = columns.Length > 2 && DateTime.TryParseExact(
                    columns[2].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dob)
                    ? dob
                    : (DateTime?)null,
                Email = columns.Length > 3 ? columns[3].Trim() : string.Empty,
                Phone = columns.Length > 4 ? columns[4].Trim() : string.Empty,
                Address = columns.Length > 5 ? columns[5].Trim() : null,
            };
        }

        private bool IsValidCard(BusinessCardRequestDto card)
        {
            return !string.IsNullOrEmpty(card.Name) && !string.IsNullOrEmpty(card.Email);
        }

        private async Task<List<BusinessCardResponseDto>> DeserializeXmlFileAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

            var root = new XmlRootAttribute(XmlRootArrayOfCards);
            var serializer = new XmlSerializer(typeof(List<BusinessCardResponseDto>), root);

            return (List<BusinessCardResponseDto>?)serializer.Deserialize(reader) ?? new List<BusinessCardResponseDto>();
        }

        private BusinessCardRequestDto MapResponseToRequestDto(BusinessCardResponseDto rc)
        {
            return new BusinessCardRequestDto
            {
                Name = rc.Name,
                Gender = rc.Gender,
                DateOfBirth = rc.DateOfBirth,
                Email = rc.Email,
                Phone = rc.Phone,
                Address = rc.Address,
                Photo = null
            };
        }
        #endregion
    }
}
