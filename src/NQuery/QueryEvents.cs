using System.Collections.Concurrent;

namespace NQuery;

public class QueryEventArgs : EventArgs
{
    public string Key { get; set; } = null!;
}

public class QueryEvents
{
    private readonly QueryWorker _worker;
    private readonly QueryConfiguration _queryConfiguration;
    public QueryEvents(ConcurrentDictionary<string, object> data, 
        QueryConfiguration queryConfiguration)
    {
        _queryConfiguration = queryConfiguration;
        _worker = new QueryWorker(data, _queryConfiguration);
    }

    public void Run(object sender, QueryEventArgs e)
    {
        var thread = new Thread(() => _worker.Remove(e.Key));
        thread.Start();
    }
}