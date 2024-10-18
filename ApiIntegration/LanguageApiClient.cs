using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ViewModels.Common;
using ViewModels.System.Languages;

namespace ApiIntegration
{
    public class LanguageApiClient : BaseApiClient, ILanguageApiClient
    {
        public LanguageApiClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
        }

        public async Task<ApiResponse<List<LanguageDto>>> GetAll()
        {
            return await GetAsync<ApiResponse<List<LanguageDto>>>("/api/languages");
        }
    }
}