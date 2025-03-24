using CCSystem.Infrastructure.DTOs.BookingDetails;
using CCSystem.Infrastructure.DTOs.Bookings;
using CCSystem.Infrastructure.DTOs.ServiceDetails;
using CCSystem.Infrastructure.DTOs.Services;
using CCSystem.Presentation.Constants;
using CCSystem.Presentation.Helpers;
using CCSystem.Presentation.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace CCSystem.Presentation.Pages
{
    public class BookingModel : PageModel
    {
        private readonly BookingService _bookingService;
        private readonly ServiceService _serviceService;
        private readonly ServiceDetailService _serviceDetailService;

        public BookingModel(BookingService bookingService, ServiceDetailService serviceDetailService, ServiceService serviceService)
        {
            _bookingService = bookingService;
            _serviceService = serviceService;
            _serviceDetailService = serviceDetailService;
        }

        [BindProperty]
        public PostBookingRequest BookingRequest { get; set; } = new PostBookingRequest();

        [BindProperty]
        public PostBookingDetailRequest NewBookingDetail { get; set; } = new PostBookingDetailRequest();

        [BindProperty(SupportsGet = true)]
        public List<int> SelectedServices { get; set; } = new List<int>();

        public async Task<IActionResult> OnGetAsync()
        {
            if (SelectedServices == null || !SelectedServices.Any())
            {
                SelectedServices = new List<int>();
            }

            BookingRequest.BookingDetails = SelectedServices
                .Select(serviceId => new PostBookingDetailRequest { ServiceId = serviceId })
                .ToList();

            var services = new List<(ServiceResponse service, List<GetServiceDetailResponse> details)>();

            foreach (var serviceId in SelectedServices)
            {
                var service = await _serviceService.GetServiceAsync(serviceId);
                if (service != null)
                {
                    var details = await _serviceDetailService.GetServiceDetailByServiceAsync(serviceId);
                    services.Add((service, details));
                }
            }

            ViewData["Services"] = services;

            return Page();
        }


        public IActionResult OnPostAddDetail()
        {
            if (ModelState.IsValid)
            {
                BookingRequest.BookingDetails.Add(NewBookingDetail);
                NewBookingDetail = new PostBookingDetailRequest();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    Console.WriteLine($"Result1: {ModelState.IsValid}");
            //    return Page();
            //}
            Console.WriteLine($"BookingRequest: {JsonSerializer.Serialize(BookingRequest)}");

            string userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine(userIdString);
            if (int.TryParse(userIdString, out int userId))
            {

                BookingRequest.CustomerId = userId;
                Console.WriteLine(BookingRequest.CustomerId);
            }
            var result = await _bookingService.CreateBooking(BookingRequest);
            if (result == null)
            {
                ToastHelper.ShowError(TempData, Message.Bookig.CreatedSuccessfully);
                ModelState.AddModelError("", "Failed to create booking");
                return await ReloadPage();
            }
            ToastHelper.ShowSuccess(TempData, Message.Bookig.CreatedFailed);
            return RedirectToPage("/Index");
        }


        private async Task<IActionResult> ReloadPage()
        {
            ModelState.Clear(); 

            var services = new List<(ServiceResponse service, List<GetServiceDetailResponse> details)>();

            foreach (var serviceId in SelectedServices)
            {
                var service = await _serviceService.GetServiceAsync(serviceId);
                if (service != null)
                {
                    var details = await _serviceDetailService.GetServiceDetailByServiceAsync(serviceId);
                    services.Add((service, details));
                }
            }

            ViewData["Services"] = services;

            return Page();
        }
    }
}