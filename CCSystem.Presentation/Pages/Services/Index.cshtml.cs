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
using CCSystem.Presentation.Configurations;

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
