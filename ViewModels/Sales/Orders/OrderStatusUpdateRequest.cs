using Common.Enums;

namespace ViewModels.Sales.Orders
{
    public class OrderStatusUpdateRequest
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
    }
}