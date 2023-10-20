using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json;
using StackExchange.Redis;
using ArgumentNullException = System.ArgumentNullException;

namespace NQuery;

public class Query
{
    private readonly ConcurrentDictionary<string, object> _memoryDb;
    private readonly QueryConfiguration _configuration;
    private readonly IDatabase _redisDb;

    private event EventHandler<QueryEventArgs> OnInserted;

    private Query(QueryConfiguration queryConfiguration)
    {
        _memoryDb = new ConcurrentDictionary<string, object>();
        _configuration = queryConfiguration;
        var queryEvents = new QueryEvents(_memoryDb, _configuration);
        OnInserted += queryEvents.Run;
    }

    private Query(QueryConfiguration queryConfiguration, IConnectionMultiplexer connectionMultiplexer)
    {
        _configuration = queryConfiguration;
        _redisDb = connectionMultiplexer.GetDatabase();
    }

    public static Query Create(QueryConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        // pre-check
        if (configuration is { UseInMemory: false, RedisConfiguration: null })
        {
            throw new ArgumentException("RedisConfiguration required if not using InMemory Query Cache");
        }

        return configuration switch
        {
            { UseInMemory: true, RedisConfiguration: null }
                => new Query(configuration),
            { UseInMemory: false, RedisConfiguration: not null }
                => new Query(configuration, ConnectionMultiplexer.Connect(configuration.RedisConfiguration.Endpoint)),
            _
                => throw new ArgumentException("Can't initialize NQuery")
        };
    }

    public async Task<TOutput?> QueryAsync<TOutput>(string key, Func<Task<TOutput?>> query)
    {
        return _configuration.UseInMemory
            ? await InMemoryQuery(key, query)
            : await ExternalQuery(key, query);
    }
    
    public async Task<IEnumerable<TOutput>?> QueryAsync<TOutput>(string key, Func<Task<IEnumerable<TOutput>>> query)
    {
        return _configuration.UseInMemory
            ? await InMemoryQuery(key, query)
            : await ExternalQuery(key, query);
    }

    private async Task<TOutput> InMemoryQuery<TOutput>(string key, Func<Task<TOutput>> query)
    {
        // check if key is already set
        if (_memoryDb.TryGetValue(key, out var value))
        {
            return (TOutput)value;
        }

        // key not set 
        var rst = await query();
        if (rst is null) return rst;

        _memoryDb[key] = rst;
        OnInserted?.Invoke(this, new QueryEventArgs { Key = key });

        return rst;
    } 

    private async Task<IEnumerable<TOutput>> InMemoryQuery<TOutput>(string key, Func<Task<IEnumerable<TOutput>>> query)
    {
        // check if key is already set
        if (_memoryDb.TryGetValue(key, out var value))
        {
            return (IEnumerable<TOutput>)value;
        }

        // key not set 
        var rst = await query();
        var rstLst = rst.ToList();
        if (rstLst.Count == 0) return rstLst;

        _memoryDb[key] = rst;
        OnInserted?.Invoke(this, new QueryEventArgs { Key = key });

        return rstLst;
    }
    private async Task<TOutput?> ExternalQuery<TOutput>(string key, Func<Task<TOutput>> query)
    {
        // check if key is already set
        var exist = await _redisDb.StringGetAsync(new RedisKey(key));
        if (!exist.IsNull)
        {
            return JsonSerializer.Deserialize<TOutput>(exist!);
        }

        // key not set 
        var rst = await query();
        if (rst is null) return rst;
        
        await _redisDb.StringSetAsync(
            key, 
            JsonSerializer.Serialize(rst), 
            TimeSpan.FromMinutes(_configuration.CacheDuration));
        return rst;
    } 
    
    private async Task<IEnumerable<TOutput>> ExternalQuery<TOutput>(string key, Func<Task<IEnumerable<TOutput>>> query)
    {
        // check if key is already set
        var exist = await _redisDb.StringGetAsync(new RedisKey(key));
        if (!exist.IsNull)
        {
            return JsonSerializer.Deserialize<IEnumerable<TOutput>>(exist);
        }

        // key not set 
        var rst = await query();
        var rstLst = rst.ToList();
        if(rstLst.Count == 0) return rstLst;
        
        await _redisDb.StringSetAsync(
            key, 
            JsonSerializer.Serialize(rstLst), 
            TimeSpan.FromMinutes(_configuration.CacheDuration));
        return rstLst;
    }
}