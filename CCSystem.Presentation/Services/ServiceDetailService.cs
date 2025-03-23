
﻿using CCSystem.Infrastructure.DTOs.ServiceDetails;
using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Helpers;

namespace CCSystem.Presentation.Services
{
    public class ServiceDetailService
    {
        public readonly HttpClient _httpClient;
        public readonly ApiEndpoints _apiEndpoints;

        public ServiceDetailService(IHttpClientFactory httpFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpFactory.CreateClient("ServiceDetailAPI");
            _apiEndpoints = apiEndpoints;
        }

        //get service detail by service
        public async Task<List<GetServiceDetailResponse>> GetServiceDetailByServiceAsync(int serviceId)
        {
            //get url
            var url = _apiEndpoints.GetFullUrl(_apiEndpoints.ServiceDetail.GetServiceDetailByService(serviceId));
            //send request

            try {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) {
                    return null;
                }
                var apiResponse = await response.Content.ReadFromJsonAsync<List<GetServiceDetailResponse>>();
                return (apiResponse != null ) ? apiResponse ?? new List<GetServiceDetailResponse>() : null;

            } catch (Exception ex) {
                Console.WriteLine(ex);
                return null;
            }


        }

        //get service detail
        public async Task<(bool Success, GetServiceDetailResponse Data, string ErrorMessage)> GetServiceDetail(int id)
        {
            var url = _apiEndpoints.GetFullUrl(_apiEndpoints.ServiceDetail.GetServiceDetail(id));
            try
            {
                //call api
                var response = await _httpClient.GetAsync(url);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<GetServiceDetailResponse>>();
                if (!response.IsSuccessStatusCode || apiResponse.StatusCode != 0)
                {
                    var errorMessage = apiResponse.Messages?.FirstOrDefault()?.DescriptionErrors?.FirstOrDefault();
                    return (false, null, errorMessage);
                }
                return (true, apiResponse.Data, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return (false, null, $"{ex.Message}");
            }
        }

        //create service detail
        public async Task CreateServiceDetailAsync(PostServiceDetailRequest request)
        {
            var url = _apiEndpoints.GetFullUrl(_apiEndpoints.ServiceDetail.CreateServiceDetail);
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, request);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode) throw new HttpRequestException($"Status: {response.StatusCode}, Response: {responseContent}");
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<PostServiceDetailResponse>>();
                if (apiResponse == null || apiResponse.StatusCode != 0)
                {
                    var errorMessage = apiResponse?.Messages?.FirstOrDefault()?.DescriptionErrors?.FirstOrDefault()
                        ?? "Unknown error from API";
                    throw new HttpRequestException(errorMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        //update service detail

        //delete service detail
    }
}

