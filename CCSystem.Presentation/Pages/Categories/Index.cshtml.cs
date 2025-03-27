using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CCSystem.Presentation.Helpers;
using CCSystem.Presentation.Constants;
using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Models.Category;

namespace CCSystem.Presentation.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        public IndexModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("CategoryAPI");
            _apiEndpoints = apiEndpoints;
        }

        public List<CategoryResponse> Category { get; set; } = new List<CategoryResponse>();

        public async Task OnGetAsync()
        {
            var response = await _httpClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Category.GetAllCategories));
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<CategoryResponse>>>();
                Category = apiResponse?.Data ?? new List<CategoryResponse>();
            }
            else
            {
                ToastHelper.ShowError(TempData, "Cannot load categories");
            }
        }

        public async Task<IActionResult> OnPostLockAsync(int id)
        {
            var response = await _httpClient.PutAsync(
                            _apiEndpoints.GetFullUrl($"{_apiEndpoints.Category.DeleteCategory}/{id}"), null
                        );
            if (response.IsSuccessStatusCode)
            {
                ToastHelper.ShowSuccess(TempData, Message.Category.LockSuccessfully);
            }
            else
            {
                ToastHelper.ShowError(TempData, Message.Category.LockFailed);
            }
            return RedirectToPage();
        }
    }
}
    