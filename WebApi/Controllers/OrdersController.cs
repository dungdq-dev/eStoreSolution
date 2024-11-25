using BusinessLogic.Catalog.Products;
using BusinessLogic.Common.Email;
using BusinessLogic.Sales.OrderDetails;
using BusinessLogic.Sales.Orders;
using Common.Enums;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ViewModels.Sales.OrderDetails;
using ViewModels.Sales.Orders;

namespace WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IProductService _productService;
        private readonly IEmailService _emailSender;

        public OrdersController(ILogger logger, IOrderService orderService, IOrderDetailService orderDetailService, IEmailService emailSender, IWebHostEnvironment webHostEnvironment, IProductService productService)
        {
            _logger = logger;
            _orderService = orderService;
            _orderDetailService = orderDetailService;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
            _productService = productService;
        }

        // POST: api/Orders
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Order))]
        [AllowAnonymous]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var orderId = await _orderService.Create(request);
            if (orderId == 0)
                return BadRequest();

            var order = await _orderService.GetById(orderId);
            var orderDetails = await _orderDetailService.GetOrderDetails(new GetOrderDetailsPagingRequest()
            {
                OrderId = orderId,
                PageIndex = 1,
                PageSize = int.MaxValue,
            });

            StringBuilder detailList = new StringBuilder();
            decimal totalPayment = 0;

            foreach (var item in orderDetails.Data)
            {
                var product = await _productService.GetById(item.ProductId, "vi");

                string detailBody = "<tr>" +
                "<td style=\"color:#636363;border:1px solid #e5e5e5;padding:12px;text-align:left;vertical-align:middle;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif;word-wrap:break-word\">" +
                $"{product.Data.Name}" +
                "</td>" +
                "<td style=\"color:#636363;border:1px solid #e5e5e5;padding:12px;text-align:left;vertical-align:middle;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif\">" +
                $"{item.Quantity}" +
                "</td>" +
                "<td style=\"color:#636363;border:1px solid #e5e5e5;padding:12px;text-align:left;vertical-align:middle;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif\">" +
                $"<span>{item.Price}&nbsp;<span>₫</span></span>" +
                "</td>" +
                "</tr>";

                // nội dung email chứa danh sách sản phẩm đã đặt
                detailList.Append(detailBody).ToString();

                // tính tổng số tiền phải thanh toán
                totalPayment += item.Price * item.Quantity;
            }

            try
            {
                await _emailSender.SendEmail(request.ShipEmail, "Xác nhận đơn hàng",
                    PopulateBody(order.Data.ShipName, order.Data.ShipAddress, order.Data.ShipEmail, order.Data.ShipPhoneNumber, detailList.ToString(), totalPayment));
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Failed to send email. Error message: " + ex);
            }
            finally
            {
                _logger.LogInformation("Người dùng đặt hàng thành công");
            }

            return Created();
        }

        // GET: /api/Orders?keyword=...&pageIndex=1&pageSize=25
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] GetOrderPagingRequest request)
        {
            var orders = await _orderService.GetList(request);
            return Ok(orders);
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        [ActionName(nameof(GetById))]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _orderService.GetById(id);
            if (result == null)
                return NotFound(result);

            return Ok(result);
        }

        // PATCH: api/Orders/5/NewStatus
        [HttpPatch("{orderId}/{newStatus}")]
        [Authorize]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] int orderId, OrderStatus newStatus)
        {
            var isSuccess = await _orderService.UpdateStatus(orderId, newStatus);
            if (!isSuccess)
                return BadRequest();

            return Ok();
        }

        // DELETE: api/Orders/5
        [HttpDelete("{orderId}")]
        [Authorize]
        public async Task<IActionResult> DeleteOrder([FromRoute] int orderId)
        {
            var order = await _orderService.Delete(orderId);
            if (order == 0)
                return BadRequest();

            return NoContent();
        }

        // GET: /api/Orders/details?orderId=1&pageIndex=1&pageSize=25
        [HttpGet("details")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrderDetails([FromQuery] GetOrderDetailsPagingRequest request)
        {
            var orderDetails = await _orderDetailService.GetOrderDetails(request);
            return Ok(orderDetails);
        }

        /// <summary>
        /// email body
        /// </summary>
        /// <param name="shipName"></param>
        /// <param name="shipAddress"></param>
        /// <param name="shipEmail"></param>
        /// <param name="shipPhoneNumber"></param>
        /// <param name="orderDetails"></param>
        /// <param name="totalPayment"></param>
        /// <returns></returns>
        private string PopulateBody(string shipName, string shipAddress, string shipEmail, string shipPhoneNumber, string orderDetails, decimal totalPayment)
        {
            string body = string.Empty;
            string path = Path.Combine(
                _webHostEnvironment.WebRootPath,
                "templates\\place_order_mail.html");
            using (StreamReader reader = new StreamReader(path))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{ShipName}", shipName);
            body = body.Replace("{ShipAddress}", shipAddress);
            body = body.Replace("{ShipEmail}", shipEmail);
            body = body.Replace("{ShipPhoneNumber}", shipPhoneNumber);
            body = body.Replace("{OrderDetails}", orderDetails);
            body = body.Replace("{TotalPayment}", totalPayment.ToString());

            return body;
        }
    }
}