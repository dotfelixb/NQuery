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
    public string? Password { get; set; }
    public bool AbortOnConnectFail { get; set; }
    public bool ResolveDns { get; set; }
    public bool UseSsl { get; set; }
    public int ConnectTimeout { get; set; } = 5000;
}

public class RedisEndpoint
{
    public string Host { get; set; } = null!;
    public int Port { get; set; }
}