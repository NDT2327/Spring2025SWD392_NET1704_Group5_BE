using AutoMapper;
using CCSystem.BLL.Constants;
using CCSystem.BLL.DTOs.ScheduleAssign;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.DAL.Infrastructures;
using CCSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Services.Implementations
{
    public class ScheduleAssignService : IScheduleAssignService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;

        public ScheduleAssignService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public Task AddAsync(ScheduleAssignment assignment)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ScheduleAssignmentResponse>> GetAllAsync()
        {
            try
            {
                var assigns = await _unitOfWork.ScheduleAssignRepository.GetAllAsync();
                var responses = _mapper.Map<List<ScheduleAssignmentResponse>>(assigns);
                return responses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ScheduleAssignmentResponse> GetByIdAsync(int id)
        {
            try
            {
                var assign = await _unitOfWork.ScheduleAssignRepository.GetByIdAsync(id);
                if(assign == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistAssignId);
                }
                var reponse = _mapper.Map<ScheduleAssignmentResponse>(assign);
                return reponse;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task UpdateAsync(ScheduleAssignment assignment)
        {
            throw new NotImplementedException();
        }
    }
}
