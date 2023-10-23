namespace NQuery;

public class NQueryConfiguration
{
    public bool UseInMemory { get; set; }  
    public int CacheDuration { get; set; } = 5;
    public RedisConfiguration? RedisConfiguration { get; set; }
}

public class RedisConfiguration
{

    public RedisConfiguration() : this(Array.Empty<RedisEndpoint>())
    {
    }
    
    public RedisConfiguration(RedisEndpoint[] endpoints)
    {
        foreach (var endpoint in endpoints)
        {
            Endpoints.Add(endpoint);
        }
    }
    
    public IList<RedisEndpoint> Endpoints { get; } = new List<RedisEndpoint>();
}

public class RedisEndpoint
{
    public string Host { get; set; } = null!;
    public int Port { get; set; }
}