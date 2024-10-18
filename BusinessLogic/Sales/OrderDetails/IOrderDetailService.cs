using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Common;
using ViewModels.Sales.OrderDetails;

namespace BusinessLogic.Sales.OrderDetails
{
    public interface IOrderDetailService
    {
        Task<OrderDetail> AddOrderDetail(OrderDetailCreateRequest request);

        Task<PagedResponse<OrderDetailDto>> GetOrderDetails(GetOrderDetailsPagingRequest request);
    }
}
