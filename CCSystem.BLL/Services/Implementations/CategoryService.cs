
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
                ImageUrl = category.Image,   
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

            var response = new CategoryResponse
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                ImageUrl = category.Image,
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
                // Validate if an image file is provided
                if (request.Image == null || request.Image.Length == 0)
                {
                    throw new InvalidOperationException(MessageConstant.CommonMessage.NotExistFile);
                }

                // Generate a temporary file path
                tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName));

                // Save the uploaded file to the temporary location
                await using (var stream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await request.Image.CopyToAsync(stream);
                }

                // Upload image to Firebase Storage
                imageUrl = await _unitOfWork.FirebaseStorageRepository.UploadImageToFirebase(tempFilePath, folderName);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    isUpload = true;
                }

                // Create a new Category object
                var category = new Category
                {
                    CategoryName = request.CategoryName,
                    Description = request.Description,
                    IsActive = request.IsActive,
                    Image = imageUrl
                };
                // Save the new category to the database
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
                // Delete the uploaded image if an error occurs
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
            // Retrieve the category to be updated
                var category = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(id);
            if (category == null) throw new KeyNotFoundException(MessageConstant.CommonMessage.NotExistCategoryId);

            string folderName = "category_images";
            bool isUpload = false;
            string imageUrl = category.Image; // Retain the old image if no new image is uploaded
            string tempFilePath = "";

            try
            {
                // If a new image is provided, upload it
                if (request.Image != null && request.Image.Length > 0)
                {
                    tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName));

                    await using (var stream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await request.Image.CopyToAsync(stream);
                    }

                    imageUrl = await _unitOfWork.FirebaseStorageRepository.UploadImageToFirebase(tempFilePath, folderName);
                    isUpload = true;

                    // Delete the old image from Firebase if a new one is uploaded
                    if (!string.IsNullOrEmpty(category.Image) && Uri.IsWellFormedUriString(category.Image, UriKind.Absolute))
                    {
                        await _unitOfWork.FirebaseStorageRepository.DeleteImageFromFirebase(category.Image);
                    }
                }

                // Update category details
                category.CategoryName = request.CategoryName;
                category.Description = request.Description;
                category.IsActive = request.IsActive;
                category.Image = imageUrl; 

                _unitOfWork.CategoryRepository.UpdateCategory(category);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                // If an error occurs, delete the newly uploaded image
                if (isUpload)
                {
                    await _unitOfWork.FirebaseStorageRepository.DeleteImageFromFirebase(imageUrl);
                }
                throw new Exception(MessageConstant.CategoryMessage.UpdateCategoryFailed + ": " + ex.Message);
            }
            finally
            {
                // Safely delete the temporary file
                await FileUtils.SafeDeleteFileAsync(tempFilePath);
            }
        }

        public async Task DeleteCategoryAsync(int id)
        {
            // Retrieve category by ID
            var category = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                throw new NotFoundException(MessageConstant.CommonMessage.NotExistCategoryId);
            }

            // Mark the category as INACTIVE instead of permanently deleting it
            category.IsActive = false;
            _unitOfWork.CategoryRepository.UpdateCategory(category);
            await _unitOfWork.CommitAsync();
        }

        public async Task<List<CategoryResponse>> SearchCategoryAsync(string categoryName, bool? isActive)
        {
            try
            {
                // Search for categories based on name and active status
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
