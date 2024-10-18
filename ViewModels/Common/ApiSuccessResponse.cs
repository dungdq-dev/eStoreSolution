namespace ViewModels.Common
{
    public class ApiSuccessResponse<T> : ApiResponse<T>
    {
        public ApiSuccessResponse(T result)
        {
            IsSuccess = true;
            Data = result;
        }

        public ApiSuccessResponse()
        {
            IsSuccess = true;
        }
    }
}