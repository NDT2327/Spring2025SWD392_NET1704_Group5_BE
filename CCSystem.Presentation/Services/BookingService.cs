using CCSystem.Presentation.Configurations;

namespace CCSystem.Presentation.Services
{
    public class BookingService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        public BookingService(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("BookingAPI");
            _apiEndpoints = apiEndpoints;
        }
    }
}
