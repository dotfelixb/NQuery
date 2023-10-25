using Microsoft.Extensions.DependencyInjection;

namespace NQuery.Core;

public static class NQueryServiceExtension
{
    public static IServiceCollection AddNQuery(
        this IServiceCollection services,
        Action<NQueryConfiguration> action)
    {
        var configuration = new NQueryConfiguration();
        action.Invoke(configuration);
        return services.AddNQuery(configuration);
    }
    
    static IServiceCollection AddNQuery(
        this IServiceCollection services,
        NQueryConfiguration configuration)
    {
        services.AddSingleton<Interfaces.INQuery>(x => Core.NQuery.Create(configuration));
        return services;
    }
}