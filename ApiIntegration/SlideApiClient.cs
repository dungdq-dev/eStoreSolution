using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ViewModels.Utilities.Slides;

namespace ApiIntegration
{
    public class SlideApiClient : BaseApiClient, ISlideApiClient
    {
        public SlideApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
        }

        public async Task<List<SlideDto>> GetAll()
        {
            return await GetListAsync<SlideDto>("/api/slides");
        }
    }
}