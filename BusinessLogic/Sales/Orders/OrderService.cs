using Common.Enums;
using Common.Exceptions;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using ViewModels.Common;
using ViewModels.Sales.Orders;

namespace BusinessLogic.Sales.Orders
{
    public class OrderService : IOrderService
    {
        private readonly EStoreDbContext _context;

        public OrderService(EStoreDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// tạo đơn hàng
        /// </summary>
        /// <param name="request"></param>
        /// <returns>created order</returns>
        public async Task<int> Create(OrderCreateRequest request)
        {
            var orderDetails = new List<OrderDetail>();
            foreach (var item in request.OrderDetails)
            {
                var orderDetail = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                };
                orderDetails.Add(orderDetail);
            }

            var order = new Order()
            {
                OrderDate = request.OrderDate,
                UserId = request.UserId,
                ShipName = request.ShipName,
                ShipAddress = request.ShipAddress,
                ShipEmail = request.ShipEmail,
                ShipPhoneNumber = request.ShipPhoneNumber,
                Status = OrderStatus.Pending,
                OrderDetails = orderDetails
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order.Id;
        }

        /// <summary>
        /// lấy api đơn hàng dưới dạng phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PagedResponse<OrderDto>> GetAll(GetOrderPagingRequest request)
        {
            // 1. Select join
            var query = from o in _context.Orders
                        join u in _context.Users on o.UserId equals u.Id into u1
                        from u in u1.DefaultIfEmpty()
                        select new { o, u };

            // 2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.o.ShipName.Contains(request.Keyword) ||
                                    x.o.ShipAddress.Contains(request.Keyword) ||
                                    x.o.ShipEmail.Contains(request.Keyword) ||
                                    x.o.ShipPhoneNumber.Contains(request.Keyword));

            // 3. Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new OrderDto()
                {
                    Id = x.o.Id,
                    OrderDate = x.o.OrderDate,
                    User = x.u.UserName ?? "Guest",
                    ShipAddress = x.o.ShipAddress,
                    ShipEmail = x.o.ShipEmail,
                    ShipName = x.o.ShipName,
                    ShipPhoneNumber = x.o.ShipPhoneNumber,
                    Status = x.o.Status
                }).OrderByDescending(d => d.OrderDate).ToListAsync();

            // 4. Select and projection
            var pagedResult = new PagedResponse<OrderDto>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Data = data
            };
            return pagedResult;
        }

        /// <summary>
        /// lấy đơn hàng theo mã
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<ApiResponse<OrderDto>> GetById(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == order.UserId);
                var orderDto = new OrderDto()
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    User = user.UserName ?? "Guest",
                    ShipAddress = order.ShipAddress,
                    ShipEmail = order.ShipEmail,
                    ShipName = order.ShipName,
                    ShipPhoneNumber = order.ShipPhoneNumber,
                    Status = order.Status,
                };

                return new ApiSuccessResponse<OrderDto>(orderDto);
            }

            return new ApiErrorResponse<OrderDto>($"Không thể tìm thấy đơn hàng với id ({orderId})");
        }

        /// <summary>
        /// cập nhật trạng thái của đơn hàng
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        /// <exception cref="ElectronixException"></exception>
        public async Task<bool> UpdateStatus(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new NotFoundException("Order", orderId);

            order.Status = newStatus;

            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// xóa đơn hàng
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        /// <exception cref="ElectronixException"></exception>
        public async Task<int> Delete(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new NotFoundException("Order", orderId);

            var orderDetails = await _context.OrderDetails.Where(x => x.OrderId == orderId).ToListAsync();
            foreach (var item in orderDetails)
            {
                _context.OrderDetails.Remove(item);
            }

            _context.Orders.Remove(order);
            return await _context.SaveChangesAsync();
        }
    }
}