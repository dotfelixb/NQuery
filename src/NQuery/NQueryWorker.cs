using System.Collections.Concurrent;

namespace NQuery;

internal class NQueryWorker
{
    private readonly ConcurrentDictionary<string, object> _data;
    private readonly NQueryConfiguration _nQueryConfiguration;

    public NQueryWorker(ConcurrentDictionary<string, object> data, 
        NQueryConfiguration nQueryConfiguration)
    {
        _data = data;
        _nQueryConfiguration = nQueryConfiguration;
    }

    public void Remove(string key)
    {
        Thread.Sleep(_nQueryConfiguration.CacheDuration);
        _data.Remove(key, out var _);
    }
}