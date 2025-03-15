using CCSystem.DAL.DBContext;
using Redis.OM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.Infrastructure.Infrastructures
{
    public interface IDbFactory : IDisposable
    {
        public SP25_SWD392_CozyCareContext InitDbContext();
        public Task<RedisConnectionProvider> InitRedisConnectionProvider();
    }
}
