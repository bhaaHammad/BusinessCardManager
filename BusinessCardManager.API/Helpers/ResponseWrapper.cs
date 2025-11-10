using System.Text.Json.Serialization;

namespace BusinessCardManager.API.Helpers
{
    public class ResponseWrapper<T>
    {
        [JsonPropertyName("status")]
        public string Status { get; }

        [JsonPropertyName("message")]
        public string Message { get; }

        [JsonPropertyName("data")]
        public T Data { get; }

        [JsonPropertyName("errors")]
        public List<string> Errors { get; }

        public DateTimeOffset TimeGenerated { get; }

        protected internal ResponseWrapper(string status, string message, T data, List<string> errors)
        {
            Status = status;
            Message = message;
            Data = data;
            Errors = errors ?? new List<string>();
            TimeGenerated = DateTime.UtcNow;
        }
    }

    public class ResponseWrapper : ResponseWrapper<object>
    {
        protected ResponseWrapper(string status, string message, List<string> errors)
            : base(status, message, null, errors)
        {
        }

        public static ResponseWrapper<T> Ok<T>(T data, string message = "Request processed successfully.")
        {
            return new ResponseWrapper<T>("success", message, data, null);
        }

        public static ResponseWrapper Ok(string message = "Request processed successfully.")
        {
            return new ResponseWrapper("success", message, null);
        }

        public static ResponseWrapper Error(string message, List<string> errors)
        {
            return new ResponseWrapper("error", message, errors);
        }
    }
}
