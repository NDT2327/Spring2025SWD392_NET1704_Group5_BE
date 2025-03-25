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
using Firebase.Storage;
using CCSystem.Infrastructure.DTOs.Services;
using CCSystem.Presentation.Helpers;
using System.Text.Json;
using System.Net.Http;

namespace CCSystem.Presentation.Pages.Services
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _serviceApiClient;
        private readonly HttpClient _categoryApiClient;
        private readonly ApiEndpoints _apiEndpoints;

        public DeleteModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _serviceApiClient = httpClientFactory.CreateClient("ServiceAPI");
            _categoryApiClient = httpClientFactory.CreateClient("CategoryAPI");
            _apiEndpoints = apiEndpoints;
        }

        [BindProperty]
        public ServiceResponse Service { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _serviceApiClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Service.GetServiceById(id)));
            if (!response.IsSuccessStatusCode)
            {
                ToastHelper.ShowError(TempData, "Failed to load service");
                return NotFound();
            }
            var json = await response.Content.ReadAsStringAsync();
            var service = JsonSerializer.Deserialize<ServiceResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (service == null)
            {
                return NotFound();
            }
            else
            {
                Service = service;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var response = await _serviceApiClient.PutAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Service.DeleteService(id)), null);
            if (!response.IsSuccessStatusCode)
            {
                ToastHelper.ShowError(TempData, "Failed to delete service");
                return Page();
            }
            ToastHelper.ShowSuccess(TempData, "Delete Service successfully");
            return RedirectToPage("./Index");

        }
    }
}
