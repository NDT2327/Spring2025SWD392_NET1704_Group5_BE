using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Helpers;
using CCSystem.Presentation.Models.Promotions;


namespace CCSystem.Presentation.Pages.Promotions
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;
        public CreateModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("PromotionAPI");
            _apiEndpoints = apiEndpoints;
        }
        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public PostPromotionRequest Promotion { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var result = await _httpClient.PostAsJsonAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Promotion.CreatePromotion), Promotion);
            if (!result.IsSuccessStatusCode)
            {
                ToastHelper.ShowError(TempData, "Promotion is created failure!");
                return Page();
            }
            ToastHelper.ShowSuccess(TempData, "Promotion is created successfully");
            return RedirectToPage("./Index");
        }
    }
}
