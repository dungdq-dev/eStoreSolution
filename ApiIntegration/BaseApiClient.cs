using Common.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ApiIntegration
{
    public class BaseApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        protected BaseApiClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        protected async Task<TResponse> GetAsync<TResponse>(string url)
        {
            var sessions = _httpContextAccessor
                .HttpContext
                .Session
                .GetString(SystemConstants.AppSettings.Token);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                TResponse myDeserializedObjList = (TResponse)JsonConvert.DeserializeObject(body, typeof(TResponse));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                return myDeserializedObjList;
            }
#pragma warning disable CS8603 // Possible null reference return.
            return JsonConvert.DeserializeObject<TResponse>(body);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<List<T>> GetListAsync<T>(string url, bool requiredLogin = false)
        {
            var sessions = _httpContextAccessor
               .HttpContext
               .Session
               .GetString(SystemConstants.AppSettings.Token);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject(body, typeof(List<T>)) as List<T>;
                return data;
            }
            throw new ArgumentNullException(body);
        }

        public async Task<bool> Delete(string url)
        {
            var sessions = _httpContextAccessor
               .HttpContext
               .Session
               .GetString(SystemConstants.AppSettings.Token);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.DeleteAsync(url);
            if (response.IsSuccessStatusCode)
                return true;

            return false;
        }
    }
}