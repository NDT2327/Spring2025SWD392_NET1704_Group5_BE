using CCSystem.Presentation.Configurations;
using CCSystem.Infrastructure.DTOs.Services;

namespace CCSystem.Presentation.Services
{
    public class ServiceService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        public ServiceService(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("AccountAPI");
            _apiEndpoints = apiEndpoints;
        }

        //Get all HomeService
        public async Task<List<ServiceResponse>?> GetServicesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<ServiceResponse>>(_apiEndpoints.GetFullUrl(_apiEndpoints.Service.GetServices));
            return response ?? new List<ServiceResponse>();
        }

    }
}
