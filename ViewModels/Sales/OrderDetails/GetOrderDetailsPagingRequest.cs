using ViewModels.Common;

namespace ViewModels.Sales.OrderDetails
{
    public class GetOrderDetailsPagingRequest : BasePagingRequest
    {
        public int? OrderId { get; set; }
    }
}