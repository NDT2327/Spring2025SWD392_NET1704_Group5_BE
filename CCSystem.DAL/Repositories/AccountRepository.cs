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

        public async Task<List<Account>> SearchAccountsAsync(
        int? accountId,
        string email,
        string role,
        string address,
        string phone,
        string fullName,
        string status,
        DateTime? minCreatedDate,
        DateTime? maxCreatedDate)
        {
            var query = _context.Accounts.AsQueryable();

            if (accountId.HasValue)
                query = query.Where(a => a.AccountId == accountId.Value);

            if (!string.IsNullOrEmpty(email))
                query = query.Where(a => a.Email.Contains(email));

            if (!string.IsNullOrEmpty(role))
                query = query.Where(a => a.Role.Equals(role));

            if (!string.IsNullOrEmpty(address))
                query = query.Where(a => a.Address.Contains(address));

            if (!string.IsNullOrEmpty(phone))
                query = query.Where(a => a.Phone.Contains(phone));

            if (!string.IsNullOrEmpty(fullName))
                query = query.Where(a => a.FullName.Contains(fullName));

            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status.Equals(status));

            if (minCreatedDate.HasValue)
                query = query.Where(a => a.CreatedDate >= minCreatedDate.Value);

            if (maxCreatedDate.HasValue)
                query = query.Where(a => a.CreatedDate <= maxCreatedDate.Value);

            // Sắp xếp giảm dần theo ngày tạo
            query = query.OrderByDescending(a => a.CreatedDate);

            return await query.ToListAsync();
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
        public async Task<Account> GetByIdAsync(int accountId)
        {
            try
            {
                return await _context.Accounts
                    .SingleOrDefaultAsync(a => a.AccountId == accountId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAsync(Account account)
        {
            try
            {
                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
