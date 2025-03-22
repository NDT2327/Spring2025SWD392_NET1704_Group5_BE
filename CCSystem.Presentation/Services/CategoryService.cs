using CCSystem.Infrastructure.DTOs.Category;
using CCSystem.Presentation.Configurations;
using System.Text.Json;

namespace CCSystem.Presentation.Services
{
    public class CategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        public CategoryService(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("CategoryAPI");
            _apiEndpoints = apiEndpoints;
        }

        public async Task<List<CategoryResponse>> GetAllCategoriesAsync()
        {
            var url = _apiEndpoints.GetFullUrl(_apiEndpoints.Category.GetAllCategories);
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new List<CategoryResponse>();
            }

            var jsonString = await response.Content.ReadAsStringAsync();

            try
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<CategoryResponse>>(jsonString);
                return apiResponse?.Data ?? new List<CategoryResponse>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new List<CategoryResponse>();
            }
        }

        public async Task<bool> CreateCategoryAsync(CategoryRequest categoryRequest)
        {
            try
            {
                var url = _apiEndpoints.GetFullUrl(_apiEndpoints.Category.CreateCategory);

                using var content = new MultipartFormDataContent();

                // Thêm tên category
                content.Add(new StringContent(categoryRequest.CategoryName), "CategoryName");

                // Thêm mô tả
                content.Add(new StringContent(categoryRequest.Description), "Description");

                // Thêm trạng thái (true/false)
                content.Add(new StringContent(categoryRequest.IsActive.ToString()), "IsActive");

                // Nếu có ảnh, thêm ảnh vào form-data
                if (categoryRequest.Image != null)
                {
                    using var stream = categoryRequest.Image.OpenReadStream();
                    var imageContent = new StreamContent(stream);
                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(categoryRequest.Image.ContentType);
                    content.Add(imageContent, "Image", categoryRequest.Image.FileName);
                }

                var response = await _httpClient.PostAsync(url, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<CategoryResponse?> GetCategoryAsync(int id)
        {
            var url = _apiEndpoints.GetFullUrl(_apiEndpoints.Category.GetCategory(id));

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var jsonString = await response.Content.ReadAsStringAsync();

            var jsonDoc = JsonDocument.Parse(jsonString);
            var dataElement = jsonDoc.RootElement.GetProperty("data");

            var category = JsonSerializer.Deserialize<CategoryResponse>(dataElement.GetRawText(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return category;
        }


        public async Task<bool> UpdateCategoryAsync(int id, CategoryRequest categoryRequest)
        {
            try
            {
                var url = _apiEndpoints.GetFullUrl(_apiEndpoints.Category.UpdateCategory(id));
                var content = new MultipartFormDataContent
            {
                {new StringContent(categoryRequest.CategoryName), "CategoryName"},
                {new StringContent(categoryRequest.Description), "Description" },
                {new StringContent(categoryRequest.IsActive.ToString()), "IsActive" },
            };

                if (categoryRequest.Image != null && categoryRequest.Image.Length > 0)
                {
                    var stream = categoryRequest.Image.OpenReadStream();
                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(categoryRequest.Image.ContentType);
                    content.Add(fileContent, "Image", categoryRequest.Image.FileName);
                }

                var response = await _httpClient.PutAsync(url, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> LockCategoryAsync(int id)
        {
            var url = _apiEndpoints.GetFullUrl(_apiEndpoints.Category.DeleteCategory(id));
            var response = await _httpClient.PutAsync(url, null);
            return response.IsSuccessStatusCode;
        }
    }
}
