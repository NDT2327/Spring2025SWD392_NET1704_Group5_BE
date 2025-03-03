using AutoMapper;
using CCSystem.BLL.DTOs.Review;
using CCSystem.DAL.Models;

namespace CCSystem.BLL.Profiles
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<ReviewRequest, Review>();
            CreateMap<Review, ReviewResponse>();
        }
    }
}
