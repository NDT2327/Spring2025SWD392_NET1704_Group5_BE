using CCSystem.DAL.DBContext;
using CCSystem.DAL.FirebaseStorages.Repositories;
using CCSystem.DAL.Redis.Repositories;
using CCSystem.DAL.Repositories;
using CCSystem.DAL.SMTPs.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
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
        private CategoryRepository _categoryRepository;
        private RedisConnectionProvider _redisConnectionProvider;
        private AccountTokenRedisRepository _accountTokenRedisRepository;
        private EmailVerificationRedisRepository _emailVerificationRedisRepository;
        private EmailRepository _emailRepository;
        private FirebaseStorageRepository _firebaseStorageRepository;
        private ServiceRepository _serviceRepository;
        private PaymentRepository _paymentRepository;
        private BookingRepository _bookingRepository;
        private ReportRepository _reportRepository;
        private ReviewRepository _reviewRepository;
        private BookingDetailRepository _bookingDetailRepository;
        private ServiceDetailRepository _serviceDetailRepository;
        private PromotionRepository _promotionRepository;



        public UnitOfWork(IDbFactory dbFactory)
        {
            this._dbFactory = dbFactory;
            if (this._dbContext == null)
            {
                this._dbContext = dbFactory.InitDbContext();
            }
        }
        public ReviewRepository ReviewRepository
        {
            get
            {
                if (this._reviewRepository == null)
                {
                    this._reviewRepository = new ReviewRepository(this._dbContext);
                }
                return this._reviewRepository;
            }
        }


        public ReportRepository ReportRepository
        {
            get
            {
                if (this._reportRepository == null)
                {
                    this._reportRepository = new ReportRepository(this._dbContext);
                }
                return this._reportRepository;
            }
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _dbContext.Database.BeginTransactionAsync();
        }


        public BookingDetailRepository BookingDetailRepository
        {
            get
            {
                if (this._bookingDetailRepository == null)
                {
                    this._bookingDetailRepository = new BookingDetailRepository(this._dbContext);
                }
                return this._bookingDetailRepository;
            }
        }


        public BookingRepository BookingRepository
        {
            get
            {
                if (this._bookingRepository == null)
                {
                    this._bookingRepository = new BookingRepository(this._dbContext);
                }
                return this._bookingRepository;
            }
        }

        public PaymentRepository PaymentRepository
        {
            get
            {
                if (this._paymentRepository == null)
                {
                    this._paymentRepository = new PaymentRepository(this._dbContext);
                }
                return this._paymentRepository;
            }
        }

        public ServiceRepository ServiceRepository
        {
            get
            {
                if (this._serviceRepository == null)
                {
                    this._serviceRepository = new ServiceRepository(this._dbContext);
                }
                return this._serviceRepository;
            }
        }

        public CategoryRepository CategoryRepository
        {
            get
            {
                if (this._categoryRepository == null)
                {
                    this._categoryRepository = new CategoryRepository(this._dbContext);
                }
                return this._categoryRepository;
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

        #region ServiceDetailRepository
        public ServiceDetailRepository ServiceDetailRepository
        {
            get
            {
                if (this._serviceDetailRepository == null)
                {
                    this._serviceDetailRepository = new ServiceDetailRepository(this._dbContext);
                }
                return this._serviceDetailRepository;
            }
        }
        #endregion

        #region PromotionRepository
        public PromotionRepository PromotionRepository
        {
            get
            {
                if (this._promotionRepository == null)
                {
                    this._promotionRepository = new PromotionRepository(this._dbContext);
                }
                return this._promotionRepository;
            }
        }
        #endregion

        public void Commit()
        {
            this._dbContext.SaveChanges();
        }


        public async Task CommitAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔥 Error: {ex.InnerException?.Message ?? ex.Message}");
                throw;
            }
        }
    }
}

