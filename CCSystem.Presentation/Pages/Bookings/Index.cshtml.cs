using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CCSystem.DAL.DBContext;
using CCSystem.DAL.Models;
using CCSystem.Infrastructure.DTOs.Accounts;
using CCSystem.Presentation.Configurations;
using CCSystem.Infrastructure.DTOs.Bookings;

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

        public IList<BookingResponse> Booking { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Booking = await _httpClient.GetFromJsonAsync<List<BookingResponse>>(_apiEndpoints.GetFullUrl(_apiEndpoints.Booking.GetAllBookings)) ?? new List<BookingResponse>();
        }
    }
}
