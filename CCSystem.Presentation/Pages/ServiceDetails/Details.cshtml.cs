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
using CCSystem.Infrastructure.DTOs.ServiceDetails;
using CCSystem.Presentation.Helpers;

namespace CCSystem.Presentation.Pages.ServiceDetails
{
    public class DetailsModel : PageModel
    {
        private readonly ServiceDetailService _serviceDetailService;
        public DetailsModel(ServiceDetailService serviceDetailService)
        {
            _serviceDetailService = serviceDetailService;
        }

        public int ServiceId { get; set; }
        public GetServiceDetailResponse ServiceDetail { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var (success, data, errorMessage) = await _serviceDetailService.GetServiceDetail(id.Value);
            if (!success)
            {
                ToastHelper.ShowError(TempData, errorMessage);
                ServiceDetail = new GetServiceDetailResponse();
            }
            else
            {
                ServiceDetail = data;
            }
            return Page();
        }
    }
}
