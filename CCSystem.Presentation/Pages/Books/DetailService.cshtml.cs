
using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Helpers;
using CCSystem.Presentation.Models.ServiceDetails;
using CCSystem.Presentation.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace CCSystem.Presentation.Pages.Books
{
    [Authorize(Roles = "CUSTOMER")]
    public class DetailServiceModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        public DetailServiceModel(HttpClient httpClient, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClient;
            _apiEndpoints = apiEndpoints;
        }

        [BindProperty(SupportsGet = true)]
        public int ServiceId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedServicesJson { get; set; }

        public List<GetServiceDetailResponse> ServiceDetails { get; set; } = new List<GetServiceDetailResponse>();
        public string ServiceName { get; set; }

        public async Task OnGetAsync()
        {
            Console.WriteLine($"ServiceId: {ServiceId}");
            var url = _apiEndpoints.GetFullUrl(_apiEndpoints.ServiceDetail.GetServiceDetailByService(ServiceId));
            Console.WriteLine($"Fetching ServiceDetail URL: {url}");

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    ToastHelper.ShowError(TempData, $"Cannot fetch service details. Status: {response.StatusCode}");
                    return; // Thoát nếu không lấy được dữ liệu
                }
                var details = await response.Content.ReadFromJsonAsync<List<GetServiceDetailResponse>>();
                if (details != null)
                {
                    ServiceDetails = details;
                }
                else
                {
                    ToastHelper.ShowError(TempData, "Service details data is empty");
                    return; // Thoát nếu dữ liệu rỗng
                }
            }
            catch (Exception ex)
            {
                ToastHelper.ShowError(TempData, $"Error fetching service details: {ex.Message}");
                return;
            }

            var service = await GetServiceAsync(ServiceId); // Đổi tên thành GetServiceAsync
            if (service != null)
            {
                ServiceName = service.ServiceName;
            }
            else
            {
                ServiceName = $"Service {ServiceId}";
            }
        }

        public IActionResult OnPostBook(int serviceDetailId)
        {
            var selectedIds = string.IsNullOrEmpty(SelectedServicesJson)
                        ? new List<int>()
                        : JsonSerializer.Deserialize<List<int>>(SelectedServicesJson);

            // Kiểm tra trùng lặp
            if (selectedIds.Contains(serviceDetailId))
            {
                ToastHelper.ShowWarning(TempData, $"Service with ID {serviceDetailId} is already selected.");
            }
            else
            {
                selectedIds.Add(serviceDetailId);
                SelectedServicesJson = JsonSerializer.Serialize(selectedIds);
            }

            return RedirectToPage("/Books/Booking", new { serviceDetailId, selectedServicesJson = SelectedServicesJson });
        }

        private async Task<ServiceResponse> GetServiceAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Service.GetServiceById(id)));
                if (!response.IsSuccessStatusCode)
                {
                    ToastHelper.ShowError(TempData, $"Cannot fetch service. Status: {response.StatusCode}");
                    return null; // Trả về null thay vì new ServiceResponse()
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
