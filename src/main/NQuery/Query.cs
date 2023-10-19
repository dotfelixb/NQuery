using System.Collections.Concurrent;
using ArgumentNullException = System.ArgumentNullException;

namespace NQuery;

public class Query
{
    private readonly ConcurrentDictionary<string, object> _data = new();
    private readonly QueryConfiguration _queryConfiguration;


    event EventHandler<QueryEventArgs> OnInserted;

    private Query(QueryConfiguration queryConfiguration)
    {
        _queryConfiguration = queryConfiguration; 
    }

    public static Query Create(QueryConfiguration configuration)
    {
        // pre-check
        if (configuration is { UseInMemory: false, RedisConfiguration: null })
        {
            throw new ArgumentException("RedisConfiguration required if not using InMemory Query Cache");
        }


        var query = new Query(configuration);
        var queryEvents = new QueryEvents(query._data, configuration);
        query.OnInserted += queryEvents.Run;
        return query;
    }

    public Task QueryAsync()
    {
        return Task.CompletedTask;
    }

    public async Task<TOutput?> QueryAsync<TOutput>(string key, Func<Task<TOutput>> query)
    {
        // check if key is already set
        if (_data.TryGetValue(key, out var value))
        {
            return (TOutput)value;
        }

        // key not set 
        var rst = await query();
        if (rst is null) return rst;

        _data[key] = rst;
        OnInserted?.Invoke(this, new QueryEventArgs { Key = key });

        return rst;
    }
}