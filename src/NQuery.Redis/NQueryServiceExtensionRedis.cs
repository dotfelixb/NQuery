using NQuery.Core;

namespace NQuery.Redis;

public static class NQueryServiceExtensionRedis
{
    public static NQueryConfiguration UseRedis(
        this NQueryConfiguration configuration,
        Action<RedisConfiguration> action)
    {
        var redisConfiguration = new RedisConfiguration();
        action.Invoke(redisConfiguration);
        
        configuration.RedisConfiguration = redisConfiguration;
        return configuration;
    }
}