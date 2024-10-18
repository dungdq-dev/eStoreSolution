using ApiIntegration;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Sales.OrderDetails;
using ViewModels.Sales.Orders;

namespace AdminApp.Controllers
{
    [Controller]
    public class OrderController : ProtectedController
    {
        private readonly IOrderApiClient _orderApiClient;

        public OrderController(IOrderApiClient orderApiClient)
        {
            _orderApiClient = orderApiClient;
        }

        public async Task<IActionResult> Index(string keyword = "", int pageIndex = 1, int pageSize = 25)
        {
            var request = new GetOrderPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _orderApiClient.GetAll(request);
            ViewBag.Keyword = keyword;

            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Information(int id)
        {
            var result = await _orderApiClient.GetById(id);
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, int pageIndex = 1, int pageSize = 25)
        {
            var request = new GetOrderDetailsPagingRequest()
            {
                OrderId = id,
                PageIndex = pageIndex,
                PageSize = pageSize,
            };
            var data = await _orderApiClient.GetDetails(request);

            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> EditStatus(int id)
        {
            var order = await _orderApiClient.GetById(id);
            var updateRequest = new OrderStatusUpdateRequest()
            {
                Id = order.Data.Id,
                Status = order.Data.Status,
            };

            return View(updateRequest);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditStatus([FromForm] OrderStatusUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var result = await _orderApiClient.UpdateStatus(request);
            if (result)
            {
                TempData["result"] = "Cập nhật trạng thái đơn hàng thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Cập nhật trạng thái đơn hàng thất bại");
            return View(request);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            return View(new OrderDeleteRequest()
            {
                Id = id
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(OrderDeleteRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _orderApiClient.Delete(request.Id);
            if (result)
            {
                TempData["result"] = "Xóa bản ghi đơn hàng thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Xóa bản ghi đơn hàng thất bại");
            return View(request);
        }
    }
}
