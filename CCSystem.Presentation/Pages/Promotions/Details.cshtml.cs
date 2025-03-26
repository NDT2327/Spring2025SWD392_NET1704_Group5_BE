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
using CCSystem.Infrastructure.DTOs.Accounts;
using System.Text.Json;
using CCSystem.Infrastructure.DTOs.Promotions;

namespace CCSystem.Presentation.Pages.Promotions
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;
        public DetailsModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("PromotionAPI");
            _apiEndpoints = apiEndpoints;
        }

        public GetPromotionResponse Promotion { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var response = await _httpClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Promotion.GetPromtion(id)));
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Promotion = JsonSerializer.Deserialize<GetPromotionResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new GetPromotionResponse();
            }

            if (Promotion == null)
            {
                return RedirectToPage("Index");
            }

            return Page();
        }
    }
}
