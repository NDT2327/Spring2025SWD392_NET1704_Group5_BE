
using Microsoft.EntityFrameworkCore;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.BLL.DTOs.Category;
using CCSystem.DAL.Models;
using System;
using CCSystem.DAL.DBContext;

using AutoMapper;
using CCSystem.DAL.Infrastructures;
using System.Collections.Generic;
using System.Threading.Tasks;
using CCSystem.BLL.Exceptions;
using CCSystem.DAL.Repositories;
using CCSystem.BLL.Utils;
using CCSystem.BLL.Constants;

namespace CCSystem.BLL.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllCategoriesAsync();

            var response = categories.Select(category => new CategoryResponse
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                ImageUrl = category.Image,  // 🔴 Sửa ở đây!  
                IsActive = category.IsActive ?? false,
                CreatedDate = category.CreatedDate ?? DateTime.UtcNow,
                UpdatedDate = category.UpdatedDate ?? DateTime.UtcNow
            });

            return response;
        }

        public async Task<CategoryResponse?> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return null;
            }

            // Ánh xạ thủ công, đảm bảo Image được gán đúng
            var response = new CategoryResponse
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                ImageUrl = category.Image, // Gán đúng ảnh
                IsActive = category.IsActive ?? false,
                CreatedDate = category.CreatedDate ?? DateTime.UtcNow,
                UpdatedDate = category.UpdatedDate ?? DateTime.UtcNow
            };

            return response;
        }
        public async Task CreateCategoryAsync(CategoryRequest request)
        {
            string folderName = "category_images";
            bool isUpload = false;
            string imageUrl = "";
            string tempFilePath = "";

            try
            {
                if (request.Image == null || request.Image.Length == 0)
                {
                    throw new InvalidOperationException(MessageConstant.CommonMessage.NotExistFile);
                }

                // Tạo đường dẫn file tạm
                tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName));

                // Lưu file tạm thời
                await using (var stream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await request.Image.CopyToAsync(stream);
                }

                // Tải lên Firebase
                imageUrl = await _unitOfWork.FirebaseStorageRepository.UploadImageToFirebase(tempFilePath, folderName);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    isUpload = true;
                }

                // Tạo đối tượng Category
                var category = new Category
                {
                    CategoryName = request.CategoryName,
                    Description = request.Description,
                    IsActive = request.IsActive,
                    Image = imageUrl
                };

                await _unitOfWork.CategoryRepository.CreateCategoryAsync(category);
                await _unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string message = ErrorUtil.GetErrorString("BadRequestException", ex.Message);
                throw new BadRequestException(message);
            }
            catch (Exception ex)
            {
                if (isUpload && !string.IsNullOrEmpty(imageUrl))
                {
                    await _unitOfWork.FirebaseStorageRepository.DeleteImageFromFirebase(imageUrl);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
            finally
            {
                await FileUtils.SafeDeleteFileAsync(tempFilePath);
            }
        }


        public async Task UpdateCategoryAsync(int id, CategoryRequest request)
        {
            var category = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(id);
            if (category == null) throw new KeyNotFoundException("Category not found");

            string folderName = "category_images";
            bool isUpload = false;
            string imageUrl = category.Image; // Giữ nguyên ảnh cũ nếu không có ảnh mới
            string tempFilePath = "";

            try
            {
                // Nếu có ảnh mới, upload ảnh lên Firebase
                if (request.Image != null && request.Image.Length > 0)
                {
                    tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName));

                    await using (var stream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await request.Image.CopyToAsync(stream);
                    }

                    imageUrl = await _unitOfWork.FirebaseStorageRepository.UploadImageToFirebase(tempFilePath, folderName);
                    isUpload = true;

                    // Xóa ảnh cũ nếu có ảnh mới
                    if (!string.IsNullOrEmpty(category.Image) && Uri.IsWellFormedUriString(category.Image, UriKind.Absolute))
                    {
                        await _unitOfWork.FirebaseStorageRepository.DeleteImageFromFirebase(category.Image);
                    }
                }

                    // Cập nhật thông tin category
                    category.CategoryName = request.CategoryName;
                category.Description = request.Description;
                category.IsActive = request.IsActive;
                category.Image = imageUrl; // Cập nhật ảnh mới (hoặc giữ ảnh cũ nếu không có ảnh mới)

                _unitOfWork.CategoryRepository.UpdateCategory(category);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                if (isUpload)
                {
                    await _unitOfWork.FirebaseStorageRepository.DeleteImageFromFirebase(imageUrl);
                }
                throw new Exception("Failed to update category: " + ex.Message);
            }
            finally
            {
                await FileUtils.SafeDeleteFileAsync(tempFilePath);
            }
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
