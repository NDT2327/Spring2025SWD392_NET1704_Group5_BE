using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CCSystem.Presentation.Helpers;
using CCSystem.Presentation.Configurations;
using System.Text.Json;
using CCSystem.Presentation.Models.Services;
using CCSystem.Presentation.Models.Category;

namespace CCSystem.Presentation.Pages.Services
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _serviceApiClient;
        private readonly HttpClient _categoryApiClient;
        private readonly ApiEndpoints _apiEndpoints;
        public EditModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _serviceApiClient = httpClientFactory.CreateClient("ServiceAPI");
            _categoryApiClient = httpClientFactory.CreateClient("CategoryAPI");
            _apiEndpoints = apiEndpoints;
        }

        [BindProperty]
        public PostServiceRequest Service { get; set; } = default!;

        [BindProperty]
        public int ServiceId { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var serviceResponse = await _serviceApiClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Service.GetServiceById(id)));
                if (!serviceResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Service API Not Found for ID: {id}");
                    return NotFound();
                }
                var json = await serviceResponse.Content.ReadAsStringAsync();
                var service = JsonSerializer.Deserialize<ServiceResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (service == null)
                {
                    throw new Exception("Failed to deserialize service response.");
                }

                var categoryResponse = await _categoryApiClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Category.GetAllCategories));
                var categories = await categoryResponse.Content.ReadFromJsonAsync<ApiResponse<List<CategoryResponse>>>();
                if (categories?.Data == null)
                {
                    throw new Exception("Failed to retrieve categories.");
                }
                var selectedCategory = categories.Data.FirstOrDefault(c => c.CategoryName == service.CategoryName);
                if (selectedCategory == null)
                {
                    throw new Exception($"{service.CategoryName} not found");

                }
                Console.WriteLine(service.Image.ToString());
                IFormFile? file = await ConvertImageUrlToIFormFile(service.Image);
                if (file == null)
                {
                    Console.WriteLine("URL cannot Convert");
                }
                else
                {
                    Console.WriteLine("URL is Converted");
                }

                Service = new PostServiceRequest
                {
                    CategoryId = selectedCategory.CategoryId,
                    ServiceName = service.ServiceName,
                    Description = service.Description,
                    Price = service.Price,
                    Duration = service.Duration,
                    IsActive = service.IsActive,
                    Image = file!

                };
                ServiceId = service.ServiceId;
                ViewData["CategoryId"] = new SelectList(categories.Data, "CategoryId", "CategoryName", Service.CategoryId);
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                ToastHelper.ShowError(TempData, $"{ex.Message}");
                return Page();
            }
        }
        public async Task<IFormFile?> ConvertImageUrlToIFormFile(string imageUrl)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(imageUrl);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Failed to fetch image.");
                    return null; // Có thể gây lỗi CS8603
                }

                var stream = await response.Content.ReadAsStreamAsync();
                if (stream == null || stream.Length == 0)
                {
                    Console.WriteLine("Empty image stream.");
                    return null;
                }

                var fileName = Path.GetFileName(new Uri(imageUrl).LocalPath);
                var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                return new FormFile(memoryStream, 0, memoryStream.Length, "Image", fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream"
                };
            }
        }


        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var serviceResponse = await _serviceApiClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Service.GetServiceById(ServiceId)));
                if (!serviceResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Service API Not Found for ID: {ServiceId}");
                    return NotFound();
                }
                var json = await serviceResponse.Content.ReadAsStringAsync();
                var service = JsonSerializer.Deserialize<ServiceResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (service == null)
                {
                    throw new Exception("Failed to deserialize service response.");
                }
                var categoryResponse = await _categoryApiClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Category.GetAllCategories));
                var categories = await categoryResponse.Content.ReadFromJsonAsync<ApiResponse<List<CategoryResponse>>>();
                if (categories?.Data == null)
                {
                    throw new Exception("Failed to retrieve categories.");
                }
                var selectedCategory = categories.Data.FirstOrDefault(c => c.CategoryName == service.CategoryName);
                if (selectedCategory == null)
                {
                    throw new Exception($"{service.CategoryName} not found");

                }
                ViewData["CategoryId"] = new SelectList(categories.Data, "CategoryId", "CategoryName", Service.CategoryId);
                return Page();
            }

            try
            {
                var url = _apiEndpoints.GetFullUrl(_apiEndpoints.Service.UpdateService(ServiceId));
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
                    Console.WriteLine($"Uploading file: Name={Service.Image.FileName}, Size={Service.Image.Length}, ContentType={Service.Image.ContentType}");
                    var stream = Service.Image.OpenReadStream();
                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Service.Image.ContentType);
                    formData.Add(fileContent, "Image", Service.Image.Name);
                }

                Console.WriteLine("🔹 Sending FormData:");
                foreach (var content in formData)
                {
                    if (content is StringContent stringContent)
                    {
                        Console.WriteLine($"  ➤ {content.Headers.ContentDisposition?.Name}: {await stringContent.ReadAsStringAsync()}");
                    }
                    else if (content is StreamContent)
                    {
                        Console.WriteLine($"  ➤ {content.Headers.ContentDisposition?.Name}: (Binary File)");
                    }
                }
                Console.WriteLine($"Sending PUT to: {url}");
                var response = await _serviceApiClient.PutAsync(url, formData);
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response: Status {response.StatusCode}, Content: {responseContent}");
                if (response.IsSuccessStatusCode)
                {
                    ToastHelper.ShowSuccess(TempData, "Service updated successfully!");
                    return RedirectToPage("./Index");
                }
                else
                {
                    ToastHelper.ShowError(TempData, "Failed to update service");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ToastHelper.ShowError(TempData, $"{ex.Message}");
                var serviceResponse = await _serviceApiClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Service.GetServiceById(ServiceId)));
                if (!serviceResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Service API Not Found for ID: {ServiceId}");
                    return NotFound();
                }
                var json = await serviceResponse.Content.ReadAsStringAsync();
                var service = JsonSerializer.Deserialize<ServiceResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (service == null)
                {
                    throw new Exception("Failed to deserialize service response.");
                }
                var categoryResponse = await _categoryApiClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Category.GetAllCategories));
                var categories = await categoryResponse.Content.ReadFromJsonAsync<ApiResponse<List<CategoryResponse>>>();
                if (categories?.Data == null)
                {
                    throw new Exception("Failed to retrieve categories.");
                }
                var selectedCategory = categories.Data.FirstOrDefault(c => c.CategoryName == service.CategoryName);
                if (selectedCategory == null)
                {
                    throw new Exception($"{service.CategoryName} not found");

                }
                ViewData["CategoryId"] = new SelectList(categories.Data, "CategoryId", "CategoryName", Service.CategoryId);
                return Page();
            }
        }
    }
}
