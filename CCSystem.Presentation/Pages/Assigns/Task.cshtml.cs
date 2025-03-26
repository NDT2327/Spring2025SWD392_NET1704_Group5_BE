using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CCSystem.DAL.DBContext;
using CCSystem.DAL.Models;
using CCSystem.Presentation.Configurations;
using CCSystem.Infrastructure.DTOs.BookingDetails;
using CCSystem.Infrastructure.DTOs.Accounts;
using CCSystem.Presentation.Helpers;
using CCSystem.Infrastructure.DTOs.ScheduleAssign;
using CCSystem.Infrastructure.DTOs.Bookings;
using System.Security.Claims;

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

        public IList<BookingDetailResponse> BookingDetail { get;set; } = default!;
        public PostScheduleAssignRequest PostScheduleAssignRequest { get; set; } = default!;

        public async Task OnGetAsync()
        {
            var response = await _bookingDetailClient.GetFromJsonAsync<List<BookingDetailResponse>>(_apiEndpoints.GetFullUrl(_apiEndpoints.BookingDetail.GetActiveBookingDetails));
            if (response == null)
            {
                ToastHelper.ShowError(TempData, "Not Found!");
                return;
            }
            BookingDetail = response;
        }

        public async Task<IActionResult> OnPostAssignAsync(int detailId)
        {
            string userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                ToastHelper.ShowError(TempData, "User ID không hợp lệ!");
                return Page();
            }

            var assignRequest = new PostScheduleAssignRequest
            {
                HousekeeperId = userId,
                DetailId = detailId,
                Notes = "Assigned automatically"
            };

            var response = await _assignScheduleClient.PostAsJsonAsync(
                _apiEndpoints.GetFullUrl(_apiEndpoints.Assign.CreateScheduleAssign), assignRequest);

            if (response.IsSuccessStatusCode)
            {
                ToastHelper.ShowSuccess(TempData, "Assigned successfully!");
            }
            else
            {
                ToastHelper.ShowError(TempData, "Assignment failed!");
            }

            return RedirectToPage();
        }

    }
}
