using CCSystem.Infrastructure.DTOs.BookingDetails;
using CCSystem.Presentation.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CCSystem.Presentation.Pages.ChangeSchedules
{
    public class RescheduleModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        public RescheduleModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("BookingDetailAPI");
            _apiEndpoints = apiEndpoints;

        }
        [BindProperty]
        public DateOnly NewDate { get; set; }

        [BindProperty]
        public TimeOnly NewTime { get; set; }

        public BookingDetailResponse BookingDetail { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            //var detail = await _httpClient.GetFromJsonAsync<BookingDetailResponse>($"/api/v1/bookingDetails/{id}");
            var detail = await _httpClient.GetFromJsonAsync<BookingDetailResponse>(_apiEndpoints.GetFullUrl(_apiEndpoints.BookingDetail.GetBookingDetail(id)));


            if (detail == null)
            {
                return NotFound();
            }

            BookingDetail = detail;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var request = new RescheduleRequest
            {
                NewDate = NewDate,
                NewTime = NewTime
            };

            //var response = await _httpClient.PutAsJsonAsync($"/api/v1/bookingDetails/reschedule/{id}", request);
            var response = await _httpClient.PutAsJsonAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.BookingDetail.Reschedule(id)), request);


            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error rescheduling booking.");
                return Page();
            }

            return RedirectToPage("/BookingDetails/Index");
        }
    }
}

