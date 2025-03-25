using CCSystem.Presentation.Configurations;
using CCSystem.Infrastructure.DTOs.Services;
using Azure;
using CCSystem.Infrastructure.DTOs.Accounts;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using CCSystem.Infrastructure.DTOs.ServiceDetails;
using System.Security.Policy;
using CCSystem.Infrastructure.DTOs.Category;

namespace CCSystem.Presentation.Services
{
    public class ServiceService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public ServiceService(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("ServiceAPI");
            _apiEndpoints = apiEndpoints;
        }

        //Get all HomeService
        public async Task<List<ServiceResponse>?> GetServicesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<ServiceResponse>>(_apiEndpoints.GetFullUrl(_apiEndpoints.Service.GetServices));
            return response?.Where(x => x.IsActive == true).ToList() ?? new List<ServiceResponse>();
        }

        // Get a service by ID
        public async Task<ServiceResponse?> GetServiceAsync(int id)
        {
            var response = await _httpClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Service.GetServiceById(id)));
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ServiceResponse>(json, _jsonOptions);
        }

        // Create a new service
        public async Task<bool> CreateServiceAsync(PostServiceRequest request)
        {
            var formData = new MultipartFormDataContent
            {
                { new StringContent(request.CategoryId.ToString()), "CategoryId" },
                { new StringContent(request.ServiceName), "ServiceName" },
                { new StringContent(request.Description), "Description" },
                { new StringContent(request.Price.ToString()), "Price" },
                { new StringContent(request.Duration.ToString()), "Duration" }
            };

            if (request.IsActive.HasValue)
            {
                formData.Add(new StringContent(request.IsActive.Value.ToString()), "IsActive");
            }

            if (request.Image != null && request.Image.Length > 0)
            {
                using var stream = request.Image.OpenReadStream();
                var imageContent = new StreamContent(stream);
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(request.Image.ContentType);
                formData.Add(imageContent, "Image", request.Image.FileName);
            }

            var response = await _httpClient.PostAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Service.CreateService), formData);
            return response.IsSuccessStatusCode;
        }

        // Update a service
        public async Task<bool> UpdateServiceAsync(int serviceId, PostServiceRequest request)
        {
            var url = _apiEndpoints.GetFullUrl(_apiEndpoints.Service.UpdateService(serviceId));
            var formData = new MultipartFormDataContent
            {
                { new StringContent(request.CategoryId.ToString()), "CategoryId" },
                { new StringContent(request.ServiceName), "ServiceName" },
                { new StringContent(request.Description), "Description" },
                { new StringContent(request.Price.ToString()), "Price" },
                { new StringContent(request.Duration.ToString()), "Duration" }
            };

            if (request.IsActive.HasValue)
            {
                formData.Add(new StringContent(request.IsActive.Value.ToString()), "IsActive");
            }

            if (request.Image != null && request.Image.Length > 0)
            {
                try
                {
                    Console.WriteLine($"Uploading file: Name={request.Image.FileName}, Size={request.Image.Length}, ContentType={request.Image.ContentType}");
                    var stream = request.Image.OpenReadStream();
                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(request.Image.ContentType);
                    formData.Add(fileContent, "Image", request.Image.FileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error preparing image stream: {ex.Message}");
                    throw; // Ném lỗi để tầng trên xử lý
                }
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

            try
            {
                Console.WriteLine($"Sending PUT to: {url}");
                var response = await _httpClient.PutAsync(url, formData);
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response: Status {response.StatusCode}, Content: {responseContent}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending request: {ex.Message}");
                throw; // Ném lỗi để tầng trên xử lý
            }
        }
        // Delete (Deactivate) a service
        public async Task<bool> DeleteServiceAsync(int id)
        {
            var response = await _httpClient.PutAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Service.DeleteService(id)), null);
            return response.IsSuccessStatusCode;
        }

        // Search services
        public async Task<List<ServiceResponse>> SearchServicesAsync([FromQuery] SearchServiceRequest request)
        {
            var queryParams = new List<string>();
            Console.WriteLine($"🔍 Received: {JsonSerializer.Serialize(request)}");

            if (!string.IsNullOrEmpty(request.ServiceName)) queryParams.Add($"ServiceName={Uri.EscapeDataString(request.ServiceName)}");
            if (!string.IsNullOrEmpty(request.Description)) queryParams.Add($"Description={Uri.EscapeDataString(request.Description)}");
            if (request.Price.HasValue) queryParams.Add($"Price={request.Price.Value}");
            if (request.Duration.HasValue) queryParams.Add($"Duration={request.Duration.Value}");
            if (request.IsActive.HasValue) queryParams.Add($"IsActive={request.IsActive.Value.ToString().ToLower()}");
            if (!string.IsNullOrEmpty(request.CategoryName)) queryParams.Add($"CategoryName={Uri.EscapeDataString(request.CategoryName)}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var url = _apiEndpoints.GetFullUrl(_apiEndpoints.Service.SearchServices) + queryString;
            Console.WriteLine($"🔍 Search URL: {url}");
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<ServiceResponse>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ServiceResponse>>(json, _jsonOptions) ?? new List<ServiceResponse>();
        }

        // Get services by category ID
        //public async Task<List<ServiceResponse>> GetServicesByCategoryAsync(int categoryId)
        //{
        //    var response = await _httpClient.GetFromJsonAsync<List<ServiceResponse>>(_apiEndpoints.GetFullUrl(_apiEndpoints.Service.GetServicesByCategory(categoryId)));
        //    return response ?? new List<ServiceResponse>();
        //}
    }
}
