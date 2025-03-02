using AutoMapper;
using CCSystem.BLL.Constants;
using CCSystem.BLL.DTOs.Services;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.BLL.Utils;
using CCSystem.DAL.Infrastructures;
using CCSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Services.Implementations
{
    public class ServiceHomeService : IServiceHomeService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;

        public ServiceHomeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public async Task CreateServiceAsync(PostServiceRequest request)
        {
            string folderName = "service_images";
            bool isUpload = false;
            string imageUrl = "";
            string tempFilePath = "";
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(request.CategoryId);
                if (category == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistCategoryId);
                }

                if (request.Image == null || request.Image.Length == 0)
                {
                    throw new InvalidOperationException(MessageConstant.CommonMessage.NotExistFile);
                }
                tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName));
                // Lưu file tạm thời
                await using (var stream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await request.Image.CopyToAsync(stream);
                }
                imageUrl = await _unitOfWork.FirebaseStorageRepository.UploadImageToFirebase(tempFilePath, folderName);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    isUpload = true;
                }

                CCSystem.DAL.Models.Service service = new DAL.Models.Service()
                {
                    CategoryId = category.CategoryId,
                    ServiceName = request.ServiceName,
                    Image = imageUrl,
                    Description = request.Description,
                    Duration = request.Duration,
                    Price = request.Price,
                    IsActive = request.IsActive ?? true,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                };

                await _unitOfWork.ServiceRepository.CreateServiceAsync(service);
                await _unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string message = ErrorUtil.GetErrorString("BadRequestException", ex.Message);
                throw new BadRequestException(message);
            }
            catch (NotFoundException ex)
            {
                string message = ErrorUtil.GetErrorString("Failed to create service", ex.Message);
                throw new NotFoundException(message);
            }
            catch (Exception ex)
            {
                if (isUpload)
                {
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageFromFirebase(imageUrl);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
            finally
            {
                await FileUtils.SafeDeleteFileAsync(tempFilePath);
            }
        }

        public async Task<List<ServiceResponse>> GetListServicesAsync()
        {
            try
            {
                var services = await _unitOfWork.ServiceRepository.GetListServicesAsync();
                return _mapper.Map<List<ServiceResponse>>(services);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ServiceResponse> GetServiceById(int id)
        {
            try
            {
                var service = await _unitOfWork.ServiceRepository.GetServiceAsync(id);
                if (service == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistServiceId);
                }
                return _mapper.Map<ServiceResponse>(service);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ServiceResponse>> SearchServiceAsync(SearchServiceRequest request)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetCategoryByName(request.CategoryName);
                //if (category == null)
                //{
                //    throw new NotFoundException(MessageConstant.CommonMessage.NotExistCategoryName);
                //}
                // Sửa lỗi truyền categoryId null an toàn
                var categoryId = category?.CategoryId;

                var services = await _unitOfWork.ServiceRepository.SearchServiceAsync(
                    request.ServiceName,
                    request.Description,
                    request.Price,
                    request.Duration,
                    request.IsActive,
                    categoryId);

                var servicesReponse = _mapper.Map<List<ServiceResponse>>(services);
                return servicesReponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
