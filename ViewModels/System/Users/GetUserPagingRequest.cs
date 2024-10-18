using ViewModels.Common;

namespace ViewModels.System.Users
{
    public class GetUserPagingRequest : BasePagingRequest
    {
        public string? Keyword { get; set; }
    }
}