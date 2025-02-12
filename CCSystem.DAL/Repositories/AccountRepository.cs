using CCSystem.DAL.DBContext;
using CCSystem.DAL.Enums;
using CCSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.DAL.Repositories
{
    public class AccountRepository
    {
        private SP25_SWD392_CozyCareContext _context;

        public AccountRepository(SP25_SWD392_CozyCareContext context)
        {
            this._context = context;
        }

        public async Task CreateAccountAsync(Account account)
        {
            try
            {
                await this._context.Accounts.AddAsync(account);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Account> GetAccountByEmailAsync(string email)
        {
            try
            {
                return await _context.Accounts
                    .SingleOrDefaultAsync(r => r.Email.Equals(email) && r.Status != AccountEnums.Status.INACTIVE.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Account> GetActiveAccountAsync(string email)
        {
            try
            {
                return await this._context.Accounts
                    .SingleOrDefaultAsync(x => x.Email.Equals(email) && x.Status == AccountEnums.Status.ACTIVE.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Account> GetAccountAsync(int accountId)
        {
            try
            {
                return await this._context.Accounts
                    .SingleOrDefaultAsync(x => x.AccountId == accountId && x.Status !=AccountEnums.Status.INACTIVE.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Account> GetAccountAsync(string email)
        {
            try
            {
                return await this._context.Accounts
                    .SingleOrDefaultAsync(x => x.Email == email && x.Status != AccountEnums.Status.INACTIVE.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateAccount(Account account)
        {
            try
            {
                this._context.Accounts.Update(account);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
