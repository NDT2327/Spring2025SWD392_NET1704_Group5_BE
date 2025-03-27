using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Helpers;
using CCSystem.Presentation.Models.Bookings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace CCSystem.Presentation.Pages.Bookings
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;
        public DetailsModel(HttpClient httpClient, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClient;
            _apiEndpoints = apiEndpoints;
        }

        public Booking Booking { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var response = await _httpClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Booking.GetBooking(id.Value)));
            if (response.IsSuccessStatusCode)
            {
                Booking = await response.Content.ReadFromJsonAsync<Booking>();
                if (Booking == null)
                {
                    ToastHelper.ShowError(TempData, "Booking not found", 3000);
                    return RedirectToPage("/Bookings/Index");
                }
            }
            else
            {
                ToastHelper.ShowError(TempData, $"Cannot load booking details (Status: {response.StatusCode})", 3000);
                return RedirectToPage("/Bookings/Index");
            }

            return Page();
        }
    }
}
