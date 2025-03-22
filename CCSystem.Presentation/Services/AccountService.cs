using Azure.Core;
using CCSystem.Infrastructure.DTOs.Accounts;
using CCSystem.Presentation.Configurations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace CCSystem.Presentation.Services
{
    public class AccountService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        public AccountService(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("AccountAPI");
            _apiEndpoints = apiEndpoints;
        }

        //get all accountsdo
        public async Task<List<GetAccountResponse>?> GetAccountsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<GetAccountResponse>>(_apiEndpoints.GetFullUrl(_apiEndpoints.Account.GetAccounts));
            return response ?? new List<GetAccountResponse>();
        }

        //get detail
        public async Task<GetAccountResponse?> GetAccountByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Account.GetAccountDetailsUrl(id)));
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GetAccountResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return new GetAccountResponse();
        }

        //create account

        public async Task<bool> CeateAccountAsync(CreateAccountRequest newAccount)
        {
            var response = await _httpClient.PostAsJsonAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Account.CreateAccount), newAccount);
            return response.IsSuccessStatusCode;
        }

        //update account
        public async Task<bool> UpdateAccountAsync(int id, UpdateAccountRequest request)
        {
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(request.Address ?? ""), "Address");
            content.Add(new StringContent(request.Phone ?? ""), "Phone");
            content.Add(new StringContent(request.FullName ?? ""), "FullName");
            content.Add(new StringContent(request.Gender ?? ""), "Gender");
            content.Add(new StringContent(request.Status ?? ""), "Status");

            if (request.Rating.HasValue) content.Add(new StringContent(request.Rating.Value.ToString()), "Rating");

            if (request.Experience.HasValue) content.Add(new StringContent(request.Experience.Value.ToString()), "Experience");

            if(request.Year.HasValue) content.Add(new StringContent(request.Year.Value.ToString()), "Year");

            if (request.Year.HasValue) content.Add(new StringContent(request.Month.Value.ToString()), "Month");

            if (request.Year.HasValue) content.Add(new StringContent(request.Day.Value.ToString()), "Day");

            //process avatar
            if(request.Avatar != null)
            {
                using var stream = request.Avatar.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(request.Avatar.ContentType);
                content.Add(fileContent, "Avatar", request.Avatar.FileName);
            }

            //send PUT to API
            var response = await _httpClient.PutAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Account.UpdateAccountUrl(id)), content);

            return response.IsSuccessStatusCode;
        }

        //lock
        public async Task<bool> LockAccountAsync(int id)
        {
            var response = await _httpClient.PutAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Account.LockAccountUrl(id)), null);
            return response.IsSuccessStatusCode;
        }
        //unlock
        public async Task<bool> UnlockAccountAsync(int id)
        {
            var url = _apiEndpoints.GetFullUrl(_apiEndpoints.Account.UnlockAccountUrl(id));
            var response = await _httpClient.PutAsync(url, null);
            return response.IsSuccessStatusCode;
        }
    }
}
