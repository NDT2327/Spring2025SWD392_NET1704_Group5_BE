using CCSystem.DAL.DBContext;
using CCSystem.DAL.Redis.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Redis.OM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.DAL.Infrastructures
{
    public class DbFactory : Disposable, IDbFactory
    {
        private SP25_SWD392_CozyCareContext _dbContext;

        private RedisConnectionProvider _redisConnectionProvider;

        public DbFactory() { }

        public SP25_SWD392_CozyCareContext InitDbContext()
        {
            if (_dbContext == null)
            {
                _dbContext = new SP25_SWD392_CozyCareContext();
            }
            return _dbContext;
        }

        public async Task<RedisConnectionProvider> InitRedisConnectionProvider()
        {
            if (this._redisConnectionProvider == null)
            {
                var builder = new ConfigurationBuilder()
                                  .SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                IConfigurationRoot configuration = builder.Build();
                this._redisConnectionProvider = new RedisConnectionProvider(configuration.GetConnectionString("RedisDbStore"));
                await this._redisConnectionProvider.Connection.CreateIndexAsync(typeof(AccountToken));
                await this._redisConnectionProvider.Connection.CreateIndexAsync(typeof(EmailVerification));
            }
            return this._redisConnectionProvider;
        }


        protected override void DisposeCore()
        {
            if (this._dbContext != null)
            {
                this._dbContext.Dispose();
            }
        }
    }
}
