namespace NQuery.Interfaces;

public interface INQuery
{
    Task<TOutput?> QueryAsync<TOutput>(
        string key, 
        Func<Task<TOutput?>> query,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TOutput>> QueryAsync<TOutput>(
        string key, 
        Func<Task<IEnumerable<TOutput>>> query,
        CancellationToken cancellationToken = default);
}