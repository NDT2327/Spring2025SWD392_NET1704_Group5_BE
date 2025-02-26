using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCSystem.BLL.DTOs.Services;

namespace CCSystem.BLL.Services.Interfaces
{
    public interface IServiceHomeService
    {
        public Task CreateServiceAsync(PostServiceRequest request);

        public Task<List<ServiceResponse>> SearchServiceAsync(SearchServiceRequest request);

        public Task<List<ServiceResponse>> GetListServicesAsync();

        public Task<ServiceResponse> GetServiceById(int id);
    }
}
