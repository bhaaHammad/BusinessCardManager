using BusinessCardManager.Application.Common;
using BusinessCardManager.Application.DTOs.BusinessCards;
using BusinessCardManager.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Text.Json;
using System.Text.RegularExpressions;
using ZXing;
using ZXing.Windows.Compatibility;

namespace BusinessCardManager.Application.Services
{
    public class QrDecoderService : IQrDecoderService
    {
        public async Task<Response<BusinessCardRequestDto>> ImportQrAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Response<BusinessCardRequestDto>.FailureResponse(Messages.QrFileRequired);

            try
            {
                await using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                ms.Position = 0;

                var dto = Decode(ms);

                if (dto == null)
                    return Response<BusinessCardRequestDto>.FailureResponse(Messages.QrDecodeFailed);

                return Response<BusinessCardRequestDto>.SuccessResponse(dto, Messages.QrDecodeSuccess);
            }
            catch (Exception ex)
            {
                return Response<BusinessCardRequestDto>.FailureResponse(
                    string.Format(Messages.QrProcessingError, ex.Message));
            }
        }

        public BusinessCardRequestDto? Decode(Stream imageStream)
        {
            using var bmp = new Bitmap(imageStream);

            var reader = new BarcodeReader
            {
                AutoRotate = true,
                Options = new ZXing.Common.DecodingOptions
                {
                    TryHarder = true,
                    PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE }
                }
            };

            var result = reader.Decode(bmp);
            if (result == null || string.IsNullOrWhiteSpace(result.Text))
                return null;

            return ParseToDto(result.Text);
        }

        private static BusinessCardRequestDto ParseToDto(string text)
        {
            var dto = new BusinessCardRequestDto();

            try
            {
                var jsonDto = JsonSerializer.Deserialize<BusinessCardRequestDto>(text,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (jsonDto != null) return jsonDto;
            }
            catch { }

            if (text.StartsWith("BEGIN:VCARD", StringComparison.OrdinalIgnoreCase))
            {
                var lines = Regex.Split(text, @"\r?\n").Select(l => l.Trim());
                foreach (var line in lines)
                {
                    if (line.StartsWith("FN:", StringComparison.OrdinalIgnoreCase))
                        dto.Name = line[3..].Trim();
                    else if (Regex.IsMatch(line, @"^TEL", RegexOptions.IgnoreCase))
                    {
                        var idx = line.IndexOf(':');
                        if (idx > -1) dto.Phone = line[(idx + 1)..].Trim();
                    }
                    else if (Regex.IsMatch(line, @"^EMAIL", RegexOptions.IgnoreCase))
                    {
                        var idx = line.IndexOf(':');
                        if (idx > -1) dto.Email = line[(idx + 1)..].Trim();
                    }
                    else if (line.StartsWith("ADR:", StringComparison.OrdinalIgnoreCase))
                        dto.Address = line[4..].Trim();
                }

                return dto;
            }

            var kv = Regex.Matches(text, @"\b([^=;]+)=([^;]+)");
            if (kv.Count > 0)
            {
                foreach (Match m in kv.Cast<Match>())
                {
                    var key = m.Groups[1].Value.Trim().ToLowerInvariant();
                    var val = m.Groups[2].Value.Trim();

                    switch (key)
                    {
                        case "name":
                        case "fullname": dto.Name = val; break;
                        case "gender": dto.Gender = val; break;
                        case "dob":
                        case "dateofbirth":
                            if (DateTime.TryParse(val, out var dob))
                                dto.DateOfBirth = dob;
                            break;
                        case "email": dto.Email = val; break;
                        case "phone": dto.Phone = val; break;
                        case "address": dto.Address = val; break;
                    }
                }
                return dto;
            }

            dto.Name = text;
            return dto;
        }
    }
}
