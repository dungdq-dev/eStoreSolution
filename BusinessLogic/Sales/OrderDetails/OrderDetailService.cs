using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using ViewModels.Common;
using ViewModels.Sales.OrderDetails;

namespace BusinessLogic.Sales.OrderDetails
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly EStoreDbContext _context;

        public OrderDetailService(EStoreDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// thêm chi tiết đơn hàng
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<OrderDetail> AddOrderDetail(OrderDetailCreateRequest request)
        {
            var orderDetail = new OrderDetail()
            {
                OrderId = request.OrderId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Price = request.Price,
            };

            if (request.OrderId == 0)
            {
                throw new ArgumentException("Request is empty");
            }

            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
            {
                throw new ArgumentException("This product doesn't exist.");
            }

            _context.OrderDetails.Add(orderDetail);
            product.Stock -= request.Quantity;
            await _context.SaveChangesAsync();
            return orderDetail;
        }

        public async Task<PagedResponse<OrderDetailDto>> GetOrderDetails(GetOrderDetailsPagingRequest request)
        {
            var query = from od in _context.OrderDetails
                        join p in _context.Products on od.ProductId equals p.Id
                        where od.OrderId == request.OrderId
                        select new { od, p };

            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new OrderDetailDto()
                {
                    OrderId = x.od.OrderId,
                    ProductId = x.od.ProductId,
                    Quantity = x.od.Quantity,
                    Price = x.od.Price,
                }).ToListAsync();

            var result = new PagedResponse<OrderDetailDto>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Data = data
            };

            return result;
        }
    }
}