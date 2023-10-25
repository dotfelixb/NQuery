NQuery
=======

Simple caching layer for data fetching


### Installing MediatR

You can install [NQuery with NuGet](https://www.nuget.org/packages/NQuery.Redis):

    Install-Package NQuery

Or for Redis

    Install-Package NQuery.Redis
    
Or via the .NET Core command line interface:

    dotnet add package NQuery

Or for Redis

    dotnet add package NQuery

### Using NQuery

You can use NQuery as

```csharp
var query = NQuery.Create(new NQueryConfiguration{
    UseInMemory = true
});

await query.Query( "query-key", await () => QueryYourData() );
```
Or for Asp.net Core Dependency for Redis as

```csharp
services.AddNQuery(cfg =>
{
    cfg.UseRedis(opts =>
    {
        opts.Endpoints.Add(new RedisEndpoint() { Host = "localhost", Port = 6379 });
    });
})
```

### Examples
See Test