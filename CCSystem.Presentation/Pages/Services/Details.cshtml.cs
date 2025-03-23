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
        public List<GetServiceDetailResponse> Details { get; set; } = new();
        [BindProperty]
        public PostServiceDetailRequest NewDetail { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {

                var service = await _serviceService.GetServiceAsync(id.Value);
                if (service == null)
                {
                    return NotFound();
                }
                else
                {
                    Service = service;
                    Details = await _serviceDetailService.GetServiceDetailByServiceAsync(id.Value) ?? new List<GetServiceDetailResponse>();

                }
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        //create new service detail
        public async Task<IActionResult> OnPostCreateAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                await LoadServiceData(id);
                //back to page
                return Page();
            }

            try
            {
                NewDetail.ServiceId = id;
                await _serviceDetailService.CreateServiceDetailAsync(NewDetail);
                ToastHelper.ShowSuccess(TempData, "Create Service Detail successfully");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ToastHelper.ShowError(TempData, ex.Message);
            }

            await LoadServiceData(id);
            return Page();
        }

        private async Task LoadServiceData(int id)
        {
            try
            {
                Service = await _serviceService.GetServiceAsync(id) ?? new ServiceResponse();
                Details = await _serviceDetailService.GetServiceDetailByServiceAsync(id) ?? new List<GetServiceDetailResponse>();
            }
            catch (Exception ex) { 
                Console.Write(ex.Message);
                Service = new ServiceResponse();
                Details = new List<GetServiceDetailResponse>();
            }
        }
    }
}
