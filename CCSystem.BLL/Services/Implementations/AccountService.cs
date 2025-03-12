using AutoMapper;
using CCSystem.BLL.Constants;
using CCSystem.BLL.DTOs.Accounts;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.BLL.Utils;
using CCSystem.DAL.Enums;
using CCSystem.DAL.Infrastructures;
using CCSystem.DAL.Models;
using CCSystem.DAL.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Services.Implementations
{
    public class AccountService : IAccountService
    {

        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }
        // Fetch accounts based on search criteria
        public async Task<List<GetAccountResponse>> SearchAccountsAsync(AccountSearchRequest searchRequest)
        {
            var accounts = await _unitOfWork.AccountRepository.SearchAccountsAsync(
            searchRequest.AccountId,
            searchRequest.Email,
            searchRequest.Role,
            searchRequest.Address,
            searchRequest.Phone,
            searchRequest.FullName,
            searchRequest.Status,
            searchRequest.MinCreatedDate,
            searchRequest.MaxCreatedDate);

            // Convert entity list to DTO list
            return _mapper.Map<List<GetAccountResponse>>(accounts);
        }

        public async Task<bool> IsActiveAccountAsync(string email)
        {
            try
            {
                // Check if an active account exists for the given email
                Account existedAccount = await this._unitOfWork.AccountRepository.GetActiveAccountAsync(email);
                if (existedAccount == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task<GetAccountResponse> GetAccountAsync(int idAccount, IEnumerable<Claim> claims)
        {
            try
            {
                // Retrieve email from claims
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;

                // Fetch account by ID
                Account existedAccount = await this._unitOfWork.AccountRepository.GetAccountAsync(idAccount);

                if (existedAccount is null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistAccountId);
                }
                // Ensure the requested account belongs to the authenticated user

                if (existedAccount.Email.Equals(email) == false)
                {
                    throw new BadRequestException(MessageConstant.AccountMessage.AccountIdNotBelongYourAccount);
                }
                GetAccountResponse getAccountResponse = this._mapper.Map<GetAccountResponse>(existedAccount);
                return getAccountResponse;
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Account id", ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString("Account id", ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
        public async Task<bool> LockAccount(int accountId)
        {
            // Fetch account by ID
            var account = await _unitOfWork.AccountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                throw new NotFoundException(MessageConstant.CommonMessage.NotExistAccountId);
            }

            if (account.Status == nameof(AccountEnums.Status.INACTIVE))
            {
                return false; 
            }

            account.Status = nameof(AccountEnums.Status.INACTIVE);// Change status to INACTIVE
            await _unitOfWork.AccountRepository.UpdateAsync(account);
            await _unitOfWork.CommitAsync();

            return true;
        }
        public async Task<bool> UnlockAccount(int accountId)
        {
            var account = await _unitOfWork.AccountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                throw new NotFoundException(MessageConstant.CommonMessage.NotExistAccountId);
            }
            if (account.Status == nameof(AccountEnums.Status.ACTIVE))
            {
                return false; 
            }
            account.Status = nameof(AccountEnums.Status.ACTIVE);// Change status to ACTIVE
            await _unitOfWork.AccountRepository.UpdateAsync(account);
            await _unitOfWork.CommitAsync();
            return true;

        }

        public async Task<List<GetAccountResponse>> GetAccountsListAsync(AccountsListRequest accountsListRequest)
        {
            try
            {
                // Validate input
                if (accountsListRequest is null)
                {
                    throw new BadRequestException("Invalid request data.");
                }

                // Fetch accounts list from repository
                var accounts = await _unitOfWork.AccountRepository.GetAccountsListAsync(
                    accountsListRequest.PageIndex,
                    accountsListRequest.PageSize,
                    accountsListRequest.SearchByName,
                    accountsListRequest.Sort);

                // Return mapped account list
                return _mapper.Map<List<GetAccountResponse>>(accounts);
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Accounts List", ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString("Invalid Request", ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }


        public async Task<GetAccountResponse> GetAccountByIdAsync(int idAccount)
        {
            // Retrieve account details by ID
            Account existedAccount = await this._unitOfWork.AccountRepository.GetAccountAsync(idAccount);
            if (existedAccount is null)
            {
                throw new NotFoundException(MessageConstant.CommonMessage.NotExistAccountId);
            }
            GetAccountResponse getAccountResponse = this._mapper.Map<GetAccountResponse>(existedAccount);
            return getAccountResponse;
        }

        public async Task UpdateAccountAsync(int accountId, UpdateAccountRequest request)
        {
            var account = await _unitOfWork.AccountRepository.GetAccountAsync(accountId);
            if (account == null)
            {
                throw new NotFoundException(MessageConstant.CommonMessage.NotExistAccountId);
            }
            // Process avatar upload if provided
            if (request.Avatar != null && request.Avatar.Length > 0)
            {
                string folderName = "account_avatars";
                string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(request.Avatar.FileName));

                await using (var stream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await request.Avatar.CopyToAsync(stream);
                }
                string imageUrl = await _unitOfWork.FirebaseStorageRepository.UploadImageToFirebase(tempFilePath, folderName);

                
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    // Delete old avatar from Firebase if it exists
                    if (!string.IsNullOrEmpty(account.Avatar) && Uri.IsWellFormedUriString(account.Avatar, UriKind.Absolute))
                    {
                        await _unitOfWork.FirebaseStorageRepository.DeleteImageFromFirebase(account.Avatar);
                    }

                    account.Avatar = imageUrl;
                }

                await FileUtils.SafeDeleteFileAsync(tempFilePath);

                // Validate and update Date of Birth
                if (request.Year.HasValue && request.Month.HasValue && request.Day.HasValue)
                {
                    try
                    {
                        account.DateOfBirth = new DateOnly(request.Year.Value, request.Month.Value, request.Day.Value);
                    }
                    catch
                    {
                        throw new BadRequestException(MessageConstant.AccountMessage.InvalidDateOfBirth);
                    }
                }


                // Update other account details
                account.FullName = request.FullName;
                account.Phone = request.Phone;
                account.Address = request.Address;
                account.Gender = request.Gender;
                account.Rating = request.Rating;
                account.Experience = request.Experience;
                account.Status = request.Status;
                account.UpdatedDate = DateTime.UtcNow;  // Set last update timestamp


                await _unitOfWork.AccountRepository.UpdateAccount(account);
                await _unitOfWork.CommitAsync();
            }


        }
    }
}