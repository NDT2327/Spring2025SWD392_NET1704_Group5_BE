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

namespace CCSystem.Presentation.Pages.Services
{
    public class FilterModel : PageModel
    {
        private readonly ServiceService _serviceService;
        public IList<ServiceResponse> Services { get; set; } = new List<ServiceResponse>();

        public FilterModel(ServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        public async Task OnGetAsync()
        {
            var services = await _serviceService.GetServicesAsync();
            if (services != null)
            {
                Services = services;
            }
        }
    }
}
