using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Models.Promotions;

namespace CCSystem.Presentation.Pages.Promotions
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;
        public IndexModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("PromotionAPI");
            _apiEndpoints = apiEndpoints;
        }

        public IList<GetPromotionResponse> Promotion { get;set; } = default!;

        public async Task OnGetAsync()
        {
            try
            {
                var promotions = await _httpClient.GetFromJsonAsync<List<GetPromotionResponse>>(_apiEndpoints.GetFullUrl(_apiEndpoints.Promotion.GetAllPromotions));
                if (promotions == null)
                {
                    promotions = new List<GetPromotionResponse>();
                }
                //filter account
                Promotion = promotions;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Promotion = new List<GetPromotionResponse>();
            }
        }
    }
}
