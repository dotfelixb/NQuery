namespace NQuery;

public class QueryConfiguration
{
    public bool UseInMemory { get; init; } = false;
    public int CacheDuration { get; init; } = 5000;
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
    public string Host { get; set; }
    public int Port { get; set; }
}