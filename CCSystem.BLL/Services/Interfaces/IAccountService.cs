﻿using CCSystem.Infrastructure.DTOs.Accounts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<bool> IsActiveAccountAsync(string email);
        public Task<GetAccountResponse> GetAccountAsync(int idAccount, IEnumerable<Claim> claims);
        public Task<List<GetAccountResponse>> SearchAccountsAsync(AccountSearchRequest searchRequest);
        public Task<List<GetAccountResponse>>GetAccountsListAsync(AccountsListRequest accountsListRequest);
        public Task<GetAccountResponse> GetAccountByIdAsync(int idAccount);
        public Task UpdateAccountAsync(int accountId, UpdateAccountRequest request);
        //public Task UpdateAccountAsync(int idAccount, UpdateAccountRequest updateAccountRequest, IEnumerable<Claim> claims);
        Task<bool> LockAccount(int idAccount);
        Task<bool> UnlockAccount(int idAccount);

    }
}
