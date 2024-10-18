using ViewModels.Common;

namespace ViewModels.Sales.Orders
{
    public class GetOrderPagingRequest : BasePagingRequest
    {
        public string? Keyword { get; set; }
    }
}