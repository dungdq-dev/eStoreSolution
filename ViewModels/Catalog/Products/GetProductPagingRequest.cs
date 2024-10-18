using ViewModels.Common;

namespace ViewModels.Catalog.Products
{
    public class GetProductPagingRequest : BasePagingRequest
    {
        public string? Keyword { get; set; }

        public string LanguageId { get; set; }

        public int? CategoryId { get; set; }
    }

    public class GetPublicProductPagingRequest : BasePagingRequest
    {
        public int? CategoryId { get; set; }
    }
}