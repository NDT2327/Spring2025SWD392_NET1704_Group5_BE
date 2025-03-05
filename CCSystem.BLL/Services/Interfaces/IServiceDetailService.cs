using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCSystem.BLL.DTOs.ServiceDetails;

namespace CCSystem.BLL.Services.Interfaces
{
    public interface IServiceDetailService
    {
        //public Task<List<GetServiceDetailResponse>> GetAllServiceDetailsAsync();
        public Task<GetServiceDetailResponse> GetServiceDetailByIdAsync(int id);
        public Task<PostServiceDetailResponse> CreateServiceDetailAsync(PostServiceDetailRequest request);
        public Task<bool> UpdateServiceDetailAsync(int serviceDetailID, PutServiceDetailRequest request);
        public Task<bool> DeleteServiceDetailAsync(int id);
        Task<List<GetServiceDetailResponse>> GetServiceDetailsByServiceIdAsync(int serviceId);
    }
}
