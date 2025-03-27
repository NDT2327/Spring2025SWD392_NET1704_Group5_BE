using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Models.Services;

namespace CCSystem.Presentation.Pages.Services
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;
        public IList<ServiceResponse> Services { get; set; } = default!;

        public IndexModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("ServiceAPI");
            _apiEndpoints = apiEndpoints;
        }

        public async Task OnGetAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<ServiceResponse>>(_apiEndpoints.GetFullUrl(_apiEndpoints.Service.GetServices));
            if (response != null)
            {
                Services = response.Where(x => x.IsActive == true).ToList() ?? new List<ServiceResponse>();
            }
        }
    }
}
