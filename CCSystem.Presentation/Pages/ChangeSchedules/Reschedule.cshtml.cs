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
        public string NewDate { get; set; } = string.Empty;  // Sử dụng string

        [BindProperty]
        public string NewTime { get; set; } = string.Empty;  // Sử dụng string

        public BookingDetailResponse BookingDetail { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var detail = await _httpClient.GetFromJsonAsync<BookingDetailResponse>(
                _apiEndpoints.GetFullUrl(_apiEndpoints.BookingDetail.GetBookingDetail(id))
            );

            if (detail == null)
            {
                return NotFound();
            }

            BookingDetail = detail;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Chuyển đổi sang định dạng chính xác
            if (!DateOnly.TryParseExact(NewDate, "yyyy-MM-dd", out var parsedDate) ||
                !TimeOnly.TryParseExact(NewTime, "HH:mm:ss", out var parsedTime))
            {
                ModelState.AddModelError(string.Empty, "Invalid date or time format.");
                return Page();
            }

            var request = new RescheduleRequest
            {
                NewDate = parsedDate,
                NewTime = parsedTime
            };

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

