﻿using CCSystem.DAL.DBContext;
using CCSystem.DAL.FirebaseStorages.Repositories;
using CCSystem.DAL.Redis.Repositories;
using CCSystem.DAL.Repositories;
using CCSystem.DAL.SMTPs.Repositories;
using Redis.OM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.DAL.Infrastructures
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDbFactory _dbFactory;
        private SP25_SWD392_CozyCareContext _dbContext;
        private AccountRepository _accountRepository;
        private RedisConnectionProvider _redisConnectionProvider;
        private AccountTokenRedisRepository _accountTokenRedisRepository;
        private EmailVerificationRedisRepository _emailVerificationRedisRepository;
        private EmailRepository _emailRepository;
        private FirebaseStorageRepository _firebaseStorageRepository;


        public UnitOfWork(IDbFactory dbFactory)
        {
            this._dbFactory = dbFactory;
            if (this._dbContext == null)
            {
                this._dbContext = dbFactory.InitDbContext();
            }
        }

        public FirebaseStorageRepository FirebaseStorageRepository
        {
            get
            {
                if (this._firebaseStorageRepository == null)
                {
                    this._firebaseStorageRepository = new FirebaseStorageRepository();
                }
                return this._firebaseStorageRepository;
            }
        }

        public AccountRepository AccountRepository
        {
            get
            {
                if (this._accountRepository == null)
                {
                    this._accountRepository = new AccountRepository(this._dbContext);
                }
                return this._accountRepository;
            }
        }

        public EmailRepository EmailRepository
        {
            get
            {
                if (this._emailRepository == null)
                {
                    this._emailRepository = new EmailRepository();
                }
                return this._emailRepository;
            }
        }

        public EmailVerificationRedisRepository EmailVerificationRedisRepository
        {
            get
            {
                if (this._redisConnectionProvider == null)
                {
                    this._redisConnectionProvider = this._dbFactory.InitRedisConnectionProvider().Result;
                }
                if (this._emailVerificationRedisRepository == null)
                {
                    this._emailVerificationRedisRepository = new EmailVerificationRedisRepository(this._redisConnectionProvider);
                }
                return this._emailVerificationRedisRepository;
            }
        }

        public AccountTokenRedisRepository AccountTokenRedisRepository
        {
            get
            {
                if (this._redisConnectionProvider == null)
                {
                    this._redisConnectionProvider = this._dbFactory.InitRedisConnectionProvider().Result;
                }
                if (this._accountTokenRedisRepository == null)
                {
                    this._accountTokenRedisRepository = new AccountTokenRedisRepository(this._redisConnectionProvider);
                }
                return this._accountTokenRedisRepository;
            }
        }

        public void Commit()
        {
            this._dbContext.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await this._dbContext.SaveChangesAsync();
        }
    }
}
