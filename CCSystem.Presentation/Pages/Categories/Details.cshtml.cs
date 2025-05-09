﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Models.Category;

namespace CCSystem.Presentation.Pages.Categories
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        public DetailsModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("CategoryAPI");
            _apiEndpoints = apiEndpoints;
        }

        public CategoryResponse Category { get; set; } = new CategoryResponse();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _httpClient.GetFromJsonAsync<CategoryResponse>(
                 _apiEndpoints.GetFullUrl($"{_apiEndpoints.Category.GetCategory}/{id}")
             );

            if (response == null)
            {
                return NotFound();
            }

            Category = response;
            return Page();
        }
    }
}