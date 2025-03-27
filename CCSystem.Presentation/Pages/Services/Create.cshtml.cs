using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Helpers;
using CCSystem.Presentation.Models.Services;
using CCSystem.Presentation.Models.Category;


namespace CCSystem.Presentation.Pages.Services
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _serviceApiClient;
        private readonly HttpClient _categoryApiClient;
        private readonly ApiEndpoints _apiEndpoints; 

        public CreateModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _serviceApiClient = httpClientFactory.CreateClient("ServiceAPI");
            _categoryApiClient = httpClientFactory.CreateClient("CategoryAPI");
            _apiEndpoints = apiEndpoints;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var categoryResponse = await _categoryApiClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Category.GetAllCategories));
            var categories = await categoryResponse.Content.ReadFromJsonAsync<ApiResponse<List<CategoryResponse>>>();
            if (categories?.Data == null)
            {
                throw new Exception("Failed to retrieve categories.");
            }
            ViewData["CategoryId"] = new SelectList(categories.Data, "CategoryId", "CategoryName");
            return Page();
        }

        [BindProperty]
        public PostServiceRequest Service { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var categoryResponse = await _categoryApiClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Category.GetAllCategories));
                var categories = await categoryResponse.Content.ReadFromJsonAsync<ApiResponse<List<CategoryResponse>>>();
                if (categories?.Data == null)
                {
                    throw new Exception("Failed to retrieve categories.");
                }
                ViewData["CategoryId"] = new SelectList(categories.Data, "CategoryId", "CategoryName", Service.CategoryId);
                return Page();
            }

            var formData = new MultipartFormDataContent
            {
                { new StringContent(Service.CategoryId.ToString()), "CategoryId" },
                { new StringContent(Service.ServiceName), "ServiceName" },
                { new StringContent(Service.Description), "Description" },
                { new StringContent(Service.Price.ToString()), "Price" },
                { new StringContent(Service.Duration.ToString()), "Duration" }
            };

            if (Service.IsActive.HasValue)
            {
                formData.Add(new StringContent(Service.IsActive.Value.ToString()), "IsActive");
            }

            if (Service.Image != null && Service.Image.Length > 0)
            {
                var stream = Service.Image.OpenReadStream();
                var imageContent = new StreamContent(stream);
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Service.Image.ContentType);
                formData.Add(imageContent, "Image", Service.Image.FileName);
            }

            var response = await _serviceApiClient.PostAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Service.CreateService), formData);
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                ToastHelper.ShowSuccess(TempData, "Service create successfully!");
                return RedirectToPage("./Index");
            }
            else
            {
                ToastHelper.ShowError(TempData, "Failed to create service");
                return Page();
            }
        }
    }
}
