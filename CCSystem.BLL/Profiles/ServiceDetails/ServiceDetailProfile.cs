using AutoMapper;
using CCSystem.DAL.Models;
using CCSystem.BLL.DTOs.ServiceDetails;

public class ServiceDetailProfile : Profile
{
    public ServiceDetailProfile()
    {
        CreateMap<ServiceDetail, GetServiceDetailResponse>();
        CreateMap<PostServiceDetailRequest, ServiceDetail>();
        CreateMap<ServiceDetail, PostServiceDetailResponse>();
    }
}