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
using CCSystem.Infrastructure.DTOs.Accounts;
using CCSystem.Infrastructure.DTOs.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using CCSystem.Presentation.Configurations;
using System.Text.Json;
using CCSystem.Infrastructure.DTOs.Category;
using CCSystem.Presentation.Helpers;

namespace CCSystem.Presentation.Pages.Services
{
    public class FilterModel : PageModel
    {
        //private readonly ServiceService _serviceService;
        //private readonly CategoryService _categoryService;

        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;
        public IList<ServiceResponse> Services { get; set; } = new List<ServiceResponse>();
        public List<SelectListItem> CategoryList { get; set; } = new();

        //public FilterModel(ServiceService serviceService, CategoryService categoryService)
        //{
        //    _serviceService = serviceService;
        //    _categoryService = categoryService;
        //}

        public FilterModel(HttpClient httpClient, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClient;
            _apiEndpoints = apiEndpoints;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchServiceName { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public decimal? SearchPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SelectedCategory { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedServicesJson { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 6;

        public async Task OnGetAsync(int currentPage = 1)
        {
            CurrentPage = currentPage;

            //categories
            await LoadCategories();
            //search services
            await LoadService();

            if (string.IsNullOrEmpty(SelectedServicesJson))
            {
                SelectedServicesJson = JsonSerializer.Serialize(new List<int>());
            }

        }

        private async Task LoadCategories()
        {
            try
            {
                //call api
                var response = await _httpClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Category.GetAllCategories));
                //if success
                if (response.IsSuccessStatusCode)
                {
                    ///read json
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<CategoryResponse>>>();

                    //check api return success 
                    if (apiResponse?.Data != null)
                    {
                        CategoryList = apiResponse.Data.Where(c => c.IsActive == true).Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.CategoryName }).ToList() ?? new List<SelectListItem>();

                        CategoryList.Insert(0, new SelectListItem { Value = "", Text = "All" });

                    }
                    else if (apiResponse?.Data != null && apiResponse.Messages.Any())
                    {
                        ToastHelper.ShowError(TempData, apiResponse.GetError());
                    }
                }
                else
                {
                    ToastHelper.ShowError(TempData, "Failed to fetch categories from API.");
                }
            }
            catch (Exception ex)
            {

                ToastHelper.ShowError(TempData, $"{ex.Message}");
            }
        }


        //load services
        private async Task LoadService()
        {
            try
            {
                //create request
                var request = new SearchServiceRequest
                {
                    ServiceName = SearchServiceName,
                    Price = SearchPrice,
                    IsActive = true,//get the service with IsActive is true
                    CategoryName = SelectedCategory > 0 ? CategoryList.FirstOrDefault(c => c.Value == SelectedCategory.ToString())?.Text : null
                };

                //query string
                var queryString = BuildQueryString();

                var url = $"{_apiEndpoints.GetFullUrl(_apiEndpoints.Service.SearchServices)}{queryString}";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ToastHelper.ShowError(TempData, $"{response.ReasonPhrase} - {errorContent}");
                    return;
                }

                var content = await response.Content.ReadAsStringAsync();
                var serviceData = JsonSerializer.Deserialize<List<ServiceResponse>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<ServiceResponse>();

                //filter services
                var filteredServices = serviceData
                                    .Where(s => s.IsActive == true)
                                    .Where(s => string.IsNullOrEmpty(SearchServiceName) || s.ServiceName.Contains(SearchServiceName, StringComparison.OrdinalIgnoreCase))
                                    .Where(s => !SearchPrice.HasValue || s.Price <= SearchPrice.Value)
                                    .Where(s => SelectedCategory == 0 || s.CategoryName == CategoryList.FirstOrDefault(c => c.Value == SelectedCategory.ToString())?.Text)
                                    .ToList();

                TotalPages = (int)Math.Ceiling(filteredServices.Count / (double)PageSize);
                Services = filteredServices
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                ToastHelper.ShowError(TempData, $"{ex.Message}");
            }
        }

        private string BuildQueryString()
        {
            var queryParams = new List<string>
            {
                $"isActive=true",
                $"serviceName={Uri.EscapeDataString(SearchServiceName ?? "")}",
                $"price={SearchPrice}"
            };

            // Chỉ thêm categoryId nếu SelectedCategory > 0
            if (SelectedCategory > 0)
            {
                queryParams.Add($"categoryId={SelectedCategory}");
            }

            return "?" + string.Join("&", queryParams);
        }

    }
}
