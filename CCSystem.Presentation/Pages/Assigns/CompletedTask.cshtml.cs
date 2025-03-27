using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Helpers;
using System.Security.Claims;
using CCSystem.Presentation.Models.ScheduleAssign;

namespace CCSystem.Presentation.Pages.Assigns
{
    public class CompletedTaskModel : PageModel
    {
        private readonly HttpClient _assignScheduleClient;
        private readonly ApiEndpoints _apiEndpoints;

        public CompletedTaskModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _assignScheduleClient = httpClientFactory.CreateClient("AssignAPI");
            _apiEndpoints = apiEndpoints;
        }

        public IList<ScheduleAssignmentResponse> ScheduleAssignment { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                ToastHelper.ShowError(TempData, "User ID is not valid!");
                return; // Render the page with the error
            }

            var response = await _assignScheduleClient.GetFromJsonAsync<List<ScheduleAssignmentResponse>>(_apiEndpoints.GetFullUrl(_apiEndpoints.Assign.GetAssingByHousekeeper(userId)));
            if (response == null)
            {
                ToastHelper.ShowError(TempData, "Not Found!");
                return; // Render the page with the error
            }

            ScheduleAssignment = response.Where(assign => assign.Status == "COMPLETED").ToList();
            return; // Render the page with the data
        }
    }
}
