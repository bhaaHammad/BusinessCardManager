namespace BusinessCardManager.Application.Common
{
    public class Response<T>
    {
        public T? Result { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> ErrorList { get; set; }

        public Response() { }
        private Response(T? result, string message = "", bool success = true, List<string>? errorList = null)
        {
            Result = result;
            Success = success;
            Message = message;
            ErrorList = errorList ?? new List<string>();
        }

        public static Response<T> SuccessResponse(T result, string message = "")
        {
            return new Response<T>(result, message, true);
        }

        public static Response<T> FailureResponse(string message, List<string>? errorList = null)
        {
            T result = default(T);
            return new Response<T>(result, message, false, errorList);
        }
    }
}
