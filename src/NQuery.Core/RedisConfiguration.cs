namespace NQuery.Core;

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
    public bool AbortOnConnectFail { get; set; }
}

public class RedisEndpoint
{
    public string Host { get; set; } = null!;
    public int Port { get; set; }
}