using CCSystem.Infrastructure.DTOs.BookingDetails;
using CCSystem.Infrastructure.DTOs.Bookings;
using CCSystem.Infrastructure.DTOs.Payments;
using CCSystem.Infrastructure.DTOs.ServiceDetails;
using CCSystem.Infrastructure.DTOs.Services;
using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Constants;
using CCSystem.Presentation.Helpers;
using CCSystem.Presentation.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace CCSystem.Presentation.Pages
{
    public class BookingModel : PageModel
    {
        //private readonly BookingService _bookingService;
        //private readonly ServiceService _serviceService;
        //private readonly ServiceDetailService _serviceDetailService;

        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        public BookingModel(HttpClient httpClient, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClient;
            _apiEndpoints = apiEndpoints;
        }

        [BindProperty]
        public PostBookingRequest BookingRequest { get; set; } = new PostBookingRequest();

        [BindProperty]
        public PostBookingDetailRequest NewBookingDetail { get; set; } = new PostBookingDetailRequest();

        [BindProperty(SupportsGet = true)]
        public string SelectedServicesJson { get; set; }

        public List<GetServiceDetailResponse> SelectedServices { get; set; } = new List<GetServiceDetailResponse>();
        public decimal TotalAmount { get; set; }
        public int? LastSelectedServiceId { get; set; }

        public async Task OnGetAsync(int? serviceDetailId)
        {
            BookingRequest.BookingDetails = new List<PostBookingDetailRequest>();
            SelectedServices.Clear();

            if (!string.IsNullOrEmpty(SelectedServicesJson))
            {
                try
                {
                    var selectedIds = JsonSerializer.Deserialize<List<int>>(SelectedServicesJson);
                    Console.WriteLine($"Deserialized SelectedServicesJson: {JsonSerializer.Serialize(selectedIds)}");
                    foreach (var id in selectedIds)
                    {
                        var detail = await GetServiceDetailAsync(id);
                        if (detail != null && !SelectedServices.Any(s => s.ServiceDetailId == id))
                        {
                            SelectedServices.Add(detail);
                            BookingRequest.BookingDetails.Add(new PostBookingDetailRequest
                            {
                                ServiceId = detail.ServiceId ?? 0, // Xử lý null
                                ServiceDetailId = detail.ServiceDetailId,
                                Quantity = 1
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    ToastHelper.ShowError(TempData, $"Error deserializing selected services: {ex.Message}");
                }
            }

            if (serviceDetailId.HasValue)
            {
                var serviceDetail = await GetServiceDetailAsync(serviceDetailId.Value);
                Console.WriteLine($"ServiceDetail for ID {serviceDetailId}: {JsonSerializer.Serialize(serviceDetail)}");
                if (serviceDetail != null && !SelectedServices.Any(s => s.ServiceDetailId == serviceDetailId.Value))
                {
                    SelectedServices.Add(serviceDetail);
                    BookingRequest.BookingDetails.Add(new PostBookingDetailRequest
                    {
                        ServiceId = serviceDetail.ServiceId ?? 0, // Xử lý null
                        ServiceDetailId = serviceDetail.ServiceDetailId,
                        Quantity = 1
                    });
                    LastSelectedServiceId = serviceDetail.ServiceDetailId;
                }
                else if (serviceDetail == null)
                {
                    ToastHelper.ShowError(TempData, $"Service with ID {serviceDetailId} not found.");
                }
            }

            var updatedSelectedIds = SelectedServices.Select(s => s.ServiceDetailId).ToList();
            SelectedServicesJson = JsonSerializer.Serialize(updatedSelectedIds);
            Console.WriteLine($"Updated SelectedServicesJson: {SelectedServicesJson}");

            TotalAmount = SelectedServices
                .Where(s => s.BasePrice.HasValue)
                .Sum(s => s.BasePrice.Value);
            Console.WriteLine($"SelectedServices Count: {SelectedServices.Count}, TotalAmount: {TotalAmount}");
        }


        public IActionResult OnPostAddDetail()
        {
            return RedirectToPage("/Books/Filter", new { selectedServicesJson = SelectedServicesJson });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync(null);
                return Page();
            }

            SelectedServices.Clear();
            if (!string.IsNullOrEmpty(SelectedServicesJson))
            {
                try
                {
                    var selectedIds = JsonSerializer.Deserialize<List<int>>(SelectedServicesJson);
                    foreach (var id in selectedIds)
                    {
                        var detail = await GetServiceDetailAsync(id);
                        if (detail != null)
                        {
                            SelectedServices.Add(detail);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ToastHelper.ShowError(TempData, $"Error reloading selected services: {ex.Message}");
                    await OnGetAsync(null);
                    return Page();
                }
            }

            if (BookingRequest.BookingDetails.Count != SelectedServices.Count)
            {
                Console.WriteLine($"Mismatch detected: BookingDetails.Count={BookingRequest.BookingDetails.Count}, SelectedServices.Count={SelectedServices.Count}");
                ToastHelper.ShowError(TempData, "Danh sách dịch vụ không đồng bộ. Vui lòng thử lại.");
                await OnGetAsync(null);
                return Page();
            }

            TotalAmount = 0;
            for (int i = 0; i < BookingRequest.BookingDetails.Count; i++)
            {
                var detail = SelectedServices[i];
                BookingRequest.BookingDetails[i].ServiceId = detail.ServiceId ?? 0;
                BookingRequest.BookingDetails[i].ServiceDetailId = detail.ServiceDetailId;
                TotalAmount += detail.BasePrice.GetValueOrDefault() * BookingRequest.BookingDetails[i].Quantity;
            }

            string userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"UserId: {userIdString}");
            if (int.TryParse(userIdString, out int userId))
            {
                BookingRequest.CustomerId = userId;
                Console.WriteLine($"CustomerId set to: {BookingRequest.CustomerId}");
            }

            var jsonContent = new StringContent(JsonSerializer.Serialize(BookingRequest), System.Text.Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Booking.CreateBooking), jsonContent);

            if (!result.IsSuccessStatusCode)
            {
                var errorContent = await result.Content.ReadAsStringAsync();
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(errorContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var errorMessage = errorResponse?.GetError() ?? "Tạo booking thất bại.";
                Console.WriteLine($"CreateBooking API Error: Status {result.StatusCode}, Response: {errorContent}");
                ToastHelper.ShowError(TempData, errorMessage);
                TotalAmount = SelectedServices.Sum(s => s.BasePrice.GetValueOrDefault());
                return Page();
            }

            var bookingResponseContent = await result.Content.ReadAsStringAsync();
            Console.WriteLine($"CreateBooking Response: {bookingResponseContent}");

            // Deserialize với ApiResponse<int> vì "data" là số nguyên
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<int>>(bookingResponseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (apiResponse == null || apiResponse.Data <= 0)
            {
                Console.WriteLine($"Invalid BookingId: {apiResponse?.Data ?? 0}");
                ToastHelper.ShowError(TempData, "BookingId không hợp lệ.");
                return Page();
            }

            Console.WriteLine($"Deserialized BookingId: {apiResponse.Data}");
            ToastHelper.ShowSuccess(TempData, apiResponse.SuccessMessage ?? Message.Bookig.CreatedSuccessfully);

            var paymentRequest = new CreatePaymentRequest
            {
                BookingId = apiResponse.Data, // Lấy BookingId từ Data
                Description = $"Payment for {apiResponse.Data}"
            };
            var paymentJson = new StringContent(JsonSerializer.Serialize(paymentRequest), System.Text.Encoding.UTF8, "application/json");
            Console.WriteLine($"Sending Payment Request: {JsonSerializer.Serialize(paymentRequest)}");
            var paymentResult = await _httpClient.PostAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Payment.CreatePaymentUrl), paymentJson);

            if (!paymentResult.IsSuccessStatusCode)
            {
                var errorContent = await paymentResult.Content.ReadAsStringAsync();
                var paymentErrorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(errorContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var errorMessage = paymentErrorResponse?.GetError() ?? "Không thể tạo URL thanh toán VNPay.";
                Console.WriteLine($"Payment API Error: Status {paymentResult.StatusCode}, Response: {errorContent}");
                ToastHelper.ShowError(TempData, errorMessage);
                return Page();
            }

            var paymentUrl = await paymentResult.Content.ReadAsStringAsync();
            Console.WriteLine($"Payment URL: {paymentUrl}");
            SelectedServices.Clear();
            SelectedServicesJson = JsonSerializer.Serialize(new List<int>());

            return Redirect(paymentUrl);
        }


        private async Task<GetServiceDetailResponse> GetServiceDetailAsync(int serviceDetailId)
        {
            try
            {
                var url = _apiEndpoints.GetFullUrl(_apiEndpoints.ServiceDetail.GetServiceDetail(serviceDetailId));
                Console.WriteLine($"Calling API: {url}");
                var response = await _httpClient.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Raw API Response: {json}");
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize trực tiếp thành GetServiceDetailResponse vì API không bọc trong ApiResponse
                    var serviceDetail = JsonSerializer.Deserialize<GetServiceDetailResponse>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Bỏ qua phân biệt hoa thường
                    });
                    if (serviceDetail != null)
                    {
                        return serviceDetail;
                    }
                    ToastHelper.ShowError(TempData, "Service detail data is null or invalid.");
                    return null;
                }
                else
                {
                    ToastHelper.ShowError(TempData, $"API returned status: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                ToastHelper.ShowError(TempData, $"Error fetching service detail: {ex.Message}");
                return null;
            }
        }

        private async Task<ServiceResponse> GetServiceAsync(int serviceId)
        {
            try
            {
                var url = _apiEndpoints.GetFullUrl(_apiEndpoints.Service.GetServiceById(serviceId));
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    ToastHelper.ShowError(TempData, $"Cannot fetch service. Status: {response.StatusCode}");
                    return null;
                }
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ServiceResponse>(json);
            }
            catch (Exception ex)
            {
                ToastHelper.ShowError(TempData, $"Error fetching service: {ex.Message}");
                return null;
            }
        }


    }
}