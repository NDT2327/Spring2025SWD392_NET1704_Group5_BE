using AutoMapper;
using CCSystem.Infrastructure.DTOs.Category;
using CCSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        // Map từ Category sang CategoryResponse
        CreateMap<Category, CategoryResponse>();

        // Map từ CategoryRequest sang Category
        CreateMap<CategoryRequest, Category>()
            .ForMember(dest => dest.CategoryId, opt => opt.Ignore()); // Không map ID vì DB tự tạo
        CreateMap<Category, CategoryResponse>()
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Image));

    }
}