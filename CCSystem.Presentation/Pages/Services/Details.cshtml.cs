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
using CCSystem.Infrastructure.DTOs.Services;
using CCSystem.Infrastructure.DTOs.ServiceDetails;
using CCSystem.Presentation.Helpers;

namespace CCSystem.Presentation.Pages.Services
{
    public class DetailsModel : PageModel
    {
        private readonly ServiceService _serviceService;
        private readonly ServiceDetailService _serviceDetailService;
        public DetailsModel(ServiceService serviceService, ServiceDetailService serviceDetailService)
        {

            _serviceService = serviceService;
            _serviceDetailService = serviceDetailService;
        }

        public ServiceResponse Service { get; set; } = new();
        public List<GetServiceDetailResponse> Details { get; set; }
        [BindProperty]
        public PostServiceDetailRequest NewDetail { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _serviceService.GetServiceAsync(id.Value);
            if (service == null)
            {
                return NotFound();
            }
            else
            {
                Service = service;
                var (detailSuccess, detailsData, detailsError) = await _serviceDetailService.GetServiceDetailByServiceAsync(id.Value);
                Details = detailSuccess ? detailsData : new List<GetServiceDetailResponse>();
            }
            return Page();
        }

        //create new service detail
        public async Task<IActionResult> OnPostCreateAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                //back to page
                return Page();
            }

            NewDetail.ServiceId = id;
            var (createSuccess, createError) = await _serviceDetailService.CreateServiceDetailAsync(NewDetail);
            if (!createSuccess)
            {
                ToastHelper.ShowError(TempData, createError);
            }

            await LoadServiceData(id);
            return Page();
        }

        private async Task LoadServiceData(int id)
        {
            var serviceData = await _serviceService.GetServiceAsync(id);
            if (serviceData == null)
            {
                serviceData = new ServiceResponse();
            }
            Service = serviceData;

            var (detailSuccess, detailData, errors) = await _serviceDetailService.GetServiceDetailByServiceAsync(id);
            Details = detailSuccess ? detailData : new List<GetServiceDetailResponse>();
        }
    }
}
