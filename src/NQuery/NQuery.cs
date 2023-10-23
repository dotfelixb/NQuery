using System.Collections.Concurrent;
using System.Text.Json;
using NQuery.Interfaces;
using StackExchange.Redis; 

namespace NQuery;

public class NQuery : INQuery
{
    private readonly ConcurrentDictionary<string, object> _memoryDb;
    private readonly NQueryConfiguration _configuration;
    private readonly IDatabase _redisDb;

    private event EventHandler<QueryEventArgs> OnInserted;

    private NQuery(NQueryConfiguration nQueryConfiguration)
    {
        _memoryDb = new ConcurrentDictionary<string, object>();
        _configuration = nQueryConfiguration;
        var queryEvents = new NQueryEvents(_memoryDb, _configuration);
        OnInserted += queryEvents.Run;
    }

    private NQuery(NQueryConfiguration nQueryConfiguration, IConnectionMultiplexer connectionMultiplexer)
    {
        _configuration = nQueryConfiguration;
        _redisDb = connectionMultiplexer.GetDatabase();
    }

    public static NQuery Create(NQueryConfiguration configuration)
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
                => new NQuery(configuration),
            { UseInMemory: false, RedisConfiguration: not null }
                => new NQuery(configuration, ConnectionMultiplexer.Connect(configuration.RedisConfiguration.Endpoint)),
            _
                => throw new ArgumentException("Can't initialize NQuery")
        };
    }

    public async Task<TOutput?> QueryAsync<TOutput>(string key, Func<Task<TOutput?>> query, CancellationToken cancellationToken = default)
    {
        return _configuration.UseInMemory
            ? await InMemoryQuery(key, query, cancellationToken)
            : await ExternalQuery(key, query, cancellationToken);
    }
    
    public async Task<IEnumerable<TOutput>> QueryAsync<TOutput>(string key, Func<Task<IEnumerable<TOutput>>> query, CancellationToken cancellationToken = default)
    {
        return _configuration.UseInMemory
            ? await InMemoryQuery(key, query, cancellationToken)
            : await ExternalQuery(key, query, cancellationToken);
    }

    private async Task<TOutput> InMemoryQuery<TOutput>(string key, Func<Task<TOutput>> query, CancellationToken cancellationToken)
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

    private async Task<IEnumerable<TOutput>> InMemoryQuery<TOutput>(string key, Func<Task<IEnumerable<TOutput>>> query, CancellationToken cancellationToken)
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
    private async Task<TOutput?> ExternalQuery<TOutput>(string key, Func<Task<TOutput>> query, CancellationToken cancellationToken)
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
    
    private async Task<IEnumerable<TOutput>> ExternalQuery<TOutput>(string key, Func<Task<IEnumerable<TOutput>>> query, CancellationToken cancellationToken)
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