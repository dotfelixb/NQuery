using Microsoft.Extensions.DependencyInjection;
using NQuery.Core;
using NQuery.Interfaces;
using NQuery.Redis;

namespace NQuery.Test;

public class AspNetQueryTest
{
    readonly ServiceCollection _services = new ServiceCollection();
    
    [SetUp]
    public void Setup()
    {
        _services.AddNQuery(cfg =>
        {
            cfg.UseRedis(opts =>
            {
                opts.Endpoints.Add(new RedisEndpoint() { Host = "127.0.0.1", Port = 6379 });
            });
        }); 
    }

    [Test]
    public void TestAspNetCoreServices()
    {
        var provider = _services.BuildServiceProvider();
        var queryType = provider.GetRequiredService<INQuery>();
        Assert.That(queryType, Is.InstanceOf<INQuery>());
    }
}