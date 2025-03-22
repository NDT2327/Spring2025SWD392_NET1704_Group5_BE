using CCSystem.Presentation.Configurations;
using CCSystem.Infrastructure.DTOs.Services;
using Azure;
using CCSystem.Infrastructure.DTOs.Accounts;
using System.Text.Json;

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
            return response ?? new List<ServiceResponse>();
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

            var response = await _httpClient.PutAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Service.UpdateService(serviceId)), formData);
            return response.IsSuccessStatusCode;
        }
        // Delete (Deactivate) a service
        public async Task<bool> DeleteServiceAsync(int id)
        {
            var response = await _httpClient.PutAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Service.DeleteService(id)), null);
            return response.IsSuccessStatusCode;
        }

        // Search services
        public async Task<List<ServiceResponse>> SearchServicesAsync(SearchServiceRequest request)
        {
            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(request.ServiceName)) queryParams.Add($"ServiceName={Uri.EscapeDataString(request.ServiceName)}");
            if (!string.IsNullOrEmpty(request.Description)) queryParams.Add($"Description={Uri.EscapeDataString(request.Description)}");
            if (request.Price.HasValue) queryParams.Add($"Price={request.Price.Value}");
            if (request.Duration.HasValue) queryParams.Add($"Duration={request.Duration.Value}");
            if (request.IsActive.HasValue) queryParams.Add($"IsActive={request.IsActive.Value.ToString().ToLower()}");
            if (!string.IsNullOrEmpty(request.CategoryName)) queryParams.Add($"CategoryName={Uri.EscapeDataString(request.CategoryName)}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var url = _apiEndpoints.GetFullUrl(_apiEndpoints.Service.SearchServices) + queryString;

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
