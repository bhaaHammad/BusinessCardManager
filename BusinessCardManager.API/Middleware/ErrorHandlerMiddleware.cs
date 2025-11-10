using System.Text.Json;
using BusinessCardManager.API.Helpers;
using BusinessCardManager.Application.DTOs.Errors;

namespace BusinessCardManager.API.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = ExceptionStatusCodeMapper.GetStatusCode(exception);

            _logger.LogError(exception, "TraceId: {TraceId} | Error: {Message}", context.TraceIdentifier, exception.Message);

            var errorResponse = new ErrorResponse
            {
                Error = new ErrorDetail
                {
                    Message = exception.Message,
                    StatusCode = (int)statusCode
                }
            };

            var payload = JsonSerializer.Serialize(errorResponse);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(payload);
        }
    }
}
