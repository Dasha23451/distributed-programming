using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Valuator.Redis
{
    public class RedisStorage : IRedisStorage
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IConfiguration Configuration;

        public RedisStorage(IConfiguration configuration)
        {
            Configuration = configuration;
            var host = Configuration["RedisValues:HOST_NAME"];
            _connection = ConnectionMultiplexer.Connect(host);
        }

        public void Save(string key, string value)
        {
            var db = _connection.GetDatabase();

            db.StringSet(key, value);
        }

        public string Get(string key)
        {
            var db = _connection.GetDatabase();

            return db.StringGet(key);
        }

        public List<string> GetKeys()
        {
            var host = Configuration["RedisValues:HOST_NAME"];
            var port = Convert.ToInt32(Configuration["RedisValues:HOST_PORT"]);
            var keys = _connection.GetServer(host, port).Keys();

            return keys.Select(item => item.ToString()).ToList();
        }
    }

}

