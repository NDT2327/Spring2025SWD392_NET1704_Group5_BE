using Redis.OM.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.DAL.Redis.Models
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "AccountToken" }, IndexName = "account_tokens")]
    public class AccountToken
    {
        [RedisIdField]
        [Indexed]
        public int AccountId { get; set; }
        [Indexed]
        public string JWTId { get; set; }
        [Indexed]
        public string RefreshToken { get; set; }
        [Indexed]
        public DateTime ExpiredDate { get; set; }
    }
}
