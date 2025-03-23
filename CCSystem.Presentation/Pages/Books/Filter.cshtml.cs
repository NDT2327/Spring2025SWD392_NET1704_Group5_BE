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

namespace CCSystem.Presentation.Pages.Services
{
    public class FilterModel : PageModel
    {
        private readonly ServiceService _serviceService;
        private readonly CategoryService _categoryService;
        public IList<ServiceResponse> Services { get; set; } = new List<ServiceResponse>();
        public List<SelectListItem> CategoryList { get; set; } = new();

        public FilterModel(ServiceService serviceService, CategoryService categoryService)
        {
            _serviceService = serviceService;
            _categoryService = categoryService;
        }


        [BindProperty(SupportsGet = true)]
        public string SearchServiceName { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public decimal? SearchPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SelectedCategory { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 6;

        public async Task OnGetAsync(int currentPage = 1)
        {
            CurrentPage = currentPage;

            var category = await _categoryService.GetCategoryAsync(SelectedCategory);

            var request = new SearchServiceRequest
            {
                ServiceName = SearchServiceName,
                Price = SearchPrice,
                CategoryName = category?.CategoryName ?? "Unknown",
                IsActive = true
            };

            var allServices = await _serviceService.SearchServicesAsync(request);

            // Tính toán tổng số trang
            int totalItems = allServices.Count;
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            // Lọc dữ liệu theo trang
            Services = allServices.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

            // Load danh sách danh mục
            var categories = await _categoryService.GetAllCategoriesAsync();
            CategoryList = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToList();
            CategoryList.Insert(0, new SelectListItem { Value = "", Text = "All" });
        }

    }
}
