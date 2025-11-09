using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BusinessCardManager.API.Helpers
{
    public class ExceptionStatusCodeMapper
    {
        public static HttpStatusCode GetStatusCode(Exception exception)
        {
            return exception switch
            {
                KeyNotFoundException => HttpStatusCode.NotFound,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                InvalidOperationException => HttpStatusCode.BadRequest,
                DbUpdateException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };
        }
    }
}
