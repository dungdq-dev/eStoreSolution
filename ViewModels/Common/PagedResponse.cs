namespace ViewModels.Common
{
    public class PagedResponse<T> : BasePagedResponse
    {
        public List<T>? Data { set; get; }
    }
}