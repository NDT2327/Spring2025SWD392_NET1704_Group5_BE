using CCSystem.Infrastructure.DTOs.BookingDetails;
using CCSystem.Infrastructure.DTOs.Services;
using CCSystem.Presentation.Configurations;
using System.Security.Principal;

namespace CCSystem.Presentation.Services
{
    public class BookingDetailService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        public BookingDetailService(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("BookingDetailAPI");
            _apiEndpoints = apiEndpoints;
        }

        public async Task<List<BookingDetailResponse>> getAllBookingDetails()
        {
            var response = await _httpClient.GetFromJsonAsync<List<BookingDetailResponse>>(_apiEndpoints.GetFullUrl(_apiEndpoints.BookingDetail.GetAllBookingDetails));
            return response ?? new List<BookingDetailResponse>();
        }
    }
}
