namespace NQuery.Core;

public class NQueryConfiguration
{
    public bool UseInMemory { get; set; }  
    public int CacheDuration { get; set; } = 5;
    public RedisConfiguration? RedisConfiguration { get; set; }
}