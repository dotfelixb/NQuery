using System.Collections.Concurrent;

namespace NQuery;

public class QueryWorker
{
    private readonly ConcurrentDictionary<string, object> _data;
    private readonly QueryConfiguration _queryConfiguration;

    public QueryWorker(ConcurrentDictionary<string, object> data, 
        QueryConfiguration queryConfiguration)
    {
        _data = data;
        _queryConfiguration = queryConfiguration;
    }

    public void Remove(string key)
    {
        Thread.Sleep(_queryConfiguration.CacheDuration);
        _data.Remove(key, out var _);
    }
}