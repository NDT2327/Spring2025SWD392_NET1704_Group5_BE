using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Models.Profiles;
using CCSystem.Presentation.Models.Bookings;
using CCSystem.Presentation.Models.BookingDetails;
using System.Security.Claims;
using CCSystem.Presentation.Helpers;
using Azure;
using CCSystem.Infrastructure.DTOs.Accounts;
using System.Text.Json;

namespace CCSystem.Presentation.Pages.Profiles
{
    public class AccountProfileModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;
        public AccountProfileModel(HttpClient httpClient, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClient;
            _apiEndpoints = apiEndpoints;
        }

        public CustomerProfile CustomerProfile { get; set; }
        public List<Booking> BookingHistory { get; set; } = new List<Booking>();
        public List<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

        public async Task OnGetAsync()
        {
            //get accountid from cookie
            var accountIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(!int.TryParse(accountIdString, out int accountId))
            {
                //if cannot get account id -> return to login page
                RedirectToPage("/Authentications/Login");
                return;
            }

            //call api get account
            var accountResponse = await _httpClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Account.GetAccountDetailsUrl(accountId)));
            if (accountResponse.IsSuccessStatusCode) {
                var json = await accountResponse.Content.ReadAsStringAsync();
                CustomerProfile = JsonSerializer.Deserialize<CustomerProfile>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new CustomerProfile();
            }
            else
            {
                ToastHelper.ShowError(TempData, "Cannot load account data");
            }
            //call api get booking by account id
            var bookingResponse = await _httpClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Booking.GetBookingByCustomer(accountId)));
            if (bookingResponse.IsSuccessStatusCode) {
                BookingHistory = await bookingResponse.Content.ReadFromJsonAsync<List<Booking>>() ?? new List<Booking>();
            }
            else
            {
                ToastHelper.ShowError(TempData, "Cannot load booking history");
                BookingHistory = new List<Booking>();
            }

        }

        public async Task<IActionResult> OnGetBookingDetailsAsync(int bookingId)
        {
            // Gọi API lấy chi tiết booking theo bookingId
            var detailResponse = await _httpClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.BookingDetail.GetBookingDetailByBooking(bookingId)));
            if (detailResponse.IsSuccessStatusCode)
            {
                var details = await detailResponse.Content.ReadFromJsonAsync<List<BookingDetail>>() ?? new List<BookingDetail>();
                BookingDetails = details; // Gán để debug nếu cần
                return new JsonResult(details);
            }
            else
            {
                ToastHelper.ShowError(TempData, "Cannot load booking details");
                return new JsonResult(new List<BookingDetail>());
            }
        }
    }
}
