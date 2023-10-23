namespace NQuery;

public class NQueryConfiguration
{
    public bool UseInMemory { get; set; }  
    public int CacheDuration { get; set; } = 5;
    public RedisConfiguration? RedisConfiguration { get; set; }
}

public class RedisConfiguration
{
    public RedisConfiguration(string host, int port = 6379)
    {
        Endpoint = $"{host}:{port}";
    }

    public RedisConfiguration(RedisEndpoint[] endpoints)
    { 
       Endpoint = string.Join(",", endpoints.Select(e => $"{e.Host}:{e.Port}"));
    }

    internal string Endpoint { get; } 
}

public class RedisEndpoint
{
    public string Host { get; set; } = null!;
    public int Port { get; set; }
}