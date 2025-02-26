
using Microsoft.EntityFrameworkCore;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.BLL.DTOs.Category;
using CCSystem.DAL.Models;
using System;
using CCSystem.DAL.DBContext;

using AutoMapper;
using CCSystem.BLL.DTOs.Category;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.DAL.Infrastructures;
using CCSystem.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using CCSystem.BLL.Exceptions;
using CCSystem.DAL.Repositories;

namespace CCSystem.BLL.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(UnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllCategoriesAsync();
            return _mapper.Map<IEnumerable<CategoryResponse>>(categories);

        }

        public async Task<CategoryResponse?> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(id);
            return category == null ? null : _mapper.Map<CategoryResponse>(category);
        }

        public async Task<CategoryResponse> CreateCategoryAsync(CategoryRequest request)
        {
            var category = _mapper.Map<Category>(request);

            await _unitOfWork.CategoryRepository.CreateCategoryAsync(category);
            await _unitOfWork.CommitAsync();

            // 💡 Tải lại category từ DB để đảm bảo có ID đúng
            var savedCategory = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(category.CategoryId);

            return _mapper.Map<CategoryResponse>(savedCategory);
        }

        public async Task UpdateCategoryAsync(int id, CategoryRequest request)
        {
            var category = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(id);
            if (category == null) throw new KeyNotFoundException("Category not found");

            _mapper.Map(request, category);
            _unitOfWork.CategoryRepository.UpdateCategory(category); 
            await _unitOfWork.CommitAsync(); 
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                throw new NotFoundException("Category not found.");
            }

            // Chuyển trạng thái sang INACTIVE
            category.IsActive = false;
            _unitOfWork.CategoryRepository.UpdateCategory(category);
            await _unitOfWork.CommitAsync();
        }

        public async Task<List<CategoryResponse>> SearchCategoryAsync(string categoryName, bool? isActive)
        {
            try
            {
                var categories = await _unitOfWork.CategoryRepository.SearchCategoryAsync(categoryName, isActive);

                // Map the Category entities to CategoryResponse DTOs
                var categoryResponses = _mapper.Map<List<CategoryResponse>>(categories);

                return categoryResponses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
