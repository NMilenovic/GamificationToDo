using System.Data.Common;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace Services
{
public class RedisService
{
    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost,abortConnect=false");
    
    public IDatabase db;
    public RedisService()
    {
       db = redis.GetDatabase();
    }
}
}
