using CCSystem.Infrastructure.DTOs.BookingDetails;
using CCSystem.Infrastructure.DTOs.ServiceDetails;
using CCSystem.Presentation.Configurations;

namespace CCSystem.Presentation.Services
{
    public class ServiceDetailService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        public ServiceDetailService(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("ServiceDetailAPI");
            _apiEndpoints = apiEndpoints;
        }

        public async Task<List<GetServiceDetailResponse>> getAllBookingDetails()
        {
            var response = await _httpClient.GetFromJsonAsync<List<GetServiceDetailResponse>>(_apiEndpoints.GetFullUrl(_apiEndpoints.BookingDetail.GetAllBookingDetails));
            return response?.Where(x => x.IsActive == true).ToList() ?? new List<GetServiceDetailResponse>();
        }
    }
}
