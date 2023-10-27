namespace NQuery.Core;

public class NQueryConfiguration
{
    public bool UseInMemory { get; set; }  
    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(5);
    public RedisConfiguration? RedisConfiguration { get; set; }
}