using Common.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using ViewModels.Common;
using ViewModels.Sales.OrderDetails;
using ViewModels.Sales.Orders;

namespace ApiIntegration
{
    public interface IOrderApiClient
    {
        Task<bool> Create(OrderCreateRequest request);

        Task<PagedResponse<OrderDto>> GetAll(GetOrderPagingRequest request);

        Task<ApiResponse<OrderDto>> GetById(int orderId);

        Task<bool> UpdateStatus(OrderStatusUpdateRequest request);

        Task<bool> Delete(int orderId);

        Task<bool> AddDetail(OrderDetailCreateRequest request);

        Task<PagedResponse<OrderDetailDto>> GetDetails(GetOrderDetailsPagingRequest request);
    }

    public class OrderApiClient : BaseApiClient, IOrderApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public OrderApiClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<bool> Create(OrderCreateRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            var response = await client.PostAsync("/api/orders", httpContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<PagedResponse<OrderDto>> GetAll(GetOrderPagingRequest request)
        {
            var data = await GetAsync<PagedResponse<OrderDto>>(
                $"/api/orders" +
                $"?keyword={request.Keyword}" +
                $"&pageIndex={request.PageIndex}" +
                $"&pageSize={request.PageSize}");

            return data;
        }

        public async Task<ApiResponse<OrderDto>> GetById(int orderId)
        {
            return await GetAsync<ApiResponse<OrderDto>>($"/api/orders/{orderId}");
        }

        public async Task<bool> UpdateStatus(OrderStatusUpdateRequest request)
        {
            var sessions = _httpContextAccessor
                .HttpContext
                .Session
                .GetString(SystemConstants.AppSettings.Token);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent
            {
                { new StringContent(request.Id.ToString()), "id" },
                { new StringContent(request.Status.ToString()), "status" }
            };

            var response = await client.PatchAsync($"/api/orders/{request.Id}/{request.Status}", requestContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Delete(int orderId)
        {
            return await Delete($"/api/orders/{orderId}");
        }

        public async Task<bool> AddDetail(OrderDetailCreateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);

            var requestContent = new MultipartFormDataContent
            {
                { new StringContent(request.OrderId.ToString()), "orderId" },
                { new StringContent(request.ProductId.ToString()), "productId" },
                { new StringContent(request.Quantity.ToString()), "quantity" },
                { new StringContent(request.Price.ToString()), "price" },
            };

            var response = await client.PostAsync($"/api/orders/details", requestContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<PagedResponse<OrderDetailDto>> GetDetails(GetOrderDetailsPagingRequest request)
        {
            return await GetAsync<PagedResponse<OrderDetailDto>>(
                $"/api/orders/details" +
                $"?orderId={request.OrderId}" +
                $"&pageIndex={request.PageIndex}" +
                $"&pageSize={request.PageSize}");
        }
    }
}