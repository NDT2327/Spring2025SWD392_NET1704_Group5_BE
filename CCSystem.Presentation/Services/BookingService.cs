using CCSystem.Infrastructure.DTOs.BookingDetails;
using CCSystem.Infrastructure.DTOs.Bookings;
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

        public async Task<List<BookingResponse>> GetAllBookingDetails()
        {
            var response = await _httpClient.GetFromJsonAsync<List<BookingResponse>>(_apiEndpoints.GetFullUrl(_apiEndpoints.Booking.GetAllBookings));
            return response ?? new List<BookingResponse>();
        }

        public async Task<List<BookingResponse>> GetBookingsByUser(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<List<BookingResponse>>(_apiEndpoints.GetFullUrl(_apiEndpoints.Booking.GetBookingByCustomer(id)));
            return response ?? new List<BookingResponse>();
        }

        public async Task<BookingResponse> CreateBooking(PostBookingRequest bookingRequest)
        {
            var response = await _httpClient.PostAsJsonAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Booking.CreateBooking), bookingRequest);
            if (response.IsSuccessStatusCode)
            {
                var bookingResponse = await response.Content.ReadFromJsonAsync<BookingResponse>();
                return bookingResponse;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"CreateBooking error 400: {errorContent}");
                return null;
            }
        }
    }
}
