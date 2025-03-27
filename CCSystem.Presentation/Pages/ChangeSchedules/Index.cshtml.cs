
using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Models.BookingDetails;
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

        public IList<BookingDetail> BookingDetails { get; set; } = default!;


        public async Task OnGetAsync()
        {
            try
            {
                var bookingDetails = await _httpClient.GetFromJsonAsync<List<BookingDetail>>(_apiEndpoints.GetFullUrl(_apiEndpoints.BookingDetail.GetChangeScheduleEndpoint));

                if (bookingDetails == null)
                {
                    bookingDetails = new List<BookingDetail>();
                }
                BookingDetails = bookingDetails;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                BookingDetails = new List<BookingDetail>();
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
