using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Models.Bookings;

namespace CCSystem.Presentation.Pages.Bookings
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        public IndexModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("BookingAPI");
            _apiEndpoints = apiEndpoints;
        }

        public IList<Booking> Booking { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Booking = await _httpClient.GetFromJsonAsync<List<Booking>>(_apiEndpoints.GetFullUrl(_apiEndpoints.Booking.GetAllBookings)) ?? new List<Booking>();
        }
    }
}
