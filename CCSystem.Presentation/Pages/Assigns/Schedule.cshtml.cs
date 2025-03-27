using Microsoft.AspNetCore.Mvc.RazorPages;
using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using CCSystem.Presentation.Models.ScheduleAssign;

namespace CCSystem.Presentation.Pages.Assigns
{
    public class ScheduleModel : PageModel
    {
        private readonly HttpClient _assignScheduleClient;
        private readonly ApiEndpoints _apiEndpoints;

        public ScheduleModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _assignScheduleClient = httpClientFactory.CreateClient("AssignAPI");
            _apiEndpoints = apiEndpoints;
        }

        public IList<ScheduleAssignmentResponse> ScheduleAssignment { get; set; } = new List<ScheduleAssignmentResponse>(); // Initialize to avoid null
        public CompleteAssignmentRequest CompleteAssignmentRequest { get; set; } = default!;
        public PatchAssignStatusRequest PatchAssignStatusRequest { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                ToastHelper.ShowError(TempData, "User ID is not valid!");
                return Page(); // Render the page with the error
            }

            var response = await _assignScheduleClient.GetFromJsonAsync<List<ScheduleAssignmentResponse>>(_apiEndpoints.GetFullUrl(_apiEndpoints.Assign.GetAssingByHousekeeper(userId)));
            if (response == null)
            {
                ToastHelper.ShowError(TempData, "Not Found!");
                return Page(); // Render the page with the error
            }

            ScheduleAssignment = response.Where(assign => assign.Status != "COMPLETED").ToList();
            return Page(); // Render the page with the data
        }

        public async Task<IActionResult> OnPostAsync(int assignId)
        {
            var apiResponse = await _assignScheduleClient.GetFromJsonAsync<ScheduleAssignmentResponse>(_apiEndpoints.GetFullUrl(_apiEndpoints.Assign.GetAssign(assignId)));
            if (apiResponse == null)
            {
                ToastHelper.ShowError(TempData, "Not Found Assign");
                return RedirectToPage(); // Redirect to OnGetAsync to refresh data
            }

            if (apiResponse.Status == "ASSIGNED")
            {
                PatchAssignStatusRequest = new PatchAssignStatusRequest
                {
                    AssignmentId = assignId,
                    Status = "INPROGRESS"
                };
                try
                {
                    var url = _apiEndpoints.GetFullUrl(_apiEndpoints.Assign.UpdateScheduleAssign);
                    var result = await _assignScheduleClient.PatchAsJsonAsync(url, PatchAssignStatusRequest);

                    if (!result.IsSuccessStatusCode)
                    {
                        var errorContent = await result.Content.ReadAsStringAsync();
                        ToastHelper.ShowError(TempData, $"Cannot Change Status: {result.StatusCode} - {errorContent}");
                        return RedirectToPage(); // Redirect to OnGetAsync to refresh data
                    }

                    ToastHelper.ShowSuccess(TempData, "Update Status Successfully!");
                }
                catch (Exception ex)
                {
                    ToastHelper.ShowError(TempData, $"Error: {ex.Message}");
                    return RedirectToPage(); // Redirect to OnGetAsync to refresh data
                }
            }
            else if (apiResponse.Status == "INPROGRESS")
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out int userId))
                {
                    ToastHelper.ShowError(TempData, "User ID is not valid!");
                    return RedirectToPage(); // Redirect to OnGetAsync to refresh data
                }

                CompleteAssignmentRequest = new CompleteAssignmentRequest
                {
                    AssignmentId = assignId,
                    HousekeeperId = userId,
                };
                try
                {
                    var result = await _assignScheduleClient.PostAsJsonAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Assign.CompleteScheduleAssign), CompleteAssignmentRequest);

                    if (!result.IsSuccessStatusCode)
                    {
                        var errorContent = await result.Content.ReadAsStringAsync();
                        ToastHelper.ShowError(TempData, $"Cannot Change Status: {result.StatusCode} - {errorContent}");
                        return RedirectToPage(); // Redirect to OnGetAsync to refresh data
                    }

                    ToastHelper.ShowSuccess(TempData, "Update Status Successfully!");
                }
                catch (Exception ex)
                {
                    ToastHelper.ShowError(TempData, $"Error: {ex.Message}");
                    return RedirectToPage(); // Redirect to OnGetAsync to refresh data
                }
            }

            return RedirectToPage(); // Redirect to OnGetAsync to refresh data after success
        }
    }
}