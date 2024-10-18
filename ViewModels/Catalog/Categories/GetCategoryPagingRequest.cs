using ViewModels.Common;

namespace ViewModels.Catalog.Categories
{
    public class GetCategoryPagingRequest : BasePagingRequest
    {
        public string? Keyword { get; set; }
        public string LanguageId { get; set; }
    }
}