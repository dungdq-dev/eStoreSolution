using Common.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using ViewModels.Catalog.Products;
using ViewModels.Common;

namespace ApiIntegration
{
    public interface IProductApiClient
    {
        Task<bool> Create(ProductCreateRequest request);

        Task<PagedResponse<ProductDto>> GetAll(GetProductPagingRequest request);

        Task<ApiResponse<ProductDto>> GetById(int productId, string languageId);

        Task<List<ProductDto>> GetFeatured(string languageId, int take);

        Task<List<ProductDto>> GetLatest(string languageId, int take);

        Task<bool> Update(ProductUpdateRequest request);

        Task<bool> UpdateStock(ProductStockUpdateRequest request);

        Task<ApiResponse<bool>> CategoryAssign(int productId, CategoryAssignRequest request);

        Task<bool> Delete(int productId);
    }

    public class ProductApiClient : BaseApiClient, IProductApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public ProductApiClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<bool> Create(ProductCreateRequest request)
        {
            var sessions = _httpContextAccessor
                .HttpContext
                .Session
                .GetString(SystemConstants.AppSettings.Token);

            var languageId = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId) ?? "vi";

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();

            if (request.ThumbnailImage != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.ThumbnailImage.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ThumbnailImage.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "thumbnailImage", request.ThumbnailImage.FileName);
            }

            requestContent.Add(new StringContent(request.Price.ToString()), "price");
            requestContent.Add(new StringContent(request.OriginalPrice.ToString()), "originalPrice");
            requestContent.Add(new StringContent(request.Stock.ToString()), "stock");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Name) ? "" : request.Name.ToString()), "name");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Description) ? "" : request.Description.ToString()), "description");

            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Details) ? "" : request.Details.ToString()), "details");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.SeoDescription) ? "" : request.SeoDescription.ToString()), "seoDescription");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.SeoTitle) ? "" : request.SeoTitle.ToString()), "seoTitle");
            requestContent.Add(new StringContent(languageId), "languageId");

            var response = await client.PostAsync($"/api/products", requestContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<PagedResponse<ProductDto>> GetAll(GetProductPagingRequest request)
        {
            var data = await GetAsync<PagedResponse<ProductDto>>(
                $"/api/products" +
                $"?languageId={request.LanguageId}" +
                $"&categoryId={request.CategoryId}" +
                $"&keyword={request.Keyword}" +
                $"&pageIndex={request.PageIndex}" +
                $"&pageSize={request.PageSize}");

            return data;
        }

        public async Task<ApiResponse<ProductDto>> GetById(int productId, string languageId)
        {
            var response = await GetAsync<ApiResponse<ProductDto>>(
                $"/api/products/{productId}/{languageId}");
            return response;
        }

        public async Task<List<ProductDto>> GetFeatured(string languageId, int take)
        {
            var data = await GetListAsync<ProductDto>($"/api/products/featured/{take}?languageId={languageId}");
            return data;
        }

        public async Task<List<ProductDto>> GetLatest(string languageId, int take)
        {
            var data = await GetListAsync<ProductDto>($"/api/products/latest/{take}?languageId={languageId}");
            return data;
        }

        public async Task<bool> Update(ProductUpdateRequest request)
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
                //{ new StringContent(request.Id.ToString()), "id" },
                { new StringContent(string.IsNullOrEmpty(request.Name) ? "" : request.Name.ToString()), "name" },
                { new StringContent(string.IsNullOrEmpty(request.Description) ? "" : request.Description.ToString()), "description" },
                { new StringContent(string.IsNullOrEmpty(request.Details) ? "" : request.Details.ToString()), "details" },
                { new StringContent(string.IsNullOrEmpty(request.SeoTitle) ? "" : request.SeoTitle.ToString()), "seoTitle" },
                { new StringContent(string.IsNullOrEmpty(request.SeoDescription) ? "" : request.SeoDescription.ToString()), "seoDescription" },
                { new StringContent(request.Price.ToString()), "price" },
                { new StringContent(request.IsFeatured.ToString() ?? false.ToString()), "isFeatured" },
                { new StringContent(request.LanguageId), "languageId" }
            };

            if (request.ThumbnailImage != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.ThumbnailImage.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ThumbnailImage.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "thumbnailImage", request.ThumbnailImage.FileName);
            }

            var response = await client.PutAsync($"/api/products/{request.Id}", requestContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateStock(ProductStockUpdateRequest request)
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
                { new StringContent(request.ProductId.ToString()), "id" },
                { new StringContent(request.Amount.ToString()), "amount" }
            };

            var response = await client.PatchAsync($"/api/products/stock/{request.ProductId}/{request.Amount}", requestContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<ApiResponse<bool>> CategoryAssign(int id, CategoryAssignRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/products/{id}/category-assign", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResponse<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResponse<bool>>(result);
        }

        public async Task<bool> Delete(int productId)
        {
            return await Delete($"/api/products/{productId}");
        }
    }
}