using System.Collections.Concurrent;

namespace NQuery;

internal class QueryEventArgs : EventArgs
{
    public string Key { get; set; } = null!;
}

internal class NQueryEvents
{
    private readonly NQueryWorker _worker;

    public NQueryEvents(ConcurrentDictionary<string, object> data, 
        NQueryConfiguration nQueryConfiguration)
    {
        _worker = new NQueryWorker(data, nQueryConfiguration);
    }

    public void Run(object sender, QueryEventArgs e)
    {
        var thread = new Thread(() => _worker.Remove(e.Key));
        thread.Start();
    }
}