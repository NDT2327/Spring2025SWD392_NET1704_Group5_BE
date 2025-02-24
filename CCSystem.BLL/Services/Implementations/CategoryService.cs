
using Microsoft.EntityFrameworkCore;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.DAL.Models;
using System;
using CCSystem.DAL.DBContext;

using AutoMapper;
using CCSystem.DAL.Infrastructures;

namespace CCSystem.BLL.Service
{
    public class CategoryService
    {

        //private readonly SP25_SWD392_CozyCareContext _context;
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {

            return await _unitOfWork.CategoryRepository.GetAllCategoriesAsync();

        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(id);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            await _unitOfWork.CategoryRepository.CreateCategoryAsync(category);
            await _unitOfWork.CommitAsync();
            return category;
        }
        public async Task UpdateCategoryAsync(Category category)
        {
            await _unitOfWork.CategoryRepository.UpdateCategoryAsync(category);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {

            var category = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return false;
            }
            await _unitOfWork.CategoryRepository.DeleteCategoryAsync(id);

            return true;
        }
    }
}
