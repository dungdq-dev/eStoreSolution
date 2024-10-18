namespace ViewModels.Common
{
    public class ApiErrorResponse<T> : ApiResponse<T>
    {
        public string[] ValidationErrors { get; set; }

        public ApiErrorResponse()
        {
        }

        public ApiErrorResponse(string message)
        {
            IsSuccess = false;
            Message = message;
        }

        public ApiErrorResponse(string[] validationErrors)
        {
            IsSuccess = false;
            ValidationErrors = validationErrors;
        }
    }
}