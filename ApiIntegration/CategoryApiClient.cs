using Common.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using ViewModels.Catalog.Categories;
using ViewModels.Common;

namespace ApiIntegration
{
    public interface ICategoryApiClient
    {
        Task<bool> Create(CategoryCreateRequest request);

        Task<List<CategoryDto>> GetAll(string languageId);

        Task<PagedResponse<CategoryDto>> GetAllPaged(GetCategoryPagingRequest request);

        Task<ApiResponse<CategoryDto>> GetById(int categoryId, string languageId);

        Task<bool> Update(CategoryUpdateRequest request);

        Task<bool> Delete(int categoryId);
    }

    public class CategoryApiClient : BaseApiClient, ICategoryApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public CategoryApiClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<bool> Create(CategoryCreateRequest request)
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
                { new StringContent(string.IsNullOrEmpty(request.Name) ? "" : request.Name.ToString()), "name" },
                { new StringContent(string.IsNullOrEmpty(request.SeoTitle) ? "" : request.SeoTitle.ToString()), "seoTitle" },
                { new StringContent(string.IsNullOrEmpty(request.SeoDescription) ? "" : request.SeoDescription.ToString()), "seoDescription" },
                { new StringContent(request.LanguageId), "languageId" },
                { new StringContent(request.SortOrder.ToString()), "sortOrder" },
                { new StringContent(request.IsShowOnHome.ToString()), "isShowOnHome" },
            };

            var response = await client.PostAsync($"/api/categories", requestContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<CategoryDto>> GetAll(string languageId)
        {
            return await GetListAsync<CategoryDto>($"/api/categories?languageId={languageId}");
        }

        public async Task<PagedResponse<CategoryDto>> GetAllPaged(GetCategoryPagingRequest request)
        {
            var data = await GetAsync<PagedResponse<CategoryDto>>(
                $"/api/categories/paged" +
                $"?languageId={request.LanguageId}" +
                $"&keyword={request.Keyword}" +
                $"&pageIndex={request.PageIndex}" +
                $"&pageSize={request.PageSize}");

            return data;
        }

        public async Task<ApiResponse<CategoryDto>> GetById(int categoryId, string languageId)
        {
            return await GetAsync<ApiResponse<CategoryDto>>($"/api/categories/{categoryId}/{languageId}");
        }

        public async Task<bool> Update(CategoryUpdateRequest request)
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
                { new StringContent(string.IsNullOrEmpty(request.Name) ? "" : request.Name.ToString()), "name" },
                { new StringContent(string.IsNullOrEmpty(request.SeoTitle) ? "" : request.SeoTitle.ToString()), "seoTitle" },
                { new StringContent(string.IsNullOrEmpty(request.SeoDescription) ? "" : request.SeoDescription.ToString()), "seoDescription" },
                { new StringContent(request.LanguageId), "languageId" },
                { new StringContent(request.SortOrder.ToString()), "sortOrder" },
                { new StringContent(request.IsShowOnHome.ToString()), "isShowOnHome" },
                { new StringContent(request.Status.ToString()), "status" },
            };

            var response = await client.PutAsync($"/api/categories/{request.Id}", requestContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Delete(int categoryId)
        {
            return await Delete($"/api/categories/{categoryId}");
        }
    }
}