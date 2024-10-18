using Common.Enums;
using ViewModels.Common;
using ViewModels.Sales.Orders;

namespace BusinessLogic.Sales.Orders
{
    public interface IOrderService
    {
        Task<int> Create(OrderCreateRequest request);

        Task<PagedResponse<OrderDto>> GetAll(GetOrderPagingRequest request);

        Task<ApiResponse<OrderDto>> GetById(int orderId);

        Task<bool> UpdateStatus(int orderId, OrderStatus newStatus);

        Task<int> Delete(int orderId);
    }
}