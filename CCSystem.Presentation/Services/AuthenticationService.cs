using CCSystem.Infrastructure.DTOs.Accounts;
using CCSystem.Presentation.Configurations;
using System.Text.Json;

namespace CCSystem.Presentation.Services
{
    public class AuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        public AuthenticationService(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints) 
        {
            _httpClient = httpClientFactory.CreateClient("AuthenticationAPI");
            _apiEndpoints = apiEndpoints;
        }

        //register
        public async Task<AccountResponse?> RegisterAsync(AccountRegisterRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Authentication.Register), request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AccountResponse>();
        }

        //login
        public async Task<AccountResponse?> LoginAsync(AccountLoginRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Authentication.Login), request);
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<AccountResponse>() : null;
        }
    }
}
