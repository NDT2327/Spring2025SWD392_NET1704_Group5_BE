using CCSystem.DAL.Models;
using CCSystem.Infrastructure.DTOs.BookingDetails;
using CCSystem.Infrastructure.DTOs.Promotions;
using CCSystem.Infrastructure.DTOs.Services;
using CCSystem.Presentation.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CCSystem.Presentation.Pages.ChangeSchedules
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        public IndexModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("BookingDetailAPI");
            _apiEndpoints = apiEndpoints;
        }

        public IList<BookingDetailResponse> BookingDetails { get; set; } = default!;


        public async Task OnGetAsync()
        {
            try
            {
                var bookingDetails = await _httpClient.GetFromJsonAsync<List<BookingDetailResponse>>(_apiEndpoints.GetFullUrl(_apiEndpoints.BookingDetail.GetChangeScheduleEndpoint));

                if (bookingDetails == null)
                {
                    bookingDetails = new List<BookingDetailResponse>();
                }
                BookingDetails = bookingDetails;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                BookingDetails = new List<BookingDetailResponse>();
            }
        }

		public async Task<IActionResult> OnPostConfirmScheduleAsync(int id, bool isAccepted)
		{
			try
			{
				var request = new ConfirmRescheduleRequest { IsAccepted = isAccepted };

				var response = await _httpClient.PutAsJsonAsync(
					$"{_apiEndpoints.GetFullUrl(_apiEndpoints.BookingDetail.ConfirmBookingDetail(id))}",
					request);

				if (!response.IsSuccessStatusCode)
				{
					ModelState.AddModelError(string.Empty, "An error occurred while confirming the schedule.");
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"Errors: {ex.Message}");
			}

			return RedirectToPage();
		}
	}
}
