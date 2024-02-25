using StackExchange.Redis;

namespace Valuator.Redis
{
    public class RedisStorage : IRedisStorage
    {
        private readonly IConnectionMultiplexer _connection;


        public RedisStorage()
        {
            _connection = ConnectionMultiplexer.Connect(Configs.HOST_NAME + ",abortConnect=false");
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
            var keys = _connection.GetServer(Configs.HOST_NAME, Configs.HOST_PORT).Keys();

            return keys.Select(item => item.ToString()).ToList();
        }
    }

}

