using Microsoft.Extensions.Logging;

namespace Bootstrap.Components.Cache.Redis
{
    public class CacheRedisClient : RedisClient
    {
        public CacheRedisClient(string connectionString, ILoggerFactory loggerFactory) : base(connectionString,
            loggerFactory)
        {
        }
    }
}