using BusinessCardManager.API.Helpers;
using Microsoft.AspNetCore.Mvc;
using BusinessCardManager.Application.Common;
using CSharpFunctionalExtensions;

namespace BusinessCardManager.API.Controllers
{
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        #region Success Responses
        protected IActionResult OkResponse()
        {
            return base.Ok(ResponseWrapper.Ok());
        }

        protected IActionResult OkResponse<T>(T data, string message = null)
        {
            return base.Ok(ResponseWrapper.Ok(data, message));
        }

        #endregion

        #region Error Responses
        protected IActionResult ErrorResponse(string errorMessage, List<string> details = null)
        {
            return BadRequest(ResponseWrapper.Error(errorMessage, details ?? new List<string>()));
        }

        #endregion

        #region From Result Helpers
        protected IActionResult FromResult(Result result)
        {
            return result.IsSuccess
                ? OkResponse()
                : ErrorResponse(result.Error, new List<string> { result.Error });
        }

        protected IActionResult FromResult<T>(Result<T> result)
        {
            return result.IsSuccess
                ? OkResponse(result.Value)
                : ErrorResponse(result.Error, new List<string> { result.Error });
        }

        protected IActionResult FromResult<T>(Result<T> result, string successMessage)
        {
            return result.IsSuccess
                ? OkResponse(result.Value, successMessage)
                : ErrorResponse(result.Error, new List<string> { result.Error });
        }

        #endregion

        #region Handle Generic Response<T>
        protected IActionResult HandleResponse<T>(Response<T> response)
        {
            return response.Success
                ? OkResponse(response)
                : ErrorResponse(response.Message);
        }
        #endregion
    }
}
