namespace Predictly_Api.Helpers
{
    public class ApiException
    {
        public ApiException(int statusCode, string message, dynamic error)
        {
            StatusCode = statusCode;
            Message = message;
            Error = error;
        }

        public ApiException(int statusCode, dynamic message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
        public dynamic Error { get; set; }
    }
}
