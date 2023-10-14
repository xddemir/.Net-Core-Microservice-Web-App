using FreeCourse.Services.Basket.Settings;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace FreeCourse.Services.Basket.Services;

public class RedisService
{
    private readonly string _host;
    private readonly int _port;

    private ConnectionMultiplexer _connectionMultiplexer;
    
    private readonly RedisSettings _redisSettings;

    public RedisService(IOptions<RedisSettings> settings)
    {
        _redisSettings = settings.Value;
    }

    public void Connect() => _connectionMultiplexer = ConnectionMultiplexer.Connect($"{_redisSettings.Host}:{_redisSettings.Port}");

    public IDatabase GetDb(int db = 1) => _connectionMultiplexer.GetDatabase(db);
}