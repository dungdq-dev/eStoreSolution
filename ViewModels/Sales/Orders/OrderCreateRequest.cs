using Common.Enums;
using ViewModels.Sales.OrderDetails;

namespace ViewModels.Sales.Orders
{
    public class OrderCreateRequest
    {
        public DateTime OrderDate { get; set; }
        public Guid? UserId { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string ShipEmail { get; set; }
        public string ShipPhoneNumber { get; set; }
        public OrderStatus Status { get; set; }

        public List<OrderDetailCreateRequest> OrderDetails { get; set; }
    }
}