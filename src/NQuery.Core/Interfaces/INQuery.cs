namespace NQuery.Core.Interfaces;

public interface INQuery
{
    Task<TOutput?> QueryAsync<TOutput>(
        string key, 
        Func<Task<TOutput?>> query);

    Task<IEnumerable<TOutput>> QueryAsync<TOutput>(
        string key, 
        Func<Task<IEnumerable<TOutput>>> query);

    Task<TOutput?> MutationAsync<TOutput>(
        string key,
        Func<Task<TOutput?>> mutate);
}