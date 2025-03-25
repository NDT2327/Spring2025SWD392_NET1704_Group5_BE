using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CCSystem.DAL.DBContext;
using CCSystem.DAL.Models;
using CCSystem.Presentation.Services;
using CCSystem.Infrastructure.DTOs.Services;
using CCSystem.Infrastructure.DTOs.ServiceDetails;
using CCSystem.Presentation.Helpers;
using CCSystem.Presentation.Configurations;
using System.Net.Http;
using static CCSystem.API.Constants.APIEndPointConstant;
using System.Text.Json;
using System.Security.Policy;

namespace CCSystem.Presentation.Pages.Services
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _serviceApiClient;
        private readonly HttpClient _serviceDetailApiClient;
        private readonly ApiEndpoints _apiEndpoints;
        public DetailsModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _serviceApiClient = httpClientFactory.CreateClient("ServiceAPI");
            _serviceDetailApiClient = httpClientFactory.CreateClient("ServiceDetailAPI");
            _apiEndpoints = apiEndpoints;
        }

        public ServiceResponse Service { get; set; } = default!;
        public IList<GetServiceDetailResponse> Details { get; set; } = default!;
        [BindProperty]
        public PostServiceDetailRequest NewDetail { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _serviceApiClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Service.GetServiceById(id)));
                if (!response.IsSuccessStatusCode) return NotFound();
                var json = await response.Content.ReadAsStringAsync();
                var service = JsonSerializer.Deserialize<ServiceResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (service == null)
                {
                    return NotFound();
                }
                else
                {
                    Service = service;
                    var responseDetail = await _serviceDetailApiClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.ServiceDetail.GetServiceDetailByService(id)));
                    if (!responseDetail.IsSuccessStatusCode)
                    {
                        return NotFound();
                    }
                    var apiResponse = await response.Content.ReadFromJsonAsync<List<GetServiceDetailResponse>>();
                    if (apiResponse == null)
                    {
                        return NotFound();
                    }
                    Details = apiResponse;

                }
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        //create new service detail
        public async Task<IActionResult> OnPostCreateAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                await LoadServiceData(id);
                //back to page
                return Page();
            }

            try
            {
                NewDetail.ServiceId = id;
                await _serviceDetailApiClient.PostAsJsonAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.ServiceDetail.CreateServiceDetail), NewDetail);
                ToastHelper.ShowSuccess(TempData, "Create Service Detail successfully");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ToastHelper.ShowError(TempData, ex.Message);
            }

            await LoadServiceData(id);
            return Page();
        }

        private async Task LoadServiceData(int id)
        {
            try
            {
                // Gọi API để lấy thông tin Service
                var response = await _serviceApiClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Service.GetServiceById(id)));
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Service = JsonSerializer.Deserialize<ServiceResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new ServiceResponse();
                }
                else
                {
                    Service = new ServiceResponse();
                }

                // Gọi API để lấy danh sách ServiceDetail
                var responseDetail = await _serviceDetailApiClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.ServiceDetail.GetServiceDetailByService(id)));
                if (responseDetail.IsSuccessStatusCode)
                {
                    var jsonDetail = await responseDetail.Content.ReadAsStringAsync();
                    Details = JsonSerializer.Deserialize<List<GetServiceDetailResponse>>(jsonDetail, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<GetServiceDetailResponse>();
                }
                else
                {
                    Details = new List<GetServiceDetailResponse>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Service = new ServiceResponse();
                Details = new List<GetServiceDetailResponse>();
            }
        }

    }
}
