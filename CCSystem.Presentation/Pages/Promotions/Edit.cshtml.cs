using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CCSystem.Presentation.Configurations;
using System.Text.Json;
using CCSystem.Infrastructure.DTOs.Accounts;
using System.Net.Http;
using CCSystem.Presentation.Models.Promotions;

namespace CCSystem.Presentation.Pages.Promotions
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;
        public EditModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("PromotionAPI");
            _apiEndpoints = apiEndpoints;
        }

        [BindProperty]
        public PostPromotionRequest Promotion { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            string json = string.Empty;
            var response = await _httpClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Promotion.GetPromtion(id)));
            if (response.IsSuccessStatusCode)
            {
                json = await response.Content.ReadAsStringAsync();
            }
            if (string.IsNullOrWhiteSpace(json))
            {
                return NotFound();
            }
            var promotion = JsonSerializer.Deserialize<GetPromotionResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                 ?? new GetPromotionResponse();
            if (promotion == null)
            {
                return NotFound();
            }
            Promotion = new PostPromotionRequest
            {
                Code = promotion.Code,
                DiscountAmount = promotion.DiscountAmount,
                DiscountPercent = promotion.DiscountPercent,
                EndDate = promotion.EndDate,
                MaxDiscountAmount = promotion.MaxDiscountAmount,
                MinOrderAmount = promotion.MinOrderAmount,
                StartDate = promotion.StartDate,
            };
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var promotionData = new
            {
                discountAmount = Promotion.DiscountAmount,
                discountPercent = Promotion.DiscountPercent,
                startDate = Promotion.StartDate,
                endDate = Promotion.EndDate,
                minOrderAmount = Promotion.MinOrderAmount,
                maxDiscountAmount = Promotion.MaxDiscountAmount
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(promotionData),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await _httpClient.PutAsync(
                    _apiEndpoints.GetFullUrl(_apiEndpoints.Promotion.UpdatePromotion(Promotion.Code)),
                    jsonContent
                );

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Cập nhật thất bại.");
                    return Page();
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Lỗi hệ thống.");
                return Page();
            }

            return RedirectToPage("./Index");
        }

    }
}
