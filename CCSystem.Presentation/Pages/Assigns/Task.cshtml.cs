using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Helpers;
using System.Security.Claims;
using System.Text.Json;
using CCSystem.Presentation.Models.BookingDetails;
using CCSystem.Presentation.Models.ScheduleAssign;

namespace CCSystem.Presentation.Pages.Assigns
{
    public class TaskModel : PageModel
    {
        private readonly HttpClient _bookingDetailClient;
        private readonly HttpClient _assignScheduleClient;
        private readonly ApiEndpoints _apiEndpoints;

        public TaskModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _assignScheduleClient = httpClientFactory.CreateClient("AssignAPI");
            _bookingDetailClient = httpClientFactory.CreateClient("BookingDetailAPI");
            _apiEndpoints = apiEndpoints;
        }

        public IList<BookingDetail> BookingDetail { get;set; } = default!;
        public PostScheduleAssignRequest PostScheduleAssignRequest { get; set; } = default!;

        public async Task OnGetAsync()
        {
            var response = await _bookingDetailClient.GetFromJsonAsync<List<BookingDetail>>(_apiEndpoints.GetFullUrl(_apiEndpoints.BookingDetail.GetActiveBookingDetails));
            if (response == null)
            {
                ToastHelper.ShowError(TempData, "Not Found!");
                return;
            }
            BookingDetail = response;
        }

        public async Task<IActionResult> OnPostAssignAsync(int detailId, string? notes)
        {
            Console.WriteLine($"Detail ID: {detailId}, Notes: {notes ?? "NULL"}");

            if (string.IsNullOrWhiteSpace(notes))
            {
                notes = "Automatic Assign";
            }

            string userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                ToastHelper.ShowError(TempData, "User ID is not valid!");
                return Page();
            }

            var assignRequest = new PostScheduleAssignRequest
            {
                HousekeeperId = userId,
                DetailId = detailId,
                Notes = notes
            };

            Console.WriteLine($"Sending Assign Request: {assignRequest.Notes}");

            var response = await _assignScheduleClient.PostAsJsonAsync(
                _apiEndpoints.GetFullUrl(_apiEndpoints.Assign.CreateScheduleAssign), assignRequest);

            if (response.IsSuccessStatusCode)
            {
                ToastHelper.ShowSuccess(TempData, "Assigned successfully!");
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Error: {errorResponse}");

                try
                {
                    var errorData = JsonSerializer.Deserialize<ApiErrorResponse>(errorResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (errorData?.Message != null && errorData.Message.Any())
                    {
                        var errorMessage = string.Join("; ", errorData.Message.Select(m => $"{m.FieldNameError}: {string.Join(", ", m.DescriptionError)}"));
                        ToastHelper.ShowError(TempData, errorMessage);
                    }
                    else
                    {
                        ToastHelper.ShowError(TempData, "Assignment failed due to an unknown error!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"JSON Parsing Error: {ex.Message}");
                    ToastHelper.ShowError(TempData, "Assignment failed! Could not parse error message.");
                }
            }

            return RedirectToPage();
        }
    }
    public class ApiErrorResponse
{
    public int StatusCode { get; set; }
    public List<ErrorDetail> Message { get; set; } = new();
}

public class ErrorDetail
{
    public string FieldNameError { get; set; }
    public List<string> DescriptionError { get; set; } = new();
}
}
