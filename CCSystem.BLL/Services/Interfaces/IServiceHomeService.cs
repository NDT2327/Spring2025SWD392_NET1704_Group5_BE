﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCSystem.Infrastructure.DTOs.Services;

namespace CCSystem.BLL.Services.Interfaces
{
    public interface IServiceHomeService
    {
        public Task CreateServiceAsync(PostServiceRequest request);

        public Task<List<ServiceResponse>> SearchServiceAsync(SearchServiceRequest request);

        public Task UpdateServiceAsync(int serviceId, PostServiceRequest request);

        public Task DeleteServiceAsync(int serviceId);

        public Task<List<ServiceResponse>> GetListServicesAsync();

        public Task<ServiceResponse> GetServiceById(int id);

        public Task<List<ServiceResponse>> GetServicesByCategoryId(int categoryId);
    }
}
