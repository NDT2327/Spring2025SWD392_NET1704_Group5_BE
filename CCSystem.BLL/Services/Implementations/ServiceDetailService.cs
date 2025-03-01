﻿using AutoMapper;
using CCSystem.BLL.DTOs.ServiceDetails;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.DAL.Infrastructures;
using CCSystem.DAL.Models;
using CCSystem.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Services.Implementations
{
    public class ServiceDetailService : IServiceDetailService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;

        public ServiceDetailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }
        public async Task<GetServiceDetailResponse> GetServiceDetailByIdAsync(int id)
        {
            var serviceDetail =await _unitOfWork.ServiceDetailRepository.GetByIdAsync(id);

            if (serviceDetail != null)
            {
                return _mapper.Map<GetServiceDetailResponse>(serviceDetail);
            }
            else
            {
                throw new Exception("Service Detail not found");
            }
        }

        #region CreateServiceDetailAsync
        public async Task<PostServiceDetailResponse> CreateServiceDetailAsync(PostServiceDetailRequest request)
        {
            // Check if the referenced ServiceId exists
            var serviceExists = await _unitOfWork.ServiceRepository.GetServiceAsync(request.ServiceId);
            Console.WriteLine(serviceExists);
            if (serviceExists == null)
            {
                throw new KeyNotFoundException($"Service with ID {request.ServiceId} does not exist.");
            }
            try
            {
                var serviceDetail = _mapper.Map<ServiceDetail>(request);
                await _unitOfWork.ServiceDetailRepository.AddAsync(serviceDetail);
                await _unitOfWork.CommitAsync();

                return _mapper.Map<PostServiceDetailResponse>(serviceDetail);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("FOREIGN KEY") == true)
            {
                throw new InvalidOperationException("Invalid ServiceId. The referenced Service does not exist.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the ServiceDetail." + serviceExists, ex);
            }

        }

        #endregion

        public async Task<bool> UpdateServiceDetailAsync(PutServiceDetailRequest request)
        {
            // Check if ServiceDetail exists
            var serviceDetail = await _unitOfWork.ServiceDetailRepository.GetByIdAsync(request.ServiceDetailId);
            if (serviceDetail == null)
            {
                throw new Exception("ServiceDetail not found.");
            }

            // Check if ServiceId exists (Foreign Key Constraint)
            var service = await _unitOfWork.ServiceRepository.GetServiceAsync(request.ServiceId);
            if (service == null)
            {
                throw new Exception("Invalid ServiceId. No matching service found.");
            }

            // Map DTO to Entity
            serviceDetail.ServiceId = request.ServiceId;
            serviceDetail.OptionName = request.OptionName;
            serviceDetail.OptionType = request.OptionType;
            serviceDetail.BasePrice = request.BasePrice;
            serviceDetail.Unit = request.Unit;
            serviceDetail.Duration = request.Duration;
            serviceDetail.Description = request.Description;
            serviceDetail.IsActive = request.IsActive;

            return await _unitOfWork.ServiceDetailRepository.UpdateAsync(serviceDetail);
        }
    }
}
