using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CCSystem.DAL.DBContext;
using CCSystem.DAL.Models;
using CCSystem.Presentation.Services;
using CCSystem.Infrastructure.DTOs.ServiceDetails;
using CCSystem.Presentation.Helpers;

namespace CCSystem.Presentation.Pages.ServiceDetails
{
    public class CreateModel : PageModel
    {
        private readonly ServiceDetailService _serviceDetail;
        public CreateModel(ServiceDetailService serviceDetailService)
        {
            _serviceDetail = serviceDetailService;
        }

        [BindProperty]
        public PostServiceDetailRequest ServiceDetail { get; set; } = new();

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var (success, errorMessage) = await _serviceDetail.CreateServiceDetailAsync(ServiceDetail);
            if (!success)
            {
                ToastHelper.ShowError(TempData, errorMessage);
                return Page();
            }
            ToastHelper.ShowSuccess(TempData, "Service detail created successfully");

            return RedirectToPage("./Index");
        }
    }
}
