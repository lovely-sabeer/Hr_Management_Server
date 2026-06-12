namespace RestAPI.Exceptions
{
    public class AppExceptions : Exception
    {
        public string Title { get; }
        public int? StatusCode { get; }

        public AppExceptions(string title, string message, int? statusCode = 400)
            : base(message)
        {
            Title = title;
            StatusCode = statusCode;
        }
        public AppExceptions(string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
            Title = GetTitleForStatusCode(statusCode);
        } 

        public AppExceptions(string title, string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
            Title = title;
        }

        public AppExceptions(string message, int statusCode, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            Title = GetTitleForStatusCode(statusCode);
        }

        private string GetTitleForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "Validation Error",
                401 => "Unauthorized",
                403 => "Not Permitted",
                404 => "Not Found",
                409 => "Already Exists",
                500 => "Internal Server Error",
                _ => "Application Error"
            };
        }
    }
}
